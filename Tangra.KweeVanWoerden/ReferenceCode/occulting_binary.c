#include <stdheaders.h>

int Kwee_van_Woerden ( long Number_Obs, double Time_First, double Time[10000], double Variable_Star_DN[10000], double Variable_Sky_DN[10000], double Comparison_Star_DN[10000], double Comparison_Sky_DN[10000], char Directory_Name[256] )
{

/* Constant parameter */
long         Normal_Points = 101;                     /* Number of evenly spaced data pairs used in the analysis */
long         Normal_Point_Middle;                     /* Index of the middle element of the normal point array */

/* Luminosity values */
double       Luminosity_Ratio[10000];                 /* Variable luminosity divided by comparison luminosity */
double       Luminosity_Normal_Sum;                   /* Summation of luminosities for normal point computation */
long         Luminosity_Normal_Count;                 /* Counter for data points in one normal point */
double       Luminosity_Normal[102];                  /* Normalized luminosities */

/* Time values */
double       Time_Interval;                           /* Time interval between observations */
double       Time_Start, Time_Stop;                   /* Limits for normal points */
double       Time_Normal[102];                        /* Times corresponding to normal points */

/* Symmetry analysis */
double       Luminosity_Faintest;                     /* Faintest normalized luminosity */
long         Luminosity_Faintest_Index;               /* Index of the faintest normalized luminosity */
long         Start_Light_Curve;                       /* Start element of normalized light curve to analyze */
long         Middle_Light_Curve;                      /* Middle element of normalized light curve to analyze */
long         Stop_Light_Curve;                        /* Stop element of normalized light curve to analyze */
long         Start_Sum_Squares;                       /* Start element of the sum-of-squares array */
long         Stop_Sum_Squares;                        /* Stop element of the sum-of-squares array */
double       Sum_Of_Squares;                          /* Sum of squares across a time of symmetry */
long         Sum_Of_Squares_Count[102];               /* Number of squares accumulated */
double       Sum_Of_Squares_Mean[102];                /* Sum of squares divided by count */
long         Sum_Of_Squares_Smallest_Index;           /* Index of the smallest sum of squares */
double       Sum_Of_Squares_Smallest;                 /* Smallest sum of squares */

/* Time of minimum and uncertainty */
double       Quad[4];                                 /* Inputs to the quadratic equation taken from the sums of squares */
double       A, B, C;                                 /* Intermediate values for quadratic equation */
double       T0;                                      /* Time of symmetry */
double       T0_Uncertainty;                          /* Time of symmetry uncertainty */
double       Time_Of_Minimum;                         /* Time of minimum light */
double       Time_Of_Minimum_Uncertainty;             /* Time of minimum light uncertainty */

/* Output file names */
char         Summary_File[256];                       /* Path and directory name of the summary file */
char         Observations_File[256];                  /* Path and directory name of the observations file */
char         Normals_File[256];                       /* Path and directory name of the normal point file */

/* Loop indices */
int          i, j;                                    /* Loop indices */

/* Output files */
FILE *Summary;
FILE *Normals;
FILE *Observations;

/* Build the path/file names for output files */
strcpy (Summary_File, Directory_Name);
strcat (Summary_File, "Time_Of_Minimum_Summary.Txt");
strcpy (Observations_File, Directory_Name);
strcat (Observations_File, "Time_Of_Minimum_Observations.Txt");
strcpy (Normals_File, Directory_Name);
strcat (Normals_File, "Time_Of_Minimum_Normals.Txt");

/* Open the files and print column headers */
Summary = fopen(Summary_File,"w");
Observations = fopen(Observations_File,"w");
Normals = fopen(Normals_File,"w");
fprintf(Observations, "                Time     Luminosity_Ratio     Variable_Star_DN      Variable_Sky_DN   Comparison_Star_DN    Comparison_Sky_DN \n");
fprintf(Normals,"                Time    Luminosity_Ratio \n");

/* Determine luminosity ratios */
for (i = 1; i <= Number_Obs; i++) {
  Luminosity_Ratio[i] = ( Variable_Star_DN[i] - Variable_Sky_DN[i] ) / ( Comparison_Star_DN[i] - Comparison_Sky_DN[i] );
  /* Write to observation file */
  fprintf (Observations, "%20.6lf %20.6lf %20.6lf %20.6lf %20.6lf %20.6lf \n", Time[i], Luminosity_Ratio[i], Variable_Star_DN[i], Variable_Sky_DN[i], Comparison_Star_DN[i], Comparison_Sky_DN[i]);
}

/* Compute normal point times and luminosities */
Time_Interval = ( Time[Number_Obs] - Time[1] ) / (float)Normal_Points;
for (i = 1; i <= Normal_Points; i++) {
  Time_Start = Time[1] + ( i - 1 ) * Time_Interval;
  Time_Normal[i] = Time_Start + Time_Interval / 2.;
  Time_Stop = Time_Start + Time_Interval;
  Luminosity_Normal_Sum = 0.;
  Luminosity_Normal_Count = 0;
  for (j = 1; j <= Number_Obs; j++ ) {
    if ( ( Time[j] >= Time_Start ) && ( Time[j] < Time_Stop) ) {
      Luminosity_Normal_Sum = Luminosity_Normal_Sum + Luminosity_Ratio[j];
      Luminosity_Normal_Count = Luminosity_Normal_Count + 1;
    }
  }
  if ( Luminosity_Normal_Count > 0 ) {
    Luminosity_Normal[i] = Luminosity_Normal_Sum / (float)Luminosity_Normal_Count;
    /* printf ("i, Luminosity_Normal_Count, Normal_Points = %i %i %i \n", i, Luminosity_Normal_Count, Normal_Points); */
  }
  else {
    /* printf ("i, Luminosity_Normal_Count = %i %i \n", i, Luminosity_Normal_Count); */
    /* printf ("Error in computing normals \n"); */
    return 1;
  }
  /* Write to normal point file */
  fprintf (Normals, "%20.6lf %20.6lf \n", Time_Normal[i], Luminosity_Normal[i]);
}

/* Locate the faintest luminosity */
Luminosity_Faintest = 1000000000.;
for (i = 1; i <= Normal_Points; i++) {
  if ( Luminosity_Normal[i] < Luminosity_Faintest ) {
    Luminosity_Faintest_Index = i;
    Luminosity_Faintest = Luminosity_Normal[i];
  }
}

/* Set the limits of the light curve to be symmetrical around the faintest luminosity */
Start_Light_Curve = 1;
Stop_Light_Curve = Normal_Points;
Normal_Point_Middle = Normal_Points / 2 + 1;
if ( Luminosity_Faintest_Index < Normal_Point_Middle ) { Stop_Light_Curve = 2 * Luminosity_Faintest_Index + 1; }
if ( Luminosity_Faintest_Index > Normal_Point_Middle ) { Start_Light_Curve = 2 * Luminosity_Faintest_Index - Normal_Points; }

/* Compute the normalized sums of squares of luminosity differences across an array of times */
Start_Sum_Squares = Start_Light_Curve + 1;
Stop_Sum_Squares  = Stop_Light_Curve  - 1;
for (i = Start_Sum_Squares; i <= Stop_Sum_Squares; i++) {
  Sum_Of_Squares = 0.;
  Sum_Of_Squares_Count[i] = 0;
  for (j = 1; j <= Normal_Points; j++) {
    if ( i - j >= 1 && i + j <= Normal_Points ) {
      Sum_Of_Squares = Sum_Of_Squares + pow ( Luminosity_Normal[i-j] - Luminosity_Normal[i+j], 2 );
      Sum_Of_Squares_Count[i] = Sum_Of_Squares_Count[i] + 1;
    }
  }
  Sum_Of_Squares_Mean[i] = Sum_Of_Squares / (float) Sum_Of_Squares_Count[i];
}

/* Find the smallest normalized sum of squares */
Sum_Of_Squares_Smallest = 1000000000;
for (i = Start_Sum_Squares; i <= Stop_Sum_Squares; i++) {
  if ( Sum_Of_Squares_Mean[i] < Sum_Of_Squares_Smallest ) {
    /* Must also have a reasonable sample of points */
    if ( Sum_Of_Squares_Count[i] > Normal_Points / 10 ) {
      Sum_Of_Squares_Smallest_Index = i;
      Sum_Of_Squares_Smallest = Sum_Of_Squares_Mean[i];
    }
  }
}

/* Solve the quadratic equation */
for (i = 1; i <= 3; i++) {
  Quad[i] = Sum_Of_Squares_Mean[Sum_Of_Squares_Smallest_Index+i-2];
}
A = -Quad[2] + ( Quad[3] + Quad[1] ) / 2.0;
B = -3.0 * A + Quad[2] - Quad[1];
C = Quad[1] - A - B;

/* Calculate the time of minimum and uncertainty */
T0 = -B / ( 2.0 * A );
T0_Uncertainty = sqrt((4.0 * A * C - B * B) / (4.0 * A * A) / ((float)Normal_Points / 4.0 - 1.0));
Time_Of_Minimum = Time_Normal[1] + ( ( Sum_Of_Squares_Smallest_Index - 1 ) + ( T0 - 2.0 ) ) * Time_Interval;
Time_Of_Minimum_Uncertainty = T0_Uncertainty * Time_Interval;
fprintf(Summary, "      Time_First               Number_Obs \n");
fprintf(Summary, "%20.6lf %20i \n", Time_First, Number_Obs);

fprintf(Summary, "\n               Time of minimum: \n \n");
fprintf(Summary, "      Bins-Quadratic          Uncertainty \n");
fprintf(Summary, "%20.6lf %20.6lf \n", T0, T0_Uncertainty);
fprintf(Summary, "      From Time_First         Uncertainty \n");
fprintf(Summary, "%20.6lf %20.6lf \n", Time_Of_Minimum, Time_Of_Minimum_Uncertainty);
fprintf(Summary, "      Absolute                Uncertainty \n");
fprintf(Summary, "%20.6lf %20.6lf \n", Time_First + Time_Of_Minimum, Time_Of_Minimum_Uncertainty);

/* Finish */
return 0;

}

