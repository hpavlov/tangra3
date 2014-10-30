Subroutine Kwee_van_Woerden ( Number_Obs, Time, Variable_Star_DN, Variable_Sky_DN, Comparison_Star_DN, Comparison_Sky_DN )

! Determine the time of minimum light of an occulting binary star
! Uses the method of Kwee and van Woerden, B.A.N. Vol 12, Numb 464, Page 327, 1956.

! Every quantity must be explicitly declared
implicit none

! Input values
integer                    Number_Obs                              ! Number of observed data points
double precision           Time(10000)                             ! Mid-times of each observation
double precision           Variable_Star_DN(10000)                 ! Pixels values for the variable star brightness (including sky background)
double precision           Variable_Sky_DN(10000)                  ! Pixels values for the variable star sky background to be subtracted
double precision           Comparison_Star_DN(10000)               ! Pixels values for the comparison star brightness (including sky background)
double precision           Comparison_Sky_DN(10000)                ! Pixels values for the comparison star sky background to be subtracted

! Constant parameter
parameter                  Normal_Points = 101                     ! Number of evenly spaced data pairs used in the analysis
integer                    Normal_Point_Middle                     ! Index of the middle element of the normal point array

! Luminosity values
double precision           Luminosity_Ratio(10000)                 ! Variable luminosity divided by comparison luminosity
double precision           Luminosity_Normal_Sum                   ! Summation of luminosities for normal point computation
integer                    Luminosity_Normal_Count                 ! Counter for data points in one normal point
double precision           Luminosity_Normal(Normal_Points)        ! Normalized luminosities

! Time values
double precision           Time_Interval                           ! Time interval between observations, 
double precision           Time_Start, Time_Stop                   ! Limits for normal points
double precision           Time_Normal(Normal_Points)              ! Times corresponding to normal points

! Symmetry analysis
double precision           Luminosity_Faintest                     ! Faintest normalized luminosity
integer                    Luminosity_Faintest_Index               ! Index of the faintest normalized luminosity
integer                    Start_Light_Curve                       ! Start element of normalized light curve to analyze
integer                    Middle_Light_Curve                      ! Middle element of normalized light curve to analyze
integer                    Stop_Light_Curve                        ! Stop element of normalized light curve to analyze
integer                    Start_Sum_Squares                       ! Start element of the sum-of-squares array
integer                    Stop_Sum_Squares                        ! Stop element of the sum-of-squares array
double precision           Sum_Of_Squares                          ! Sum of squares across a time of symmetry
integer                    Sum_Of_Squares_Count(Normal_Points)     ! Number of squares accumulated
double precision           Sum_Of_Squares_Mean(Normal_Points)      ! Sum of squares divided by count
integer                    Sum_Of_Squares_Smallest_Index           ! Index of the smallest sum of squares
double precision           Sum_Of_Squares_Smallest                 ! Smallest sum of squares

! Time of minimum and uncertainty
double precision           Quad(3)                                 ! Inputs to the quadratic equation taken from the sums of squares
double precision           A, B, C                                 ! Intermediate values for quadratic equation
double precision           T0                                      ! Time of symmetry
double precision           T0_Uncertainty                          ! Time of symmetry uncertainty
double precision           Time_Of_Minimum                         ! Time of minimum light
double precision           Time_Of_Minimum_Uncertainty             ! Time of minimum light uncertainty

! Loop indices
integer                    i, j                                    ! Loop indices

! Open output files
open (unit=6,file='Time_Of_Minimum_Summary.Txt',action='write')
open (unit=7,file='Time_Of_Minimum_Observations.Txt',action='write')
open (unit=8,file='Time_Of_Minimum_Normals.Txt',action='write')

! Determine luminosity ratios
do i = 1, Number_Obs
  Luminosity_Ratio(i) = ( Variable_Star_DN(i) - Variable_Sky_DN(i) ) / ( Comparison_Star_DN(i) - Comparison_Sky_DN(i) )
  ! Write to observation file
  write (7,99) Time(i), Luminosity_Ratio(i), Variable_Star_DN(i), Variable_Sky_DN(i), Comparison_Star_DN(i), Comparison_Sky_DN(i)
  99 format (6f20.6)
enddo

! Compute normal point times and luminosities
Time_Interval = ( Time(Number_Obs) - Time(1) ) / Normal_Points
do i = 1, Normal_Points
  Time_Start = Time(1) + ( i - 1 ) * Time_Interval
  Time_Normal(i) = Time_Start + Time_Interval / 2.
  Time_Stop = Time_Start + Time_Interval
  Luminosity_Normal_Sum = 0.
  Luminosity_Normal_Count = 0
  do j = 1, Number_Obs
    if ( ( Time(j) >= Time_Start ) .and. ( Time(j) < Time_Stop) ) then
      Luminosity_Normal_Sum = Luminosity_Normal_Sum + Luminosity_Ratio(j)
      Luminosity_Normal_Count = Luminosity_Normal_Count + 1
    endif
  enddo
  if ( Luminosity_Normal_Count > 0 ) then
    Luminosity_Normal(i) = Luminosity_Normal_Sum / Luminosity_Normal_Count
  else
    write (0,*) 'No luminosities in a noraml point bin'
    stop
  endif
  ! Write to normal point file
  write (8,99) Time_Normal(i), Luminosity_Normal(i)
