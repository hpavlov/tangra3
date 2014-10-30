program Kwee_van_Woerden_Main_Program

! Simulate the light curve of an occultation of a binary star
! Call the Kwee_van_Woerden subroutine to find the time of minimum light

! Every quantity must be explicitly declared
implicit none

! Parametric quantities
double precision           Time_Of_Minimum                         ! Time of minimum light
double precision           Duration_Of_Occultation                 ! Duration of the occultation
double precision           Start_Observation, Stop_Observation     ! Observation start and stop times
double precision           Start_Occultation, Stop_Occultation     ! Occultation start and stop times
double precision           Time_Interval                           ! Time interval between observations, 
double precision           Variable_Maximum_DNs                    ! DNs for variable star outside of occultation
double precision           Comparison_DNs                          ! DNs for comparison star
double precision           Sky_to_Star_Fraction                    ! Ratio of DNs for sky to star
double precision           Occultation_Depth                       ! Luminosity decrease at minimum light (normalized)
double precision           Noise_DNs                               ! Noise to be added to stars and sky backgrounds

! Values passed to subroutine
integer                    Number_Obs                              ! Number of observed data points
double precision           Time(10000)                             ! Mid-times of each observation
double precision           Variable_Star_DN(10000)                 ! Pixels values for the variable star brightness (including sky background)
double precision           Variable_Sky_DN(10000)                  ! Pixels values for the variable star sky background to be subtracted
double precision           Comparison_Star_DN(10000)               ! Pixels values for the comparison star brightness (including sky background)
double precision           Comparison_Sky_DN(10000)                ! Pixels values for the comparison star sky background to be subtracted

! Other quantities
integer                    i, j                                    ! Loop indices
double precision           Phase_Inside_Of_Occultation             ! 0 at mid-event, 1 at beginning or end of occultation
double precision           Random                                  ! Random number

! Open output file for diagnsotics
open (unit=6,file='Diagnostics.Txt',action='write')

! Initialize
Time_Of_Minimum = 0.61D6
Duration_Of_Occultation = 1.2D6
Start_Occultation = Time_Of_Minimum - 0.5 * Duration_Of_Occultation
Stop_Occultation  = Time_Of_Minimum + 0.5 * Duration_Of_Occultation
Start_Observation = 0.0D6
Stop_Observation  = 1.1D6
Time_Interval = 0.001D6
Number_Obs = ( Stop_Observation - Start_Observation ) / Time_Interval
Variable_Maximum_DNs = 10000.0
Comparison_DNs       =  5000.0
Noise_DNs            =  1000.0
Sky_to_Star_Fraction = 0.1
Occultation_Depth = 0.7

! Simulate the light curve
do i = 1, Number_Obs
  Time(i) = Start_Observation + ( i - 1 ) * Time_Interval

  if ( Time(i) < Start_Occultation .or. Time(i) > Stop_Occultation ) then
    ! Before or after occultation
    Variable_Star_DN(i)   = Variable_Maximum_DNs

  else 
    ! During occultation
    Phase_Inside_Of_Occultation = abs ( Time(i) - Time_Of_Minimum ) / ( Duration_Of_Occultation / 2.0 )
    Variable_Star_DN(i)   = Variable_Maximum_DNs * ( 1.0 - Occultation_Depth ) + Phase_Inside_Of_Occultation * Variable_Maximum_DNs * Occultation_Depth
    
  endif
  
  Variable_Sky_DN(i)    = Variable_Star_DN(i) * Sky_to_Star_Fraction
  Comparison_Star_DN(i) = Comparison_DNs 
  Comparison_Sky_DN(i)  = Comparison_Star_DN(i) * Sky_to_Star_Fraction

enddo

! Add noise
call Random_Seed

do i = 1, Number_Obs

  Call Random_Number(Random)
  Variable_Star_DN(i) = Variable_Star_DN(i) + 2.0 * Noise_DNs * ( 0.5 - Random )

  Call Random_Number(Random)
  Variable_Sky_DN(i) = Variable_Sky_DN(i) + 2.0 * Noise_DNs * ( 0.5 - Random )

  Call Random_Number(Random)
  Comparison_Star_DN(i) = Comparison_Star_DN(i) + 2.0 * Noise_DNs * ( 0.5 - Random )

  Call Random_Number(Random)
  Comparison_Sky_DN(i) = Comparison_Sky_DN(i) + 2.0 * Noise_DNs * ( 0.5 - Random )

enddo

! Diagnostic
if (.false.) then
  do i = 1, Number_Obs
    write (0,99) i, Time(i), Variable_Star_DN(i), Variable_Sky_DN(i), Comparison_Star_DN(i), Comparison_Sky_DN(i)
    write (6,99) i, Time(i), Variable_Star_DN(i), Variable_Sky_DN(i), Comparison_Star_DN(i), Comparison_Sky_DN(i)
    99 format (i6,f10.6,4f10.0)
  enddo
  stop
endif

! Call subroutine
Call Kwee_van_Woerden ( Number_Obs, Time, Variable_Star_DN, Variable_Sky_DN, Comparison_Star_DN, Comparison_Sky_DN )

! Finish
stop
end

include "Kwee_van_Woerden_Subroutine.f90"