int main(void)
{

/* Parametric quantities */
double       Time_Of_Minimum;                         /* Time of minimum light */
double       Duration_Of_Occultation;                 /* Duration of the occultation */
double       Start_Observation, Stop_Observation;     /* Observation start and stop times */
double       Start_Occultation, Stop_Occultation;     /* Occultation start and stop times */
double       Time_Interval;                           /* Time interval between observations */
double       Variable_Maximum_DNs;                    /* DNs for variable star outside of occultation */
double       Comparison_DNs;                          /* DNs for comparison star */
double       Sky_to_Star_Fraction;                    /* Ratio of DNs for sky to star */
double       Occultation_Depth;                       /* Luminosity decrease at minimum light (normalized) */
double       Noise_DNs;                               /* Noise to be added to stars and sky backgrounds */

/* Variables passed to subroutine */
long         Number_Obs;                              /* Number of observed data points */
double       Time_First;                              /* Absolute time for the first observation */
double       Time[10000];                             /* Mid-times of each observation */
double       Variable_Star_DN[10000];                 /* Pixels values for the variable star brightness (including sky background) */
double       Variable_Sky_DN[10000];                  /* Pixels values for the variable star sky background to be subtracted */
double       Comparison_Star_DN[10000];               /* Pixels values for the comparison star brightness (including sky background) */
double       Comparison_Sky_DN[10000];                /* Pixels values for the comparison star sky background to be subtracted */
char         Directory_Name[256];                     /* Location for the output files */

/* Other quantities */
long         i, j;                                    /* Loop indices */
double       Phase_Inside_Of_Occultation;             /* 0 at mid-event, 1 at beginning or end of occultation */
double       Random;                                  /* Random number */
int          Random_Int;                              /* Random number, integer */

/* Announce the program */
printf("Occulting Binary in Variable Stars Directory\n");
strcpy (Directory_Name, "C:\\Tech\\Variable_Stars\\Kwee_van_Woerden_C\\");

/* Initialize */
Time_First = 2456967.1234567;
Time_Of_Minimum = 0.06;
Duration_Of_Occultation = 0.05;
Start_Occultation = Time_Of_Minimum - 0.5 * Duration_Of_Occultation;
Stop_Occultation  = Time_Of_Minimum + 0.5 * Duration_Of_Occultation;
Start_Observation = 0.00;
Stop_Observation  = 0.08;
Time_Interval = 0.00001;
Number_Obs = ( Stop_Observation - Start_Observation ) / Time_Interval;
Variable_Maximum_DNs = 10000.0;
Comparison_DNs       =  5000.0;
Noise_DNs            =  1000.0;
Sky_to_Star_Fraction = 0.1;
Occultation_Depth = 0.7;

/* Simulate the light curve */
for (i = 1; i <= Number_Obs; i++) {
  Time[i] = Start_Observation + ( i - 1 ) * Time_Interval;

  if ( Time[i] < Start_Occultation || Time[i] > Stop_Occultation ) {
    /* Before or after occultation */
    Variable_Star_DN[i]   = Variable_Maximum_DNs;
  }

  else {
    /* During occultation */
    Phase_Inside_Of_Occultation = fabs ( Time[i] - Time_Of_Minimum ) / ( Duration_Of_Occultation / 2.0 );
    Variable_Star_DN[i]   = Variable_Maximum_DNs * ( 1.0 - Occultation_Depth ) + Phase_Inside_Of_Occultation * Variable_Maximum_DNs * Occultation_Depth;
  }

  Variable_Sky_DN[i]    = Variable_Star_DN[i] * Sky_to_Star_Fraction;
  Comparison_Star_DN[i] = Comparison_DNs;
  Comparison_Sky_DN[i]  = Comparison_Star_DN[i] * Sky_to_Star_Fraction;

}

/* Add noise */
srand ( time(NULL) );

for (i = 1; i <= Number_Obs; i++) {

  Random_Int = rand();
  Random = (float)Random_Int / (float)RAND_MAX;
  Variable_Star_DN[i] = Variable_Star_DN[i] + 2.0 * Noise_DNs * ( 0.5 - Random );
  if ( Variable_Star_DN[i] < 0.0 ) { Variable_Star_DN[i] = 0.0; }

  Random_Int = rand();
  Random = (float)Random_Int / (float)RAND_MAX;
  Variable_Sky_DN[i] = Variable_Sky_DN[i] + 2.0 * Noise_DNs * ( 0.5 - Random );
  if ( Variable_Sky_DN[i] < 0.0 ) { Variable_Sky_DN[i] = 0.0; }

  Random_Int = rand();
  Random = (float)Random_Int / (float)RAND_MAX;
  Comparison_Star_DN[i] = Comparison_Star_DN[i] + 2.0 * Noise_DNs * ( 0.5 - Random );
  if ( Comparison_Star_DN[i] < 0.0 ) { Comparison_Star_DN[i] = 0.0; }

  Random_Int = rand();
  Random = (float)Random_Int / (float)RAND_MAX;
  Comparison_Sky_DN[i] = Comparison_Sky_DN[i] + 2.0 * Noise_DNs * ( 0.5 - Random );
  if ( Comparison_Sky_DN[i] < 0.0 ) { Comparison_Sky_DN[i] = 0.0; }

}

/* Call the subroutine */
i = Kwee_van_Woerden ( Number_Obs, Time_First, Time, Variable_Star_DN, Variable_Sky_DN, Comparison_Star_DN, Comparison_Sky_DN, Directory_Name );

if ( i == 1 ) printf ("Error in computing normal points \n");

/* Finish */
return 0;

}