enddo

! Diagnostic
if (.false.) then
  write (0,*) 'Luminosity_Normal'
  do i = 1, Normal_Points
    write (0,*) i, Luminosity_Normal(i)
  enddo
  ! stop
endif

! Locate the faintest luminosity
Luminosity_Faintest = 1.0D10
do i = 1, Normal_Points
  if ( Luminosity_Normal(i) < Luminosity_Faintest ) then
    Luminosity_Faintest_Index = i
    Luminosity_Faintest = Luminosity_Normal(i)
  endif
enddo

! Diagnostic
if (.false.) then
  write (0,*) 'Luminosity_Faintest_Index, Luminosity_Faintest = ', Luminosity_Faintest_Index, Luminosity_Faintest
  ! stop
endif

! Set the limits of the light curve to be symmetrical around the faintest luminosity
Start_Light_Curve = 1
Stop_Light_Curve = Normal_Points
Normal_Point_Middle = int ( Normal_Points / 2 ) + 1
if ( Luminosity_Faintest_Index < Normal_Point_Middle ) Stop_Light_Curve = 2 * Luminosity_Faintest_Index + 1
if ( Luminosity_Faintest_Index > Normal_Point_Middle ) Start_Light_Curve = 2 * Luminosity_Faintest_Index - Normal_Points

! Diagnostic
if (.false.) then
  write (0,*) 'Start_Light_Curve, Stop_Light_Curve = ', Start_Light_Curve, Stop_Light_Curve
  ! stop
endif

! Compute the normalized sums of squares of luminosity differences across an array of times
Start_Sum_Squares = Start_Light_Curve + 1
Stop_Sum_Squares  = Stop_Light_Curve  - 1
do i = Start_Sum_Squares, Stop_Sum_Squares
  Sum_Of_Squares = 0.
  Sum_Of_Squares_Count(i) = 0
  do j = 1, Normal_Points
    if ( i - j >= 1 .and. i + j <= Normal_Points ) then
      Sum_Of_Squares = Sum_Of_Squares + ( Luminosity_Normal(i-j) - Luminosity_Normal(i+j) ) **2
      Sum_Of_Squares_Count(i) = Sum_Of_Squares_Count(i) + 1
    endif
  enddo
  Sum_Of_Squares_Mean(i) = Sum_Of_Squares / Sum_Of_Squares_Count(i)
enddo  

! Diagnostic
if (.false.) then
  write (0,*) 'Sum_Of_Squares_Mean'
  do i = Start_Sum_Squares, Stop_Sum_Squares
    write (0,*) i, Sum_Of_Squares_Mean(i), Sum_Of_Squares_Count(i)
  enddo
  stop
endif

! Find the smallest normalized sum of squares
Sum_Of_Squares_Smallest = 1.0D10
do i = Start_Sum_Squares, Stop_Sum_Squares
  if ( Sum_Of_Squares_Mean(i) < Sum_Of_Squares_Smallest ) then
    ! Must also have a reasonable sample of points
    if ( Sum_Of_Squares_Count(i) > Normal_Points / 10 ) then
      Sum_Of_Squares_Smallest_Index = i
      Sum_Of_Squares_Smallest = Sum_Of_Squares_Mean(i)
    endif
  endif
enddo

! Diagnostic
if (.false.) then
  write (0,*) 'Sum_Of_Squares_Smallest_Index, Sum_Of_Squares_Smallest = ', Sum_Of_Squares_Smallest_Index, Sum_Of_Squares_Smallest
  stop
endif

! Solve the quadratic equation
do i = 1, 3
  Quad(i) = Sum_Of_Squares_Mean(Sum_Of_Squares_Smallest_Index+i-2)
enddo
A = -Quad(2) + ( Quad(3) + Quad(1) ) / 2.0
B = -3.0 * A + Quad(2) - Quad(1)
C = Quad(1) - A - B

! Diagnostic
if (.false.) then
  write (0,*) 'Quad 1/2/3, Time interval = ', Quad, Time_Interval
  write (0,*) 'A, B, C = ', A, B, C
  ! stop
endif

! Calculate the time of minimum and uncertainty
T0 = -B / ( 2.0 * A )
T0_Uncertainty = Sqrt((4.0 * A * C - B * B) / (4.0 * A * A) / (Normal_Points / 4.0 - 1.0))
Time_Of_Minimum = Time_Normal(1) + ( ( Sum_Of_Squares_Smallest_Index - 1 ) + ( T0 - 2.0 ) ) * Time_Interval
Time_Of_Minimum_Uncertainty = T0_Uncertainty * Time_Interval
write (6,*) '             T0              Uncertainty     Time_Of_Minimum        Uncertainty'
write (6,99) T0, T0_Uncertainty, Time_Of_Minimum, Time_Of_Minimum_Uncertainty

! Diagnostic
if (.true.) then
  write (0,*) '             T0              Uncertainty     Time_Of_Minimum        Uncertainty'
  write (0,99) T0, T0_Uncertainty, Time_Of_Minimum, Time_Of_Minimum_Uncertainty
  stop
endif


! Finish
Return
End
