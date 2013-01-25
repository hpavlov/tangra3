using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Numerical
{
    public class Integration
    {
        public static double RombergIntegration(Func<double, double> y, double a, double b, int n)
        {
            int i, j, m = 0;
            double h, trap;
            double[,] I = new double[n, n];
            h = b - a;
            I[0, 0] = (y(a) + y(b)) * (h / 2);
            if (n > 15) n = 15;
            do
            {
                m++;
                h = h / 2;
                trap = (y(a) + y(b)) / 2;
                for (i = 1; i < Math.Pow(2, m); i++) trap += y(a + h * i);
                I[0, m] = trap * h;
                for (i = 1; i < m + 1; i++)
                {
                    j = m - i;
                    I[i, j] = (Math.Pow(4, i) * I[i - 1, j + 1] - I[i - 1, j]) / (Math.Pow(4, i) - 1);
                }
            }
            while (m < n - 1);

            return I[n - 1, 0];
        }
        // end of RombergIntegration 

        private static object s_SyncRoot = new object();
        private static double[] s_ra = null;
        private static double[] s_rw = null;

        //!*******************************************************************************
        //!
        //!! CIRCLE_ANNULUS approximates an integral in an annulus.
        //!
        //!
        //!  Integration region:
        //!
        //!    Points (X,Y) such that
        //!
        //!      RADIUS1**2 <= ( X - XC )**2 + ( Y - YC )**2 <= RADIUS2**2
        //!
        //!  Modified:
        //!
        //!    17 January 2001
        //!
        //!  Author:
        //!
        //!    John Burkardt
        //!
        //!  Reference:
        //!
        //!    William Peirce,
        //!    Numerical Integration Over the Planar Annulus,
        //!    Journal of the Society for Industrial and Applied Mathematics,
        //!    Volume 5, Issue 2, June 1957, pages 66-73.
        //!
        //!  Parameters:
        //!
        //!    Input, external FUNC, the name of the user supplied function of two
        //!    variables which is to be integrated, of the form:
        //!
        //!      function func ( x, y )
        //!      double precision func
        //!      double precision x
        //!      double precision y
        //!
        //!    Input, double precision XC, YC, the center of the circle.
        //!
        //!    Input, double precision RADIUS1, RADIUS2, the radii of the circles.
        //!
        //!    Input, integer NR, the order of the rule.  This quantity specifies
        //!    the number of distinct radii to use.  The number of angles used will
        //!    be 4*NR, for a total of 4*NR**2 points.
        //!
        //!    Output, double precision RESULT, the approximation to the integral.
        //!
        public static double IntegrationOverCircularArea(Func<double, double, double> z, double radius)
        {
            const int num = 64;

            //!
            //!  Choose radial abscissas and weights.
            //!
            //  call legendre_set ( nr, ra, rw )
            if (s_ra == null)
            {
                lock (s_SyncRoot)
                {
                    s_ra = new double[num];
                    s_rw = new double[num];

                    legendre_set(num, s_ra, s_rw);

                    //  a = -1.0E+00;
                    //  b = +1.0E+00;
                    //  c = radius1**2
                    //  d = radius2**2
                    //  call rule_adjust ( a, b, c, d, nr, ra, rw )
                    //  ra(1:nr) = sqrt ( ra(1:nr) )
                    //  rw(1:nr) = rw(1:nr) / ( radius2**2 - radius1**2 )
                    double a = -1.0;
                    double b = 1.0;
                    double c = 0;
                    double d = radius * radius;
                    rule_adjust(a, b, c, d, num, s_ra, s_rw);

                    for (int i = 0; i < num; i++)
                    {
                        s_ra[i] = Math.Sqrt(s_ra[i]);
                        s_rw[i] = s_rw[i] / d;
                    }
                }
            }

            double n = 4.0 * num;
            double quad = 0;
            double tw = 1.0 / n;

            for (int i = 0; i < n; i++)
            {
                double t = 2.0 * Math.PI * (i - 1) / n;

                for (int j = 0; j < num; j++)
                {
                    double x = 0 + s_ra[j] * Math.Cos(t);
                    double y = 0 + s_ra[j] * Math.Sin(t);

                    quad = quad + tw * s_rw[j] * z(x, y);
                }
            }

            return quad * Math.PI * radius * radius;
        }


        //!*******************************************************************************
        //!
        //!! LEGENDRE_SET sets abscissas and weights for Gauss-Legendre quadrature.
        //!
        //!
        //!  Integration region:
        //!
        //!    [ -1, 1 ]
        //!
        //!  Weight function:
        //!
        //!    1.0E+00;
        //!
        //!  Integral to approximate:
        //!
        //!    INTEGRAL ( -1 <= X <= 1 ) F(X) dX
        //!
        //!  Approximate integral:
        //!
        //!    SUM ( I = 1 to NORDER ) WEIGHT(I) * F ( XTAB(I) )
        //!
        //!  Precision:
        //!
        //!    The quadrature rule will integrate exactly all polynomials up to
        //!    X**(2*NORDER-1).
        //!
        //!  Note:
        //!
        //!    The abscissas of the rule are the zeroes of the Legendre polynomial
        //!    P(NORDER)(X).
        //!
        //!    The integral produced by a Gauss-Legendre rule is equal to the
        //!    integral of the unique polynomial of degree NORDER-1 which
        //!    agrees with the function at the NORDER abscissas of the rule.
        //!
        //!  Reference:
        //!
        //!    Abramowitz and Stegun,
        //!    Handbook of Mathematical Functions,
        //!    National Bureau of Standards, 1964.
        //!
        //!    Vladimir Krylov,
        //!    Approximate Calculation of Integrals,
        //!    MacMillan, 1962.
        //!
        //!    Arthur Stroud and Don Secrest,
        //!    Gaussian Quadrature Formulas,
        //!    Prentice Hall, 1966.
        //!
        //!    Daniel Zwillinger, editor,
        //!    Standard Mathematical Tables and Formulae,
        //!    30th Edition,
        //!    CRC Press, 1996.
        //!
        //!  Modified:
        //!
        //!    18 December 2000
        //!
        //!  Author:
        //!
        //!    John Burkardt
        //!
        //!  Parameters:
        //!
        //!    Input, integer NORDER, the order of the rule.
        //!    NORDER must be between 1 and 20, 32 or 64.
        //!
        //!    Output, double precision XTAB(NORDER), the abscissas of the rule.
        //!
        //!    Output, double precision WEIGHT(NORDER), the weights of the rule.
        //!    The weights are positive, symmetric and should sum to 2.
        //!
        private static void legendre_set(int norder, double[] xtab, double[] weight)
        {
            if (norder != 64) throw new NotSupportedException("norder must be 64");

            xtab[0] = -0.999305041735772139456905624346E+00;
            xtab[1] = -0.996340116771955279346924500676E+00;
            xtab[2] = -0.991013371476744320739382383443E+00;
            xtab[3] = -0.983336253884625956931299302157E+00;
            xtab[4] = -0.973326827789910963741853507352E+00;
            xtab[5] = -0.961008799652053718918614121897E+00;
            xtab[6] = -0.946411374858402816062481491347E+00;
            xtab[7] = -0.929569172131939575821490154559E+00;
            xtab[8] = -0.910522137078502805756380668008E+00;
            xtab[9] = -0.889315445995114105853404038273E+00;
            xtab[10] = -0.865999398154092819760783385070E+00;
            xtab[11] = -0.840629296252580362751691544696E+00;
            xtab[12] = -0.813265315122797559741923338086E+00;
            xtab[13] = -0.783972358943341407610220525214E+00;
            xtab[14] = -0.752819907260531896611863774886E+00;
            xtab[15] = -0.719881850171610826848940217832E+00;
            xtab[16] = -0.685236313054233242563558371031E+00;
            xtab[17] = -0.648965471254657339857761231993E+00;
            xtab[18] = -0.611155355172393250248852971019E+00;
            xtab[19] = -0.571895646202634034283878116659E+00;
            xtab[20] = -0.531279464019894545658013903544E+00;
            xtab[21] = -0.489403145707052957478526307022E+00;
            xtab[22] = -0.446366017253464087984947714759E+00;
            xtab[23] = -0.402270157963991603695766771260E+00;
            xtab[24] = -0.357220158337668115950442615046E+00;
            xtab[25] = -0.311322871990210956157512698560E+00;
            xtab[26] = -0.264687162208767416373964172510E+00;
            xtab[27] = -0.217423643740007084149648748989E+00;
            xtab[28] = -0.169644420423992818037313629748E+00;
            xtab[29] = -0.121462819296120554470376463492E+00;
            xtab[30] = -0.729931217877990394495429419403E-01;
            xtab[31] = -0.243502926634244325089558428537E-01;
            xtab[32] = 0.243502926634244325089558428537E-01;
            xtab[33] = 0.729931217877990394495429419403E-01;
            xtab[34] = 0.121462819296120554470376463492E+00;
            xtab[35] = 0.169644420423992818037313629748E+00;
            xtab[36] = 0.217423643740007084149648748989E+00;
            xtab[37] = 0.264687162208767416373964172510E+00;
            xtab[38] = 0.311322871990210956157512698560E+00;
            xtab[39] = 0.357220158337668115950442615046E+00;
            xtab[40] = 0.402270157963991603695766771260E+00;
            xtab[41] = 0.446366017253464087984947714759E+00;
            xtab[42] = 0.489403145707052957478526307022E+00;
            xtab[43] = 0.531279464019894545658013903544E+00;
            xtab[44] = 0.571895646202634034283878116659E+00;
            xtab[45] = 0.611155355172393250248852971019E+00;
            xtab[46] = 0.648965471254657339857761231993E+00;
            xtab[47] = 0.685236313054233242563558371031E+00;
            xtab[48] = 0.719881850171610826848940217832E+00;
            xtab[49] = 0.752819907260531896611863774886E+00;
            xtab[50] = 0.783972358943341407610220525214E+00;
            xtab[51] = 0.813265315122797559741923338086E+00;
            xtab[52] = 0.840629296252580362751691544696E+00;
            xtab[53] = 0.865999398154092819760783385070E+00;
            xtab[54] = 0.889315445995114105853404038273E+00;
            xtab[55] = 0.910522137078502805756380668008E+00;
            xtab[56] = 0.929569172131939575821490154559E+00;
            xtab[57] = 0.946411374858402816062481491347E+00;
            xtab[58] = 0.961008799652053718918614121897E+00;
            xtab[59] = 0.973326827789910963741853507352E+00;
            xtab[60] = 0.983336253884625956931299302157E+00;
            xtab[61] = 0.991013371476744320739382383443E+00;
            xtab[62] = 0.996340116771955279346924500676E+00;
            xtab[63] = 0.999305041735772139456905624346E+00;

            weight[0] = 0.178328072169643294729607914497E-02;
            weight[1] = 0.414703326056246763528753572855E-02;
            weight[2] = 0.650445796897836285611736039998E-02;
            weight[3] = 0.884675982636394772303091465973E-02;
            weight[4] = 0.111681394601311288185904930192E-01;
            weight[5] = 0.134630478967186425980607666860E-01;
            weight[6] = 0.157260304760247193219659952975E-01;
            weight[7] = 0.179517157756973430850453020011E-01;
            weight[8] = 0.201348231535302093723403167285E-01;
            weight[9] = 0.222701738083832541592983303842E-01;
            weight[10] = 0.243527025687108733381775504091E-01;
            weight[11] = 0.263774697150546586716917926252E-01;
            weight[12] = 0.283396726142594832275113052002E-01;
            weight[13] = 0.302346570724024788679740598195E-01;
            weight[14] = 0.320579283548515535854675043479E-01;
            weight[15] = 0.338051618371416093915654821107E-01;
            weight[16] = 0.354722132568823838106931467152E-01;
            weight[17] = 0.370551285402400460404151018096E-01;
            weight[18] = 0.385501531786156291289624969468E-01;
            weight[19] = 0.399537411327203413866569261283E-01;
            weight[20] = 0.412625632426235286101562974736E-01;
            weight[21] = 0.424735151236535890073397679088E-01;
            weight[22] = 0.435837245293234533768278609737E-01;
            weight[23] = 0.445905581637565630601347100309E-01;
            weight[24] = 0.454916279274181444797709969713E-01;
            weight[25] = 0.462847965813144172959532492323E-01;
            weight[26] = 0.469681828162100173253262857546E-01;
            weight[27] = 0.475401657148303086622822069442E-01;
            weight[28] = 0.479993885964583077281261798713E-01;
            weight[29] = 0.483447622348029571697695271580E-01;
            weight[30] = 0.485754674415034269347990667840E-01;
            weight[31] = 0.486909570091397203833653907347E-01;
            weight[32] = 0.486909570091397203833653907347E-01;
            weight[33] = 0.485754674415034269347990667840E-01;
            weight[34] = 0.483447622348029571697695271580E-01;
            weight[35] = 0.479993885964583077281261798713E-01;
            weight[36] = 0.475401657148303086622822069442E-01;
            weight[37] = 0.469681828162100173253262857546E-01;
            weight[38] = 0.462847965813144172959532492323E-01;
            weight[39] = 0.454916279274181444797709969713E-01;
            weight[40] = 0.445905581637565630601347100309E-01;
            weight[41] = 0.435837245293234533768278609737E-01;
            weight[42] = 0.424735151236535890073397679088E-01;
            weight[43] = 0.412625632426235286101562974736E-01;
            weight[44] = 0.399537411327203413866569261283E-01;
            weight[45] = 0.385501531786156291289624969468E-01;
            weight[46] = 0.370551285402400460404151018096E-01;
            weight[47] = 0.354722132568823838106931467152E-01;
            weight[48] = 0.338051618371416093915654821107E-01;
            weight[49] = 0.320579283548515535854675043479E-01;
            weight[50] = 0.302346570724024788679740598195E-01;
            weight[51] = 0.283396726142594832275113052002E-01;
            weight[52] = 0.263774697150546586716917926252E-01;
            weight[53] = 0.243527025687108733381775504091E-01;
            weight[54] = 0.222701738083832541592983303842E-01;
            weight[55] = 0.201348231535302093723403167285E-01;
            weight[56] = 0.179517157756973430850453020011E-01;
            weight[57] = 0.157260304760247193219659952975E-01;
            weight[58] = 0.134630478967186425980607666860E-01;
            weight[59] = 0.111681394601311288185904930192E-01;
            weight[60] = 0.884675982636394772303091465973E-02;
            weight[61] = 0.650445796897836285611736039998E-02;
            weight[62] = 0.414703326056246763528753572855E-02;
            weight[63] = 0.178328072169643294729607914497E-02;
        }

        //!
        //!*******************************************************************************
        //!
        //!! RULE_ADJUST maps a quadrature rule from [A,B] to [C,D].
        //!
        //!
        //!  Discussion:
        //!
        //!    Most quadrature rules are defined on a special interval, like
        //!    [-1,1] or [0,1].  To integrate over an interval, the abscissas
        //!    and weights must be adjusted.  This can be done on the fly,
        //!    or by calling this routine.
        //!
        //!    If the weight function W(X) is not 1, then the W vector will
        //!    require further adjustment by the user.
        //!
        //!  Modified:
        //!
        //!    06 December 2000
        //!
        //!  Author:
        //!
        //!    John Burkardt
        //!
        //!  Parameters:
        //!
        //!    Input, double precision A, B, the endpoints of the definition interval.
        //!
        //!    Input, double precision C, D, the endpoints of the integration interval.
        //!
        //!    Input, integer NORDER, the number of abscissas and weights.
        //!
        //!    Input/output, double precision X(NORDER), W(NORDER), the abscissas
        //!    and weights.
        //!
        private static void rule_adjust(double a, double b, double c, double d, int norder, double[] x, double[] w)
        {
            for (int i = 0; i < norder; i++)
            {
                x[i] = ((b - x[i]) * c + (x[i] - a) * d) / (b - a);
                w[i] = ((d - c) / (b - a)) * w[i];
            }
        }
    }
}
