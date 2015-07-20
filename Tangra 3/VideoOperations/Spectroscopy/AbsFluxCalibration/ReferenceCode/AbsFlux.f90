program AbsFlux
 
! Calibrate spectral fluxes based on HST/STIS CALSPEC spectra

! Multi-standard mode:
! Input spectra for at least two standard stars observed over a range of airmasses
! Derive atmospheric extinction and system sensitivity calibrations based on the ratio of fluxes between observed standards and CALSPEC data
! Input spectra for one or more program stars
! Apply extinction and sensitivity corrections to program star(s)

! Simulation mode:
! Generate simulated spectra based on CALSPEC fluxes with external extinction and sensitivity corrections applied
! Derive the extinction and sensitivity functions from the standard stars
! Apply those functions as corrections to the program stars

! Allow for use Buil extinction model coefficients instead of deriving them empirically:
! Input spectrum for one standard star and one or more program stars observed at approximately the same airmass
! Apply extinction correction to standard and then derive sensitivity function
! Apply extinction and sensitivity corrections to program stars

! Possible enhancement:
! Estimate uncertainties for program stars
! (Can also use the RMS for the standard stars)

! Possible enhancement:
! Add a % difference output to be plotted for standard stars
! Low priority because the mean difference and RMS difference is already computed

! Wavelength units are Angstroms
! Flux units are ergs/cm-2/A/s

! More documentation is at the end of the file
 
implicit none           

! Parameters
include "AbsFlux_Parameters.f90"

! CALSPEC data
double precision        CALSPEC_Flux(Spectra_Max,CALSPEC_Pairs)          ! Array of one-per-Angstrom CALSPEC fluxes (#1 is star, #2 is 1-7001 for 3000-10000)
double precision        CALSPEC_Wave_Input(CALSPEC_Max_Data_Records)     ! Wavelength input for one CALSPEC star
double precision        CALSPEC_Flux_Input(CALSPEC_Max_Data_Records)     ! Flux input for one CALSPEC star
character*10            CALSPEC_Star(Spectra_Max)                        ! Names of the CALSPEC stars
character*100           CALSPEC_Master_Path_File_Name                    ! Path + file name for the CALSPEC master file
character*100           CALSPEC_Master_Record                            ! One record from the CALSPEC master file
integer                 CALSPEC_Data_Records(Spectra_Max)                ! Number of data records read from a CALSPEC file
integer                 CALSPEC_Index                                    ! Index for assigning to CALSPEC_Flux array (1-7001 for 3000-10000) and Buil extinction
double precision        CALSPEC_RA_J2000(Spectra_Max)                    ! Right ascensions for J2000 of the CALSPEC stars (radians)
double precision        CALSPEC_DC_J2000(Spectra_Max)                    ! Declinations for J2000 of the CALSPEC stars (radians)         
double precision        CALSPEC_RA_of_Date(Spectra_Max)                  ! Right ascensions of date for the CALSPEC stars (radians)
double precision        CALSPEC_DC_of_Date(Spectra_Max)                  ! Declinations of date for the CALSPEC stars (radians)
double precision        CALSPEC_Flux_Blurred                             ! Average CALSPEC flux over the half-width (used in deriving extinction function)

! CALSPEC data, namelist
character*100           CALSPEC_Path                                     ! Path to the CALSPEC files
integer                 CALSPEC_Stars                                    ! Number of CALSPEC stars

! Standard stars
double precision        Spectra_Standard_AM(Spectra_Max)                              ! Air mass
double precision        Spectra_Standard_AM_Min, Spectra_Standard_AM_Max              ! Air mass minimum and maximum
double precision        Spectra_Standard_AM_Delta                                     ! Difference betweeen minimum and maximum
double precision        Spectra_Standard_Wave(Spectra_Max,Spectra_Pairs)              ! Wavelength array, as observed
double precision        Spectra_Standard_Flux_Raw(Spectra_Max,Spectra_Pairs)          ! Raw flux array, as observed
double precision        Spectra_Standard_Flux_Raw_Ang(Spectra_Max,CALSPEC_Pairs)      ! Raw flux array, one-per-Angstrom
double precision        Spectra_Standard_Flux_Extn_Exp_Ang(Spectra_Max,CALSPEC_Pairs) ! Flux array corrected for extinction and expsosure, one-per-Angstrom
double precision        Spectra_Standard_Flux_Cal_Full(Spectra_Max,CALSPEC_Pairs)     ! Flux array corrected for extinction, exposure and sensitivity
integer                 Spectra_Standard_Records(Spectra_Max)                         ! Number of records read for this star

! Standard stars, namelist
character*100           Spectra_Standard_Path                            ! Path to standard star spectrum files
character*100           Spectra_Standard_Path_Eval                       ! Path to standard star spectrum files (output, evalution)
integer                 Spectra_Standard                                 ! Number of standard star spectra files
character*100           Spectra_Standard_File(Spectra_Max)               ! Names of standard star spectrum files
character*8             Spectra_Standard_Time(Spectra_Max)               ! Times corresponding to standard star spectrum files
double precision        Spectra_Standard_Exposure(Spectra_Max)           ! Exposure durations correspoinding to standard star spectrum files

! Program stars
double precision        Spectra_Program_AM(Spectra_Max)                      ! Air mass
double precision        Spectra_Program_Wave(Spectra_Max,Spectra_Pairs)      ! Wavelength array, as observed
double precision        Spectra_Program_Flux_Raw(Spectra_Max,Spectra_Pairs)  ! Raw flux array, as observed
double precision        Spectra_Program_Flux_Cal(Spectra_Max,Spectra_Pairs)  ! Calibrated flux array at observed wavelengths
integer                 Spectra_Program_Records(Spectra_Max)                 ! Number of records read for this star
double precision        Spectra_Program_RA_Input(Spectra_Max)                ! Right ascension input
double precision        Spectra_Program_DC_Input(Spectra_Max)                ! Declination input
double precision        Spectra_Program_RA_of_Date(Spectra_Max)              ! Right ascension precessed to date
double precision        Spectra_Program_DC_of_Date(Spectra_Max)              ! Declination precessed to date

! Program stars, namelist
character*100           Spectra_Program_Path                             ! Path to program star spectrum files
integer                 Spectra_Program                                  ! Number of program star spectra files
character*100           Spectra_Program_File(Spectra_Max)                ! Names of program star spectrum files
character*8             Spectra_Program_Time(Spectra_Max)                ! Times corresponding to program star spectrum files
double precision        Spectra_Program_Exposure(Spectra_Max)            ! Exposure durations correspoinding to program star spectrum files
logical                 Spectra_Program_J2000                            ! T = J2000 coordinates, F = coordinates of date
character*8             Spectra_Program_RA_String(Spectra_Max)           ! Character string of right ascension
character*9             Spectra_Program_DC_String(Spectra_Max)           ! Character string of declination

! Calibrated spectra
character*100           Calibrated_Path                                  ! Path to calibrated spectra

! Standard and program stars
integer                 CALSPEC_Number(Spectra_Max)                      ! Number of the CALSPEC star corresponding with the standard star (or a program star for simulation)

! Time and coordinates
integer                 RAhr, RAmn                                       ! Right ascension hours and minutes
double precision        RAsc, RA                                         ! Right ascension seconds and full right ascension in radians
integer                 DCdg, DCmn                                       ! Declination degrees and minutes
double precision        DCsc, DC                                         ! Declination seconds and full declination in radians
double precision        JD                                               ! Julian date
double precision        Lat, Lng                                         ! Observatory latitude and longitude in radians
double precision        ST0_GM                                           ! Siderial time at Greenwich meridian at JD
double precision        ST0_site                                         ! Siderial time at site coordinates at JD
double precision        DayFr                                            ! Fraction of a day

! Time and coordinates, namelist
integer                 Year, Month, Day                                 ! Year, month and day
character*9             Latitude, Longitude                              ! Observatory latitude and longitude strings

! Air mass, extinction function and sensitivity function
double precision        Extinction_Ang(CALSPEC_Pairs,2)                  ! Extinction function, one per Angstrom (1 = intercept, 2 = slope in magnitudes per air mass)
double precision        Extinction_Ang_Unc(CALSPEC_Pairs,2)              ! Uncertainties for Extinction_Ang
double precision        Extinction_Delta_Mag(Spectra_Max)                ! Delta magnitude values for this Angstrom
double precision        Extinction_AM(Spectra_Max)                       ! Air masses for this Angstrom
double precision        Extn_Ratio                                       ! Extinction luminosity ratio from coefficient and air mass
double precision        Refractive_Index                                 ! Buil's index of refraction for computing Rayleigh scattering component of extinction
double precision        Microns                                          ! Buil's wavelength in microns
double precision        Inverse_Lambda_Sq                                ! Buil's inverse of lambda squared
double precision        Rayleigh                                         ! Buil's Rayleigh component of extinction
double precision        Ozone                                            ! Buil's ozone component of extinction
double precision        Aerosol                                          ! Buil's aerosol component of extinction
double precision        Sensitivity_Ang(CALSPEC_Pairs)                   ! Sensitivity function, one per Angstrom
double precision        Sensitivity_Ang_Unc(CALSPEC_Pairs)               ! Sensitivity function, one per Angstrom
double precision        Sensitivity_Accum                                ! Accumulator for sensitivity function
double precision        Sensitivity_Ratio                                ! Ratio of standard star flux after correction for extinction and exposure duration over CALSPEC flux
logical                 Advisory_Flag                                    ! 'T' indicates that there is an advisory message to display
logical                 Advisory_Flag_Cumulative                         ! 'T' indicates that there was at least one advisory message in this program run
character*100           Advisory_String                                  ! Advisory message returned from subroutine

! Air mass and extinction, namelist input
double precision        Advisory_Air_Mass_Limit                          ! An air mass exceeding this value will generate an advisory message
double precision        Advisory_Air_Mass_Delta                          ! A range of standard star air masses less than this value will generate an advisory
logical                 Extinction_Model_Apply                           ! Apply Buil's model instead of measuring it
double precision        Extinction_Site_Altitude                         ! Altitude of observing site in meters for modeling extinction
double precision        Extinction_AOD                                   ! Aerosol optical depth: very dry, 0.02; dry desert, 0.05; typical winter, 0.07; typical summer, 0.21; hot and stormy 0.5

! Standard star calibration assessment statistics (re-used for program stars when simulating)
double precision        Calibrated_to_CALSPEC_Diff_Average(Spectra_Max)               ! Average normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff_RMS(Spectra_Max)                   ! Root-mean-square of normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff_Accum(Spectra_Max)                 ! Accumulator for normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff_Sq_Accum(Spectra_Max)              ! Accumulator for square of normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff(Spectra_Max,CALSPEC_Pairs)         ! Normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff_Sq(Spectra_Max,CALSPEC_Pairs)      ! Square of normalized calibrated-minus-CALSPEC flux differences
double precision        Calibrated_to_CALSPEC_Diff_Average_of_Averages                ! RMS of Calibrated_to_CALSPEC_Diff_Average
double precision        Calibrated_to_CALSPEC_Diff_RMS_of_RMSs                        ! RMS of Calibrated_to_CALSPEC_Diff_RMS
double precision        Calibrated_to_CALSPEC_Diff_Average_Blur(Spectra_Max)          ! Average normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_RMS_Blur(Spectra_Max)              ! Root-mean-square of normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_Accum_Blur(Spectra_Max)            ! Accumulator for normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_Sq_Accum_Blur(Spectra_Max)         ! Accumulator for square of normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_Blur(Spectra_Max,CALSPEC_Pairs)    ! Normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_Sq_Blur(Spectra_Max,CALSPEC_Pairs) ! Square of normalized calibrated-minus-CALSPEC flux differences (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur           ! RMS of Calibrated_to_CALSPEC_Diff_Average (for blurred CalSpec SED)
double precision        Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur                   ! RMS of Calibrated_to_CALSPEC_Diff_RMS (for blurred CalSpec SED)
double precision        RMS_Reduction_Factor                                          ! Square root of number of standard stars minus 2 (because there are 2 degrees of freedom: sens & extn)
double precision        Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Reduced                ! The 'Calibrated_to_CALSPEC ...' amount divided by the 'RMS_Reduction_Factor'
double precision        Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur_Reduced           ! The 'Calibrated_to_CALSPEC ...' amount divided by the 'RMS_Reduction_Factor'

! Paths and files
character*100           File                                             ! Name of a file
character*100           File_Header                                      ! Header record from a file
integer                 File_Length                                      ! Length of a file name
character*100           Path_File_Name                                   ! Path + file name for the file
integer                 Path_Length                                      ! Length of path

! Wavelengths and fluxes
double precision        Triad_Wave(3)                                    ! Three wavelengths used for LaGrangian interpolation
double precision        Triad_Flux(3)                                    ! Three fluxes used for LaGrangian interpolation
integer                 Input_Index_Counter                              ! Used for assigning input values to the triads
double precision        Frac_Wave                                        ! Fractional wavelength, used for linear interpolation
integer                 Half_Width                                       ! Half the resolution of the spectra in Angstroms, simulated and observed
integer                 Half_Width_Elements                              ! Number of elements in the half-width ( = 2 * half-width + 1 )
double precision        Half_Width_Del_Wave_Accum                        ! Accumulator of wave spans for half-width computation (observed)
integer                 Half_Width_Data_Points_Accum                     ! Accumulator of data points for half-width computation (observed)

! Simulation variables
double precision        Sim_Wave(Spectra_Pairs)                          ! Simulated wavelength
double precision        Sim_Flux(Spectra_Pairs)                          ! Simulated flux
integer                 Pair_Counter                                     ! Number of simulated wavelength/flux pairs
 
! Process control
double precision        Wave_Min_Proc, Wave_Max_Proc                     ! Minimum and maximum wavelengths to process
logical                 Sim                                              ! Simulation run
double precision        Sim_Angstrom_Spacing                             ! Spacing between wavelengths
logical                 Linear                                           ! Linear interpolation, rather than LaGrangian
logical                 Sim_Bias_Apply                                   ! Apply a bias to the flux of one or more simulated standard stars
double precision        Sim_Bias(Spectra_Max)                            ! Multiplicative factor to apply to simulated flux of a standard star
logical                 Resolution_Apply                                 ! Override the resolution from Angstroms per pixel with user supplied value
double precision        Resolution_Value                                 ! Spectral resolution which may override that from Angstroms per dispersion
logical                 Pavlov_Format                                    ! Special format for input file

! General
integer                 i, j, k, m                                       ! Loop indices
integer                 io_flag                                          ! Return value from IO operation
double precision        Rad2Deg                                          ! Radian to degrees converter

namelist / AbsFlux_Input / Year, Month, Day, Latitude, Longitude, Wave_Min_Proc, Wave_Max_Proc, CALSPEC_Path, CALSPEC_Stars, &
                           Spectra_Standard_Path, Spectra_Standard, Spectra_Standard_File, Spectra_Standard_Time, Spectra_Standard_Exposure, &
                           Spectra_Program_Path,  Spectra_Program,  Spectra_Program_File,  Spectra_Program_Time,  Spectra_Program_Exposure,  &
                           Spectra_Program_J2000, Spectra_Program_RA_String, Spectra_Program_DC_String, &
                           Calibrated_Path, Sim, Sim_Angstrom_Spacing, Linear, Sim_Bias_Apply, Sim_Bias, Advisory_Air_Mass_Limit, &
                           Advisory_Air_Mass_Delta, Spectra_Standard_Path_Eval, Resolution_Apply, Resolution_Value, &
                           Extinction_Model_Apply, Extinction_Site_Altitude, Extinction_AOD, Pavlov_Format

! ***************************************************************
! ********************** Part 1: Initialize *********************
! ***************************************************************

! Derive the value for radian to degree conversion
Rad2Deg = 180. / Pi
 
! Set the advisory flag to false
Advisory_Flag_Cumulative = .false.

! Initialize logical namelist variables
Extinction_Model_Apply = .false.
Sim = .false.
Linear = .true.
Sim_Bias_Apply = .false.
Resolution_Apply = .false.
Pavlov_Format = .false.

! Open namelist file
open (1,file='AbsFlux_Input.txt',action='read',iostat=io_flag)
if ( io_flag .ne. 0 ) then
  write (0,*) 'Namelist file iostat = ', io_flag
  stop
endif

! Open summary output file
open (6,file='AbsFlux_Output.txt',action='write',iostat=io_flag)
if ( io_flag .ne. 0 ) then
  write (0,*) 'Summary output file iostat = ', io_flag
  stop
endif

! Open the file for outputting extinction function
open (7,file='Extinction_Function.txt',action='write',iostat=io_flag)
if ( io_flag .ne. 0 ) then
  write (0,*) 'Extinction output file iostat = ', io_flag
  stop
endif

! Open the file for outputting sensitivity function
open (8,file='Sensitivity_Function.txt',action='write',iostat=io_flag)
if ( io_flag .ne. 0 ) then
  write (0,*) 'Sensitivity output file iostat = ', io_flag
  stop
endif

! Read namelist data
read (1,AbsFlux_Input)

! Convert latitude and longitude strings to radians
call LatLong (Latitude, Longitude, Lat, Lng, Rad2Deg)

! Compute JD at 0h UT
call CalcJD(Year, Month, Day, JD)

! Compute siderial time at Greenwich merdian at JD
ST0_GM = mod(18.697374558D0 + 24.06570982441908D0 * (JD - 2451545.0), 24.0D0) / 24.D0 * 2.0D0 * Pi

! Compute siderial time at site coordinates
ST0_site = mod (ST0_GM + Lng, 2.0 * Pi)

! Assign flux bias factors for simulation
if ( Sim_Bias_Apply ) then
  do i = 1, Spectra_Standard
    if ( Sim_Bias(i) == 0.0 ) Sim_Bias(i) = 1.0
  enddo
else
  do i = 1, Spectra_Standard
    Sim_Bias(i) = 1.0
  enddo
endif

! Compute Buil extinction coefficients if needed
if ( Extinction_Model_Apply ) then
  do i = CALSPEC_Wave_Min, CALSPEC_Wave_Max

    ! Determine the index for the CALSPEC_Flux array
    CALSPEC_Index = i - CALSPEC_Wave_Min + 1
    
    ! Wavelength units
    Microns = i / 10000.
    Inverse_Lambda_Sq = 1. / Microns**2

    ! Rayleigh scattering component
    Refractive_Index = 0.23465 + 107.6 / ( 146 - Inverse_Lambda_Sq ) + 0.93161 / ( 41 - Inverse_Lambda_Sq )
    Rayleigh = 0.0094977 * Inverse_Lambda_Sq * Inverse_Lambda_Sq * Refractive_Index * exp( -Extinction_Site_Altitude / 7996 )
    
    ! Ozone component
    Ozone = -2.5 * log10 ( exp ( -0.0168 * exp ( -15 * abs ( Microns - 0.59 ) ) ) )
    
    ! Aerosol component
    Aerosol = 2.5 * log10 ( exp ( Extinction_AOD * ( Microns / 0.55 ) ** -1.3 ) )

    ! Sum the three components and assign to extinction coefficient
    Extinction_Ang(CALSPEC_Index,2) = Rayleigh + Ozone + Aerosol
    
    
    ! Fill the remaining extinction array elements (coefficient uncertainty, intercept and intercept uncertainty)
    Extinction_Ang_Unc(CALSPEC_Index,2) = Extinction_Ang(CALSPEC_Index,2) * 0.2
    Extinction_Ang(CALSPEC_Index,1)     = 0.0
    Extinction_Ang_Unc(CALSPEC_Index,1) = 0.0

    ! Diagnostic
    if ( .false. ) then
      write (0,*) 'Angstroms          ', i
      write (0,*) 'Microns            ', Microns
      write (0,*) 'Inverse_Lambda_Sq  ', Inverse_Lambda_Sq
      write (0,*) 'Refractive_Index   ', Refractive_Index
      write (0,*) 'Rayleigh           ', Rayleigh
      write (0,*) 'Ozone              ', Ozone
      write (0,*) 'Aerosol            ', Aerosol
      write (0,*) 'Total extinction   ', Extinction_Ang(CALSPEC_Index,1)
      stop
    endif
    
    ! Diagnostic
    if ( .false. ) then
      if ( int ( i / 1000 ) * 1000 == i ) write (0,*) i, Extinction_Ang(CALSPEC_Index,2)
      if ( i == 10000 ) stop
    endif
    
    ! Diagnostic
    if ( .false. ) then
      write (7,*) i, Extinction_Ang(CALSPEC_Index,2)
      if ( i == 10000 ) stop
    endif
    
  enddo

endif

! Display
write (0,199)
write (6,199)
199 format (/,'AbsFlux Program',/)
write (0,700)
write (6,700)
700 format ('Namelist and derived input:')
write (0,702) Year, Month, Day
write (6,702) Year, Month, Day
702 format ('Year, Month, Day            ', 2x,i4,2x,i2,2x,i2)
write (0,704) Latitude
write (6,704) Latitude
704 format ('Latitude string             ', a11)
write (0,706) Longitude
write (6,706) Longitude
706 format ('Longitude string            ', a11)
write (0,708) Lat
write (6,708) Lat
708 format ('Latitude (radians)          ', f10.6)
write (0,710) Lng
write (6,710) Lng
710 format ('Longitude (radians)         ', f10.6)
write (0,712) ST0_GM 
write (6,712) ST0_GM 
712 format ('ST0 GM  (radians)           ', f10.6)
write (0,714) ST0_site
write (6,714) ST0_site
714 format ('ST0 site (radians)          ', f10.6)
write (0,716) Wave_Min_Proc
write (6,716) Wave_Min_Proc
716 format ('Wave Min Proc (Angstroms)   ', f10.1)
write (0,718) Wave_Max_Proc
write (6,718) Wave_Max_Proc
718 format ('Wave Max Proc (Angstroms)   ', f10.1)
write (0,720) Sim
write (6,720) Sim
720 format ('Sim                         ', L10)
write (0,722) Sim_Angstrom_Spacing
write (6,722) Sim_Angstrom_Spacing
722 format ('Sim Angstrom Spacing        ', f10.3)
write (0,724) Sim_Bias_Apply
write (6,724) Sim_Bias_Apply
724 format ('Sim Bias Apply              ', L10)
write (0,726) CALSPEC_Path
write (6,726) CALSPEC_Path
726 format ('CALSPEC Path                ', a50)
write (0,728) CALSPEC_Stars
write (6,728) CALSPEC_Stars
728 format ('CALSPEC Stars               ', i4)
write (0,744) Spectra_Standard_Path
write (6,744) Spectra_Standard_Path
744 format ('Spectra Standard_Path       ', a50)
write (0,746) Spectra_Standard
write (6,746) Spectra_Standard
746 format ('Spectra Standard            ', i4)
do i = 1, Spectra_Standard
  write (0,748) i, Spectra_Standard_File(i)
  write (6,748) i, Spectra_Standard_File(i)
  748 format ('i, Spectra Standard File(i)     ', i4,2x,a40)
  write (0,750) i, Spectra_Standard_Time(i)
  write (6,750) i, Spectra_Standard_Time(i)
  750 format ('i, Spectra Standard Time(i)     ', i4,2x,a10)
  write (0,752) i, Spectra_Standard_Exposure(i)
  write (6,752) i, Spectra_Standard_Exposure(i)
  752 format ('i, Spectra Standard Exp.(i)     ', i4,2x,f10.3)
  if (Sim) write (0,753) i, Sim_Bias(i)
  if (Sim) write (6,753) i, Sim_Bias(i)
  753 format ('i, Sim Bias(i)                  ', i4,2x,f10.3)
enddo
write (0,730) Spectra_Program_Path
write (6,730) Spectra_Program_Path
730 format ('Spectra Program_Path        ', a50)
write (0,732) Spectra_Program
write (6,732) Spectra_Program
732 format ('Spectra Program             ', i4)
do i = 1, Spectra_Program
  write (0,734) i, Spectra_Program_File(i)
  write (6,734) i, Spectra_Program_File(i)
  734 format ('i, Spectra Program File(i)      ', i4,2x,a40)
  write (0,736) i, Spectra_Program_Time(i)
  write (6,736) i, Spectra_Program_Time(i)
  736 format ('i, Spectra Program Time(i)      ', i4,2x,a10)
  write (0,738) i, Spectra_Program_Exposure(i)
  write (6,738) i, Spectra_Program_Exposure(i)
  738 format ('i, Spectra Program Exp.(i)      ', i4,2x,f10.3)
  write (0,740) i, Spectra_Program_RA_String(i)
  write (6,740) i, Spectra_Program_RA_String(i)
  740 format ('i, Spectra Program RA String(i) ', i4,2x,a10)
  write (0,742) i, Spectra_Program_DC_String(i)
  write (6,742) i, Spectra_Program_DC_String(i)
  742 format ('i, Spectra Program DC String(i) ', i4,2x,a10)
enddo
write (0,754) Calibrated_Path
write (6,754) Calibrated_Path
754 format ('Calibrated_Path             ', a50)
write (0,756) Spectra_Standard_Path_Eval
write (6,756) Spectra_Standard_Path_Eval
756 format ('Spectra Standard Path Eval  ', a50)
write (0,758) Linear
write (6,758) Linear
758 format ('Linear                      ', L10)
write (0,770) Resolution_Apply
write (6,770) Resolution_Apply
770 format ('Resolution Apply            ', L10)
write (0,772) Resolution_Value
write (6,772) Resolution_Value
772 format ('Resolution Value (Angstroms)', f10.3)
write (0,760) Advisory_Air_Mass_Limit
write (6,760) Advisory_Air_Mass_Limit
760 format ('Advisory Air Mass Limit     ', f10.3)
write (0,762) Advisory_Air_Mass_Delta
write (6,762) Advisory_Air_Mass_Delta
762 format ('Advisory Air Mass Delta     ', f10.3)
write (0,764) Extinction_Model_Apply
write (6,764) Extinction_Model_Apply
764 format ('Extinction Model Apply      ', L10)
if ( Extinction_Model_Apply ) then
  write (0,766) Extinction_Site_Altitude
  write (6,766) Extinction_Site_Altitude
  766 format ('Extinction Site Altitude (m)', f10.3)
  write (0,768) Extinction_AOD
  write (6,768) Extinction_AOD
  768 format ('Extinction AOD              ', f10.3)
  do i = CALSPEC_Wave_Min, CALSPEC_Wave_Max
    if ( int ( i / 1000 ) * 1000 == i ) then
      CALSPEC_Index = i - CALSPEC_Wave_Min + 1
      write (0,769) i, Extinction_Ang(CALSPEC_Index,1)
      write (6,769) i, Extinction_Ang(CALSPEC_Index,1)
      769 format ('Extinction coefficient      ', i5,2xf10.3)
    endif
  enddo
endif
write (0,774) Pavlov_Format
write (6,774) Pavlov_Format
774 format ('Pavlov_Format               ', L10)

! *****************************************************************************
! ********** Part 2: Input Wavelengths and Fluxes from CALSPEC files **********
! *****************************************************************************

! Display
write (0,1215)
write (6,1215)
1215 format (/,'CALSPEC Stars:')

! Build the path+file name of the CALSPEC master file
Path_Length = index(CALSPEC_Path,' ')
CALSPEC_Master_Path_File_Name(1:Path_Length) = CALSPEC_Path(1:Path_Length)
CALSPEC_Master_Path_File_Name(Path_Length:Path_Length+CALSPEC_Master_File_Length-1) = CALSPEC_Master_File(1:CALSPEC_Master_File_Length)

! Diagnostic
if ( .false. ) then
  write (0,*) 'CALSPEC_Path                  ', CALSPEC_Path
  write (0,*) 'Path_Length                   ', Path_Length
  write (0,*) 'CALSPEC_Master_File           ', CALSPEC_Master_File
  write (0,*) 'CALSPEC_Master_File_Length    ', CALSPEC_Master_File_Length
  write (0,*) 'CALSPEC_Master_Path_File_Name ', CALSPEC_Master_Path_File_Name
  stop
endif

! Open the CALSPEC master file
open (2,file=CALSPEC_Master_Path_File_Name,action='read',iostat=io_flag)
if ( io_flag .ne. 0 ) then
  write (0,*) 'CalSpec master file iostat = ', io_flag
  stop
endif

! Read two header lines from the CALSPEC master file
read (2,'(a100)') CALSPEC_Master_Record
read (2,'(a100)') CALSPEC_Master_Record

! Input data from the CALSPEC master file
do i = 1, CALSPEC_Stars
  
  ! Read a record
  read (2,'(a100)',iostat=io_flag) CALSPEC_Master_Record
  
  ! Assign the star name
  CALSPEC_Star(i) = CALSPEC_Master_Record(1:10)

  ! Diagnostic
  if ( .true. ) then
    write (0,1222) i, CALSPEC_Star(i)
    write (6,1222) i, CALSPEC_Star(i)
    1222 format (i5,2x,a)
  endif
  
  ! Parse and compute the RA
  read (CALSPEC_Master_Record(15:16),*) RAhr
  read (CALSPEC_Master_Record(18:19),*) RAmn
  read (CALSPEC_Master_Record(21:26),*) RAsc
  RA = ( RAhr + RAmn / 60. + RAsc / 3600. ) * 15.0 / Rad2Deg
  CALSPEC_RA_J2000(i) = RA
    
  ! Parse and compute the Dec
  read (CALSPEC_Master_Record(29:30),*) DCdg
  read (CALSPEC_Master_Record(32:33),*) DCmn
  read (CALSPEC_Master_Record(35:39),*) DCsc
  DC = ( DCdg + DCmn / 60. + DCsc / 3600. ) / Rad2Deg
  if ( CALSPEC_Master_Record(28:28) == "-" ) DC = -DC
  CALSPEC_DC_J2000(i) = DC
  
  ! Precess the coordinates
  Call Precess ( JD, CALSPEC_RA_J2000(i), CALSPEC_DC_J2000(i), CALSPEC_RA_of_Date(i), CALSPEC_DC_of_Date(i), Pi )

  ! Build the path+file name of the CALSPEC star file
  File = CALSPEC_Master_Record(65:100)
  call Build_Path_File_Name ( CALSPEC_Path, File, Path_File_Name, Blank100 )
  
  ! Open the CALSPEC star file
  open (3,file=Path_File_Name,action='read',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'CalSpec star file iostat = ', io_flag
    stop
  endif

  ! Diagnostic
  if ( .false. ) then
    write (0,*) 'CALSPEC_Master_Record   = ', CALSPEC_Master_Record
    write (0,*) 'i, CALSPEC_Star(i)      = ', i, CALSPEC_Star(i)
    write (0,*) 'RAhr, RAmn, RAsc        = ', RAhr, RAmn, RAsc
    write (0,*) 'RA                      = ', RA
    write (0,*) 'CALSPEC_RA_J2000(i)     = ', CALSPEC_RA_J2000(i)
    write (0,*) 'CALSPEC_RA_of_date(i)   = ', CALSPEC_RA_of_Date(i)
    write (0,*) 'DCdg, DCmn, DCsc, DC    = ', DCdg, DCmn, DCsc, DC
    write (0,*) 'DC                      = ', DC
    write (0,*) 'CALSPEC_DC_J2000(i)     = ', CALSPEC_DC_J2000(i)
    write (0,*) 'CALSPEC_DC_of_Date(i)   = ', CALSPEC_DC_of_Date(i)
    write (0,*) 'File                    = ', File
    write (0,*) 'Path_File_Name          = ', Path_File_Name
    write (0,*) 'io_flag                 = ', io_flag
    if (i == 10) then
      ! Testing precession of HD 74000
      write (0,*) 
      write (0,*) 'RA precessed (degrees) = ', CALSPEC_RA_of_Date(i) * 180.  / Pi
      write (0,*) 'DC precessed (degrees) = ', CALSPEC_DC_of_Date(i) * 180. / Pi  
      stop
    endif
    stop
  endif
  
  ! Read CALSPEC star header lines
  do j = 1, 3
    read (3,*) File_Header
  enddo
  
  ! Read CALSPEC star wavelengths and fluxes
  do k = 1, CALSPEC_Max_Data_Records
    read (3,*,iostat=io_flag) CALSPEC_Wave_Input(k), CALSPEC_Flux_Input(k)
    if ( io_flag .ne. 0 ) goto 100
    CALSPEC_Data_Records(i) = k
  enddo  
  100 continue
  
  ! Find the first wavelength which equals or exceeds the CALSPEC minimum
  do k = 1, CALSPEC_Data_Records(i)
    if ( CALSPEC_Wave_Input(k) >= CALSPEC_Wave_Min ) then
      Input_Index_Counter = k
      goto 200
    endif
  enddo
  200 continue
  
  ! Assign the first triad of wavelengths and fluxes
  Triad_Wave(1) = CALSPEC_Wave_Input(Input_Index_Counter-1)
  Triad_Wave(2) = CALSPEC_Wave_Input(Input_Index_Counter  )
  Triad_Wave(3) = CALSPEC_Wave_Input(Input_Index_Counter+1)
  Triad_Flux(1) = CALSPEC_Flux_Input(Input_Index_Counter-1)
  Triad_Flux(2) = CALSPEC_Flux_Input(Input_Index_Counter  )
  Triad_Flux(3) = CALSPEC_Flux_Input(Input_Index_Counter+1)

  ! Loop over the range of wavelengths
  do m = CALSPEC_Wave_Min, CALSPEC_Wave_Max
  
    ! Determine the index for the CALSPEC_Flux array
    CALSPEC_Index = m - CALSPEC_Wave_Min + 1
    
    ! Update the triad if the wavelength is beyond the third element
    if ( m > Triad_Wave(3) ) then
      Triad_Wave(1)       = Triad_Wave(2)
      Triad_Wave(2)       = Triad_Wave(3)
      Triad_Flux(1)       = Triad_Flux(2)
      Triad_Flux(2)       = Triad_Flux(3)
      Input_Index_Counter = Input_Index_Counter + 1
      Triad_Wave(3)       = CALSPEC_Wave_Input(Input_Index_Counter+1)
      Triad_Flux(3)       = CALSPEC_Flux_Input(Input_Index_Counter+1)
    endif

    ! Interpolate
    if ( .not. Linear ) then
      call LaGrange ( m*1.0D0, CALSPEC_Flux(i,CALSPEC_Index), Triad_Wave, Triad_Flux )
    else
      Frac_Wave = ( m*1.0D0 - Triad_Wave(2) ) / ( Triad_Wave(3) - Triad_Wave(2) )
      CALSPEC_Flux(i,CALSPEC_Index) = Triad_Flux(2) + Frac_Wave * ( Triad_Flux(3) - Triad_Flux(2) )
    endif

    ! Diagnostic
    if ( .false. ) then
      write (0,*)
      write (0,*) 'Input_Index_Counter         = ', Input_Index_Counter
      write (0,*) 'Triad_Wave                  = ', Triad_Wave
      write (0,*) 'Triad_Flux                  = ', Triad_Flux
      write (0,*) 'Target wavelength           = ', float(m)
      write (0,*) 'i                           = ', i
      write (0,*) 'CALSPEC_Index               = ', CALSPEC_Index
      write (0,*) 'CALSPEC_Flux                = ', CALSPEC_Flux(i,CALSPEC_Index)
      stop
    endif
    
  enddo   ! End of 'Interpolate CALSPEC_Flux values based on triads'

enddo   ! End of 'Input data from the CALSPEC master file'

! Skip if this is not a simulation run
if ( .not. Sim ) goto 2412

! *****************************************************************************
! ********************** Part 3: Simulate Standard Stars **********************
! *****************************************************************************

! Diagnostic
write (0,1213)
write (6,1213)
1213 format (/,'Simulation of standard stars:')

! Assign the half-width (also applies to simulation of program stars)
Half_Width = nint ( Sim_Angstrom_Spacing / 2.0 )

! Override the half-width from Angstroms per pixels with user supplied value
if ( Resolution_Apply ) Half_Width = ( Resolution_Value / 2.0 )
 
! Loop over standard star files
do i = 1, Spectra_Standard

  ! Display
  write (0,1211) i, Spectra_Standard_File(i)
  write (6,1211) i, Spectra_Standard_File(i)
  1211 format (i5,2x,a50)

  ! Build the path+file name of the standard star file
  call Build_Path_File_Name ( Spectra_Standard_Path, Spectra_Standard_File(i), Path_File_Name, Blank100 )
  
  ! Open the standard star file
  open (4,file=Path_File_Name,action='write',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'Standard star file (simulation) iostat = ', io_flag
    stop
  endif

  ! Identify the corresponding CALSPEC star
  call ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, Spectra_Standard_File(i), CALSPEC_Number(i), .false.)

  ! Compute the air mass
  Call AirMass (Lat, Lng, ST0_site, Spectra_Standard_Time(i), CALSPEC_RA_of_Date(CALSPEC_Number(i)), CALSPEC_DC_of_Date(CALSPEC_Number(i)), &
                Pi, Spectra_Standard_AM(i), Advisory_Air_Mass_Limit, Advisory_Flag, Advisory_String, Advisory_Flag_Cumulative)
  if ( Advisory_Flag ) then
    write (0,2321) Advisory_String
    write (6,2321) Advisory_String
    2321 format (a65)
  endif
  write (0,1232) Spectra_Standard_AM(i)
  write (6,1232) Spectra_Standard_AM(i)
  1232 format ('Air Mass = ', f7.2)

  ! Simulate fluxes, including senstivity and extinction effects, and output the result
  call Simulate (i, CALSPEC_Flux, Spectra_Standard_Exposure(i), Spectra_Standard_AM(i), CALSPEC_Number(i), Wave_Min_Proc, Wave_Max_Proc, &
                 Sim_Angstrom_Spacing, Rad2Deg, Linear, Half_Width, Sim_Wave, Sim_Flux, Pair_Counter)
  
  ! Loop over pairs
  do j = 1, Pair_Counter
  
    ! Apply the bias factor
    Sim_Flux(j) = Sim_Flux(j) * Sim_Bias(i)
    
    ! Output the pair
    write (4,*) Sim_Wave(j), Sim_Flux(j)
    ! write (0,*) j, Sim_Wave(j), Sim_Flux(j)
    
  enddo

  ! Close the standard star file
  close (4)

enddo   ! End of loop over standard star files

! *****************************************************************************
! ********************** Part 4: Simulate Program Stars ***********************
! *****************************************************************************

! Diagnostic
write (0,1219)
write (6,1219)
1219 format (/,'Simulation of program stars:')

! Loop over program star files
do i = 1, Spectra_Program

  ! Dianostic
  write (0,1311) i, Spectra_Program_File(i)
  write (6,1311) i, Spectra_Program_File(i)
  1311 format (i5,2x,a50)

  ! Build the path+file name of the program star file
  call Build_Path_File_Name ( Spectra_Program_Path, Spectra_Program_File(i), Path_File_Name, Blank100 )

  ! Open the program star file
  open (5,file=Path_File_Name,action='write',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'Program star file (simulation) iostat = ', io_flag
    stop
  endif

  ! Identify the corresponding CALSPEC star
  call ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, Spectra_Program_File(i), CALSPEC_Number(i), .false.)

  ! Compute the air mass
  Call AirMass (Lat, Lng, ST0_site, Spectra_Program_Time(i), CALSPEC_RA_of_Date(CALSPEC_Number(i)), CALSPEC_DC_of_Date(CALSPEC_Number(i)), &
                Pi, Spectra_Program_AM(i), Advisory_Air_Mass_Limit, Advisory_Flag, Advisory_String, Advisory_Flag_Cumulative)
  if ( Advisory_Flag ) then
    write (0,2321) Advisory_String
    write (6,2321) Advisory_String
  endif
  write (0,1232) Spectra_Program_AM(i)
  write (6,1232) Spectra_Program_AM(i)

  ! Simulate fluxes, including senstivity and extinction effects, and output the result
  call Simulate (i, CALSPEC_Flux, Spectra_Program_Exposure(i), Spectra_Program_AM(i), CALSPEC_Number(i), Wave_Min_Proc, Wave_Max_Proc, &
                 Sim_Angstrom_Spacing, Rad2Deg, Linear, Half_Width, Sim_Wave, Sim_Flux, Pair_Counter)

  ! Loop over pairs
  do j = 1, Pair_Counter
  
    ! Output the pair
    write (4,*) Sim_Wave(j), Sim_Flux(j)
    
  enddo

  ! Close the program star file
  close (5)
  
enddo   ! End of loop over program star files

2412 continue   ! Goto label for skipping simulation

! *****************************************************************************
! ** Part 5: Derive Extinction and Sensitivity Functions from Standard Stars **
! *****************************************************************************

! Display
write (0,841)
write (6,841)
841 format (/,'Derivation of extinction and sensitivity functions:')

! Initialize the half-width accumulator variables
Half_Width_Del_Wave_Accum    = 0.0
Half_Width_Data_Points_Accum = 0

! Loop over standard star files
do i = 1, Spectra_Standard

  ! Display
  write (0,1211) i, Spectra_Standard_File(i)
  write (6,1211) i, Spectra_Standard_File(i)

  ! Build the path+file name of the standard star file
  call Build_Path_File_Name ( Spectra_Standard_Path, Spectra_Standard_File(i), Path_File_Name, Blank100 )

  ! Open the standard star file
  open (4,file=Path_File_Name,action='read',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) Path_File_Name
    write (0,*) 'Standard star file (input) iostat = ', io_flag
    stop
  endif

  ! Identify the corresponding CALSPEC star
  call ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, Spectra_Standard_File(i), CALSPEC_Number(i), Pavlov_Format)
   
  ! Compute the air mass
  Call AirMass (Lat, Lng, ST0_site, Spectra_Standard_Time(i), CALSPEC_RA_of_Date(CALSPEC_Number(i)), CALSPEC_DC_of_Date(CALSPEC_Number(i)), &
       Pi, Spectra_Standard_AM(i), Advisory_Air_Mass_Limit, Advisory_Flag, Advisory_String, Advisory_Flag_Cumulative)
  if ( Advisory_Flag ) then
    write (0,2321) Advisory_String
    write (6,2321) Advisory_String
  endif
  write (0,1232) Spectra_Standard_AM(i)
  write (6,1232) Spectra_Standard_AM(i)
  ! write (0,843) Spectra_Standard_Exposure(i)
  ! write (6,843) Spectra_Standard_Exposure(i)
  ! 843 format ('Exposure = ', f7.3)

  ! Input the raw spectrum
  Call Input_Raw_Spectrum ( i, Spectra_Standard_Wave, Spectra_Standard_Flux_Raw, Wave_Min_Proc, Wave_Max_Proc, Spectra_Standard_Records, Pavlov_Format )

  ! Increment the half-width accumulators
  j = Spectra_Standard_Records(i)
  Half_Width_Del_Wave_Accum    = Half_Width_Del_Wave_Accum    + ( Spectra_Standard_Wave(i,j-1) - Spectra_Standard_Wave(i,1) )
  Half_Width_Data_Points_Accum = Half_Width_Data_Points_Accum +   Spectra_Standard_Records(i)
  
  ! Diagnostic
  if ( .false. ) then
    write (0,*) 'i                            = ', i
    write (0,*) 'j                            = ', j
    write (0,*) 'Spectra_Standard_Wave(i,j-1) = ', Spectra_Standard_Wave(i,j-1)
    write (0,*) 'Spectra_Standard_Wave(i,1)   = ', Spectra_Standard_Wave(i,1)
    write (0,*) 'Half_Width_Del_Wave_Accum    = ', Half_Width_Del_Wave_Accum
    write (0,*) 'Half_Width_Data_Points_Accum = ', Half_Width_Data_Points_Accum
    ! stop
  endif
  
  ! Close the standard star file
  close (4)
  
  ! Interpolate to one-per-Angstrom array of wavelengths
  
  ! Find the first wavelength which equals or exceeds the user-specified minimum
  do k = 1, Spectra_Standard_Records(i)
    if ( Spectra_Standard_Wave(i,k) >= Wave_Min_Proc ) then
      Input_Index_Counter = k
      ! write (0,*) i, Spectra_Standard_Wave(i,k), Wave_Min_Proc
      ! stop
      goto 950
    endif
  enddo
  950 continue
  
  ! Counter must be at least two to avoid exceeding array lower bound
  if ( Input_Index_Counter == 1 ) Input_Index_Counter = 2

  ! Counter must be at least one smaller than size of array to avoid exceeding upper bound
  if ( Input_Index_Counter == Spectra_Standard_Records(i) ) Input_Index_Counter = Spectra_Standard_Records(i) - 1

  ! Assign the first triad of wavelengths and fluxes
  Triad_Wave(1) = Spectra_Standard_Wave(i,Input_Index_Counter-1)
  Triad_Wave(2) = Spectra_Standard_Wave(i,Input_Index_Counter  )
  Triad_Wave(3) = Spectra_Standard_Wave(i,Input_Index_Counter+1)
  Triad_Flux(1) = Spectra_Standard_Flux_Raw(i,Input_Index_Counter-1)
  Triad_Flux(2) = Spectra_Standard_Flux_Raw(i,Input_Index_Counter  )
  Triad_Flux(3) = Spectra_Standard_Flux_Raw(i,Input_Index_Counter+1)

  ! Loop over the range of wavelengths
  do m = Wave_Min_Proc, Wave_Max_Proc
  
    ! Determine the index for the CALSPEC_Flux array
    CALSPEC_Index = m - CALSPEC_Wave_Min + 1
    
    ! Update the triad if the wavelength is beyond the third element
    if ( m > Triad_Wave(3) ) then
    
      Triad_Wave(1)       = Triad_Wave(2)
      Triad_Wave(2)       = Triad_Wave(3)
      Triad_Flux(1)       = Triad_Flux(2)
      Triad_Flux(2)       = Triad_Flux(3)
      Input_Index_Counter = Input_Index_Counter + 1
      Triad_Wave(3)       = Spectra_Standard_Wave(i,Input_Index_Counter+1)
      Triad_Flux(3)       = Spectra_Standard_Flux_Raw(i,Input_Index_Counter+1)
      
      ! Diagnostic
      if ( .false. ) then
        write (0,*)
        write (0,*) 'Updated Triads:'
        write (0,*) Input_Index_Counter
        write (0,*) Triad_Wave
        write (0,*) Triad_Flux
        write (0,*)
        ! stop
      endif
      
    endif

    ! Interpolate
    if ( .not. Linear ) then
      call LaGrange ( m*1.0D0, Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index), Triad_Wave, Triad_Flux )
    else
      Frac_Wave = ( m*1.0D0 - Triad_Wave(2) ) / ( Triad_Wave(3) - Triad_Wave(2) )
      Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index) = Triad_Flux(2) + Frac_Wave * ( Triad_Flux(3) - Triad_Flux(2) )
    endif

    ! Diagnostic
    if ( .false. ) then
      write (0,*)
      write (0,*) 'Input_Index_Counter         = ', Input_Index_Counter
      write (0,*) 'Triad_Wave                  = ', Triad_Wave
      write (0,*) 'Triad_Flux                  = ', Triad_Flux
      write (0,*) 'Target wavelength           = ', float(m)
      write (0,*) 'i                           = ', i
      write (0,*) 'CALSPEC_Index               = ', CALSPEC_Index
      write (0,*) 'Standard Star Flux          = ', Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index)
      stop
    endif
    
  enddo   ! End of 'Loop over the range of wavelengths'

enddo   ! End of loop over standard star files

! Compute the half-width for blurring the CALSPEC flux, default is to use the dispersion in Angstroms per pixel
Half_Width = nint ( Half_Width_Del_Wave_Accum  / Half_Width_Data_Points_Accum / 2.0 )

! Override the half-width from Angstroms per pixels with user supplied value
if ( Resolution_Apply ) Half_Width = ( Resolution_Value / 2.0 )

! Calculate the number of half-width elements
Half_Width_Elements = Half_Width * 2 + 1

! Diagnostic
if ( .false. ) then
  write (0,*) 'Half-Width          = ', Half_Width
  write (0,*) 'Half-Width_Elements = ', Half_Width_Elements
  stop
endif

! Derive the extinction coefficients, one per Angstrom

! Loop over the range of wavelengths
do m = Wave_Min_Proc, Wave_Max_Proc

  ! Assign the CALSPEC index (1-7001 for 3000-10000)
  CALSPEC_Index = m - CALSPEC_Wave_Min + 1
  
  ! Loop over the stars
  do i = 1, Spectra_Standard

    ! Blur the CALSPEC fluxes over the half-width
    CALSPEC_Flux_Blurred = 0.0
    do j = -Half_Width, Half_Width
      CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred + CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index+j)
    enddo
    
    ! Normalize the blured CALSPEC flux
    CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred / Half_Width_Elements

    ! Derive the delta magnitudes (apply exposure duration)
    Extinction_Delta_Mag(i) = -2.5 * log10 &
                              ( ( Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index) / Spectra_Standard_Exposure(i) ) &
                                / CALSPEC_Flux_Blurred )
    
    ! Diagnostic
    if ( .false. ) then
      write (0,*)
      write (0,*) 'm                                              = ', m
      write (0,*) 'CALSPEC_Index                                  = ', CALSPEC_Index
      write (0,*) 'i                                              = ', i
      write (0,*) 'CALSPEC_Number(i)                              = ', CALSPEC_Number(i)
      write (0,*) 'Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index) = ', Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index)
      write (0,*) 'Spectra_Standard_Exposure(i)                   = ', Spectra_Standard_Exposure(i)
      write (0,*) 'CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)  = ', CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
      write (0,*) 'CALSPEC_Flux_Blurred                           = ', CALSPEC_Flux_Blurred
      write (0,*) 'Extinction_Delta_Mag(i)                        = ', Extinction_Delta_Mag(i)
      write (0,*) 'Spectra_Standard_AM(i)                         = ', Spectra_Standard_AM(i)
      write (99,*)
      write (99,*) 'm                                              = ', m
      write (99,*) 'CALSPEC_Index                                  = ', CALSPEC_Index
      write (99,*) 'i                                              = ', i
      write (99,*) 'CALSPEC_Number(i)                              = ', CALSPEC_Number(i)
      write (99,*) 'Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index) = ', Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index)
      write (99,*) 'Spectra_Standard_Exposure(i)                   = ', Spectra_Standard_Exposure(i)
      write (99,*) 'CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)  = ', CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
      write (99,*) 'CALSPEC_Flux_Blurred                           = ', CALSPEC_Flux_Blurred
      write (99,*) 'Extinction_Delta_Mag(i)                        = ', Extinction_Delta_Mag(i)
      write (99,*) 'Spectra_Standard_AM(i)                         = ', Spectra_Standard_AM(i)
      stop
    endif
      
  enddo   ! End of loop over the stars

  ! Sensitivity function is zero order extinction coefficient when not using Build extinction model
  if ( .not. Extinction_Model_Apply ) then
  
    Call Linear_Fit (Spectra_Standard, Spectra_Standard_AM, Extinction_Delta_Mag, Extinction_Ang(CALSPEC_Index,1),     Extinction_Ang(CALSPEC_Index,2), &
                                                                                  Extinction_Ang_Unc(CALSPEC_Index,1), Extinction_Ang_Unc(CALSPEC_Index,2) )
    ! Assign zero order coefficient to sensitivity array
    Sensitivity_Ang(CALSPEC_Index) = 10.0 ** ( Extinction_Ang(CALSPEC_Index,1) /- 2.5 )
    Sensitivity_Ang_Unc(CALSPEC_Index) = 10.0 ** ( Extinction_Ang_Unc(CALSPEC_Index,1) / 2.5 )
    ! Output to sensitivity file
    write (8,*) m, Sensitivity_Ang(CALSPEC_Index), Sensitivity_Ang_Unc(CALSPEC_Index)

  
  endif
  
  ! Diagnostic
  if ( .false. ) then
    write (0,*)
    write (0,*) 'm                                             = ', m
    write (0,*) 'Extinction_Ang(CALSPEC_Index,1)               = ', Extinction_Ang(CALSPEC_Index,1)
    write (0,*) 'Extinction_Ang(CALSPEC_Index,2)               = ', Extinction_Ang(CALSPEC_Index,2)
    write (99,*)
    write (99,*) 'm                                             = ', m
    write (99,*) 'Extinction_Ang(CALSPEC_Index,1)               = ', Extinction_Ang(CALSPEC_Index,1)
    write (99,*) 'Extinction_Ang(CALSPEC_Index,2)               = ', Extinction_Ang(CALSPEC_Index,2)
    stop
  endif
  
  ! Output to extinction file
  write (7,*) m, Extinction_Ang(CALSPEC_Index,2), Extinction_Ang_Unc(CALSPEC_Index,2)

enddo  ! End of loop over the range of wavelengths

! If using Build extiction model, correct standard star fluxes for extinction and exposure duration prior to computing sensitivity
if ( Extinction_Model_Apply ) then

  ! Loop over the range of wavelengths
  do m = Wave_Min_Proc, Wave_Max_Proc

    ! Assign the CALSPEC index (1-7001 for 3000-10000)
    CALSPEC_Index = m  - CALSPEC_Wave_Min + 1
  
    ! Loop over the stars
    do i = 1, Spectra_Standard

      ! Compute luminosity ratio from extinction coefficient and air mass
      Extn_Ratio = 10. ** ( ( Extinction_Ang(CALSPEC_Index,2) * Spectra_Standard_AM(i) ) / 2.5 )
                    !   1               2               2                      2 2 1
    
      ! Apply corrections to extinction and exposure duration to raw flux
      Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index) = Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index) * Extn_Ratio / Spectra_Standard_Exposure(i)
    
      ! Diagnostic
      if ( .false. ) then
        write (0,*) 'm, i, CALSPEC_Index         = ', m, i, CALSPEC_Index
        write (0,*) 'Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index)      = ', Spectra_Standard_Flux_Raw_Ang(i,CALSPEC_Index)
        write (0,*) 'Extinction_Ang(CALSPEC_Index,2)                     = ', Extinction_Ang(CALSPEC_Index,2)
        write (0,*) 'Spectra_Standard_AM(i)                              = ', Spectra_Standard_AM(i)
        write (0,*) 'Extn_Ratio                                          = ', Extn_Ratio
        write (0,*) 'Spectra_Standard_Exposure(i)                        = ', Spectra_Standard_Exposure(i)
        write (0,*) 'Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index) = ', Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index)
        stop
      endif

    enddo   ! End of loop over the stars

  enddo  ! End of loop over the range of wavelengths

  ! Derive the sensitivity function

  ! Loop over the range of wavelengths
  do m = Wave_Min_Proc, Wave_Max_Proc

    ! Assign the CALSPEC index (1-7001 for 3000-10000)
    CALSPEC_Index = m  - CALSPEC_Wave_Min + 1
  
    ! Loop over the stars
    Sensitivity_Accum = 0.0
    do i = 1, Spectra_Standard

      ! Blur the CALSPEC fluxes over the half-width
      CALSPEC_Flux_Blurred = 0.0
      do j = -Half_Width, Half_Width
        CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred + CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index+j)
      enddo
    
      ! Normalize the blurred CALSPEC flux
      CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred / Half_Width_Elements

      ! Compute sensitivity ratio
      Sensitivity_Ratio = Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index) / CALSPEC_Flux_Blurred
    
      ! Add to accumulation
      Sensitivity_Accum = Sensitivity_Accum + Sensitivity_Ratio
    
      ! Diagnostic
      if ( .false. ) then
        write (0,*)
        write (0,*) 'm, CALSPEC_Index       = ', m, CALSPEC_Index
        write (0,*) 'i, CALSPEC_Number(i)   = ', i, CALSPEC_Number(i)
        write (0,*) 'CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)       = ', CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
        write (0,*) 'Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index) = ', Spectra_Standard_Flux_Extn_Exp_Ang(i,CALSPEC_Index)
        write (0,*) 'Sensitivity_Ratio                                   = ', Sensitivity_Ratio
        write (0,*) 'Sensitivity_Accum                                   = ', Sensitivity_Accum
     endif
    
    enddo   ! End of loop over the stars
  
    ! Compute the average sensitivity
    Sensitivity_Ang(CALSPEC_Index) = Sensitivity_Accum / Spectra_Standard
  
    ! Diagnostic
    if ( .false. ) then
      write (0,*) 'Sensitivity_Ang(CALSPEC_Index) = ', Sensitivity_Ang(CALSPEC_Index)
      stop
    endif

    ! Output to sensitivity file
    write (8,*) m, Sensitivity_Ang(CALSPEC_Index)

  enddo   ! End of loop over the range of wavelengths

endif   ! End of computing sensitivity function separately from extinction when using Build extinction model

! Skip the following test of the air mass range if using Buil function
if ( Extinction_Model_Apply ) goto 9342

! Check that the range of standard star air masses exceeds the minimum advised value
Spectra_Standard_AM_Min = +1000000.
Spectra_Standard_AM_Max = -1000000.

! Loop over the stars
do i = 1, Spectra_Standard

  if ( Spectra_Standard_AM(i) > Spectra_Standard_AM_Max ) Spectra_Standard_AM_Max = Spectra_Standard_AM(i)
  if ( Spectra_Standard_AM(i) < Spectra_Standard_AM_Min ) Spectra_Standard_AM_Min = Spectra_Standard_AM(i)
   
enddo

! Compare to minimum advised value
Spectra_Standard_AM_Delta = Spectra_Standard_AM_Max - Spectra_Standard_AM_Min
if ( Spectra_Standard_AM_Delta < Advisory_Air_Mass_Delta ) then
  Advisory_Flag_Cumulative = .true.
  Advisory_String = '*** Standard star air mass range is less than advised minimum ***'
  write (0,2321) Advisory_String
  write (6,2321) Advisory_String
  write (0,1040) Spectra_Standard_AM_Delta, Advisory_Air_Mass_Delta
  write (6,1040) Spectra_Standard_AM_Delta, Advisory_Air_Mass_Delta
  1040 format ('Range = ', f10.3,5x,'Minimum = ', f10.3)
endif

9342 continue
if ( Spectra_Program == 0 ) goto 9343

! *****************************************************************************
! **************** Part 6: Apply Calibration to Program Stars *****************
! *****************************************************************************

! Diagnostic
write (0,1912)
write (6,1912)
1912 format (/,'Calibration of program stars:')

! Loop over program stars
do i = 1, Spectra_Program

  ! Display
  write (0,1211) i, Spectra_Program_File(i)
  write (6,1211) i, Spectra_Program_File(i)

  ! Parse and compute the RA
  read (Spectra_Program_RA_String(i)(1:2),*) RAhr
  read (Spectra_Program_RA_String(i)(4:5),*) RAmn
  read (Spectra_Program_RA_String(i)(7:8),*) RAsc
  RA = ( RAhr + RAmn / 60. + RAsc / 3600. ) * 15.0 / Rad2Deg
  Spectra_Program_RA_Input(i) = RA
    
  ! Parse and compute the Dec
  read (Spectra_Program_DC_String(i)(2:3),*) DCdg
  read (Spectra_Program_DC_String(i)(5:6),*) DCmn
  read (Spectra_Program_DC_String(i)(8:9),*) DCsc
  DC = ( DCdg + DCmn / 60. + DCsc / 3600. ) / Rad2Deg
  if ( Spectra_Program_DC_String(i)(1:1) == "-" ) DC = -DC
  Spectra_Program_DC_Input(i) = DC
  
  ! Precess the coordinates if needed
  if ( Spectra_Program_J2000 ) then
    Call Precess ( JD, Spectra_Program_RA_Input(i), Spectra_Program_DC_Input(i), Spectra_Program_RA_of_Date(i), Spectra_Program_DC_of_Date(i), Pi )
  else
    Spectra_Program_RA_of_Date(i) = Spectra_Program_RA_Input(i)
    Spectra_Program_DC_of_Date(i) = Spectra_Program_DC_Input(i)
  endif

  ! Compute the air mass
  Call AirMass (Lat, Lng, ST0_site, Spectra_Program_Time(i), Spectra_Program_RA_of_Date(i), Spectra_Program_DC_of_Date(i), &
                Pi, Spectra_Program_AM(i), Advisory_Air_Mass_Limit, Advisory_Flag, Advisory_String, Advisory_Flag_Cumulative)
  if ( Advisory_Flag ) then
    write (0,2321) Advisory_String
    write (6,2321) Advisory_String
  endif
  write (0,1232) Spectra_Program_AM(i)
  write (6,1232) Spectra_Program_AM(i)
  ! write (0,843) Spectra_Program_Exposure(i)
  ! write (6,843) Spectra_Program_Exposure(i)
  if ( Spectra_Program_AM(i) > Spectra_Standard_AM_Max .or. Spectra_Program_AM(i) < Spectra_Standard_AM_Min ) then
    Advisory_Flag_Cumulative = .true.
    Advisory_String = '*** Program star air mass is outside of standard star range ***'
    write (0,2321) Advisory_String
    write (6,2321) Advisory_String
    write (0,1041) Spectra_Program_AM(i), Spectra_Standard_AM_Min, Spectra_Standard_AM_Max
    write (6,1041) Spectra_Program_AM(i), Spectra_Standard_AM_Min, Spectra_Standard_AM_Max
    1041 format ('Air mass = ', f10.3,5x,'Range = ', 2f10.3)
  endif

  ! Diagnostic
  if ( .false. ) then
    write (0,*)
    write (0,*) 'i                             ', i
    write (0,*) 'Spectra_Program_RA_String(i)  ', Spectra_Program_RA_String(i)
    write (0,*) 'Spectra_Program_RA_Input(i)   ', Spectra_Program_RA_Input(i)
    write (0,*) 'Spectra_Program_RA_of_Date(i) ', Spectra_Program_RA_of_Date(i)
    write (0,*) 'Spectra_Program_DC_String(i)  ', Spectra_Program_DC_String(i)
    write (0,*) 'Spectra_Program_DC_Input(i)   ', Spectra_Program_DC_Input(i)
    write (0,*) 'Spectra_Program_DC_of_Date(i) ', Spectra_Program_DC_of_Date(i)
    write (0,*) 'Spectra_Program_AM(i)         ', Spectra_Program_AM(i)
    ! stop
  endif

  ! Build the path+file name of the program star input file
  call Build_Path_File_Name ( Spectra_Program_Path, Spectra_Program_File(i), Path_File_Name, Blank100 )

  ! Open the program star input file
  open (4,file=Path_File_Name,action='read',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'Program star file (input) iostat = ', io_flag
    stop
  endif

  ! Build the path+file name of the program star output file
  call Build_Path_File_Name ( Calibrated_Path, Spectra_Program_File(i), Path_File_Name, Blank100 )

  ! Open the program star output file
  open (9,file=Path_File_Name,action='write',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'Program star file (output) iostat = ', io_flag
    stop
  endif

  ! Input the raw spectrum
  Call Input_Raw_Spectrum ( i, Spectra_Program_Wave, Spectra_Program_Flux_Raw, Wave_Min_Proc, Wave_Max_Proc, Spectra_Program_Records, Pavlov_Format )

  ! Apply calibrations and output the calibrated spectrum
  do j = 1, Spectra_Program_Records(i)
  
    ! Skip record if not in user-specified range
    if ( Spectra_Program_Wave(i,j) < Wave_Min_Proc .or. Spectra_Program_Wave(i,j) > Wave_Max_Proc ) goto 1002

    ! Compute index to extinction and sensitivity values
    CALSPEC_Index = Spectra_Program_Wave(i,j) - CALSPEC_Wave_Min + 1
    
    ! Compute luminosity ratio from extinction coefficient and air mass
    Extn_Ratio = 10. ** ( ( Extinction_Ang(CALSPEC_Index,2) * Spectra_Program_AM(i) ) / 2.5 )
    
    ! Apply correction for extinction
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Raw(i,j) * Extn_Ratio
    
    ! Apply the correction for exposure
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Cal(i,j) / Spectra_Program_Exposure(i)

    ! Apply the correction for sensitivity
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Cal(i,j) / Sensitivity_Ang(CALSPEC_Index)
        
    ! Diagnostic
    if ( .false. ) then
      write (0,*) 'i, j, CALSPEC_Index             = ', i, j, CALSPEC_Index
      write (0,*) 'Spectra_Program_Wave(i,j)       = ', Spectra_Program_Wave(i,j)
      write (0,*) 'Spectra_Program_Flux_Raw(i,j)   = ', Spectra_Program_Flux_Raw(i,j)
      write (0,*) 'Extinction_Ang(CALSPEC_Index,2) = ', Extinction_Ang(CALSPEC_Index,2)
      write (0,*) 'Spectra_Program_AM(i)           = ', Spectra_Program_AM(i)
      write (0,*) 'Extn_Ratio                      = ', Extn_Ratio
      write (0,*) 'Spectra_Program_Exposure(i)     = ', Spectra_Program_Exposure(i)
      write (0,*) 'Sensitivity_Ang(CALSPEC_Index)  = ', Sensitivity_Ang(CALSPEC_Index)
      write (0,*) 'Spectra_Program_Flux_Cal(i,j)   = ', Spectra_Program_Flux_Cal(i,j)
      stop
    endif

    ! Output the calibrated spectrum
    write (9,*) Spectra_Program_Wave(i,j), Spectra_Program_Flux_Cal(i,j)

    ! Skip here if wavelength is outside of user-specified range
    1002 continue

  enddo   ! End of loop over calibrations

enddo    ! End of loop over program star files

9343 continue   ! skip here if there are no program stars

! *****************************************************************************
! *************** Part 7: Assess the Standard Star Calibrations ***************
! *****************************************************************************

! Diagnostic
write (0,1919)
write (6,1919)
1919 format (/,'Assessment of standard star calibrations:')

! Initialize the accumulating variables for the RMS of the average differences and the RMS of the RMS differences
Calibrated_to_CALSPEC_Diff_Average_of_Averages      = 0.0
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs              = 0.0
Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur = 0.0
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur         = 0.0

! Loop over standard stars
do i = 1, Spectra_Standard

  ! Identify the corresponding CALSPEC star
  call ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, Spectra_Standard_File(i), CALSPEC_Number(i), Pavlov_Format)

  ! Build the path+file name of the standard star output file
  call Build_Path_File_Name ( Spectra_Standard_Path_Eval, Spectra_Standard_File(i), Path_File_Name, Blank100 )

  ! Open the standard star evaluation output file
  open (11,file=Path_File_Name,action='write',iostat=io_flag)
  if ( io_flag .ne. 0 ) then
    write (0,*) 'Standard star file (output) iostat = ', io_flag
    stop
  endif

  ! Initialize the accumulating variable for differences between calibrated standard star fluxes and CALSPEC fluxes
  Calibrated_to_CALSPEC_Diff_Accum(i) = 0.0
  Calibrated_to_CALSPEC_Diff_Sq_Accum(i) = 0.0

  ! Process each wavelength/flux pair
   do j = 1, Spectra_Standard_Records(i)
  
    ! Skip record if not in user-specified range
    if ( Spectra_Standard_Wave(i,j) < Wave_Min_Proc .or. Spectra_Standard_Wave(i,j) > Wave_Max_Proc ) goto 1092

    ! Compute index to extinction and sensitivity values
    CALSPEC_Index = Spectra_Standard_Wave(i,j) - CALSPEC_Wave_Min + 1
    
    ! Compute luminosity ratio from extinction coefficient and air mass
    Extn_Ratio = 10. ** ( ( Extinction_Ang(CALSPEC_Index,2) * Spectra_Standard_AM(i) ) / 2.5 )
    
    ! Apply correction for extinction
    Spectra_Standard_Flux_Cal_Full(i,j) = Spectra_Standard_Flux_Raw(i,j) * Extn_Ratio
    
    ! Apply the correction for exposure
    Spectra_Standard_Flux_Cal_Full(i,j) = Spectra_Standard_Flux_Cal_Full(i,j) / Spectra_Standard_Exposure(i)

    ! Apply the correction for sensitivity
    Spectra_Standard_Flux_Cal_Full(i,j) = Spectra_Standard_Flux_Cal_Full(i,j) / Sensitivity_Ang(CALSPEC_Index)
    
    ! Determine normalized calibrated-minus-CALSPEC flux differences and its square (not blurred)
    Calibrated_to_CALSPEC_Diff(i,j) = ( Spectra_Standard_Flux_Cal_Full(i,j) - CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index) ) / CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
    Calibrated_to_CALSPEC_Diff_Sq(i,j) = Calibrated_to_CALSPEC_Diff(i,j) * Calibrated_to_CALSPEC_Diff(i,j)

    ! Sum the CALSPEC fluxes over the half-width to blur it
    CALSPEC_Flux_Blurred = 0.0
    do k = -Half_Width, Half_Width
      CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred + CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index+k)
    enddo

    ! Normalize the blured CALSPEC flux
    CALSPEC_Flux_Blurred = CALSPEC_Flux_Blurred / Half_Width_Elements
    
    ! Determine normalized calibrated-minus-CALSPEC flux differences and its square for the blurred CalSpec value
    Calibrated_to_CALSPEC_Diff_Blur(i,j) = ( Spectra_Standard_Flux_Cal_Full(i,j) - CALSPEC_Flux_Blurred ) / CALSPEC_Flux_Blurred
    Calibrated_to_CALSPEC_Diff_Sq_Blur(i,j) = Calibrated_to_CALSPEC_Diff_Blur(i,j) * Calibrated_to_CALSPEC_Diff_Blur(i,j)

    ! Accumulate the differences
    Calibrated_to_CALSPEC_Diff_Accum(i) = Calibrated_to_CALSPEC_Diff_Accum(i) + Calibrated_to_CALSPEC_Diff(i,j)
    Calibrated_to_CALSPEC_Diff_Sq_Accum(i) = Calibrated_to_CALSPEC_Diff_Sq_Accum(i) + Calibrated_to_CALSPEC_Diff_Sq(i,j)
    Calibrated_to_CALSPEC_Diff_Accum_Blur(i) = Calibrated_to_CALSPEC_Diff_Accum_Blur(i) + Calibrated_to_CALSPEC_Diff_Blur(i,j)
    Calibrated_to_CALSPEC_Diff_Sq_Accum_Blur(i) = Calibrated_to_CALSPEC_Diff_Sq_Accum_Blur(i) + Calibrated_to_CALSPEC_Diff_Sq_Blur(i,j)
    
    ! Write wavelength and calibrated flux to output file for evaluation
    write (11,*) Spectra_Standard_Wave(i,j), Spectra_Standard_Flux_Cal_Full(i,j)
  
    ! Diagnostic
    if ( int(j/100)*100==j .and. .false. ) then
      write (0,*) 'i, j, CALSPEC_Index             = ', i, j, CALSPEC_Index
      write (0,*) 'Spectra_Standard_Wave(i,j)       = ', Spectra_Standard_Wave(i,j)
      write (0,*) 'Spectra_Standard_Flux_Raw(i,j)   = ', Spectra_Standard_Flux_Raw(i,j)
      write (0,*) 'Extinction_Ang(CALSPEC_Index,2) = ', Extinction_Ang(CALSPEC_Index,2)
      write (0,*) 'Spectra_Standard_AM(i)           = ', Spectra_Standard_AM(i)
      write (0,*) 'Extn_Ratio                      = ', Extn_Ratio
      write (0,*) 'Spectra_Standard_Exposure(i)     = ', Spectra_Standard_Exposure(i)
      write (0,*) 'Sensitivity_Ang(CALSPEC_Index)  = ', Sensitivity_Ang(CALSPEC_Index)
      write (0,*) 'Spectra_Standard_Flux_Cal_Full(CALSPEC_Index,j)   = ', Spectra_Standard_Flux_Cal_Full(i,j)
      write (0,*) 'CALSPEC_Number(i)                               = ', CALSPEC_Number(i)
      write (0,*) 'CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)   = ', CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
      write (0,*) 'Calibrated_to_CALSPEC_Diff(i,j)                 = ', Calibrated_to_CALSPEC_Diff(i,j)
      write (0,*) 'Calibrated_to_CALSPEC_Diff_Accum(i)             = ', Calibrated_to_CALSPEC_Diff_Accum(i)
      ! stop
    endif
    
    ! Skip here if wavelength is outside of user-specified range
    1092 continue

  enddo   ! End of loop over calibrations

  ! Compute the normalized average difference and the RMS
  Calibrated_to_CALSPEC_Diff_Average(i)      = Calibrated_to_CALSPEC_Diff_Accum(i) / Spectra_Standard_Records(i)
  Calibrated_to_CALSPEC_Diff_RMS(i)          = sqrt ( Calibrated_to_CALSPEC_Diff_Sq_Accum(i) / Spectra_Standard_Records(i) )
  Calibrated_to_CALSPEC_Diff_Average_Blur(i) = Calibrated_to_CALSPEC_Diff_Accum_Blur(i) / Spectra_Standard_Records(i)
  Calibrated_to_CALSPEC_Diff_RMS_Blur(i)     = sqrt ( Calibrated_to_CALSPEC_Diff_Sq_Accum_Blur(i) / Spectra_Standard_Records(i) )
  
  ! Add to the accumulating variables for the RMS of the average differences and the RMS of the RMS differences
  Calibrated_to_CALSPEC_Diff_Average_of_Averages      = Calibrated_to_CALSPEC_Diff_Average_of_Averages + Calibrated_to_CALSPEC_Diff_Average(i)
  Calibrated_to_CALSPEC_Diff_RMS_of_RMSs              = Calibrated_to_CALSPEC_Diff_RMS_of_RMSs + Calibrated_to_CALSPEC_Diff_RMS(i) ** 2
  Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur = Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur + Calibrated_to_CALSPEC_Diff_Average_Blur(i)
  Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur         = Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur + Calibrated_to_CALSPEC_Diff_RMS_Blur(i) ** 2

  ! Display
  write (0,1211) i, Spectra_Standard_File(i)
  write (6,1211) i, Spectra_Standard_File(i)
  write (0,1233) Spectra_Standard_AM(i)
  write (6,1233) Spectra_Standard_AM(i)
  1233 format ('Air Mass        = ', f5.2)
  write (0,1048) Calibrated_to_CALSPEC_Diff_Average(i), Calibrated_to_CALSPEC_Diff_Average_Blur(i)
  write (6,1048) Calibrated_to_CALSPEC_Diff_Average(i), Calibrated_to_CALSPEC_Diff_Average_Blur(i)
  1048 format ('Normalized average bias = ', f6.3,3x,'Blurred = ',f6.3)
  write (0,1049) Calibrated_to_CALSPEC_Diff_RMS(i), Calibrated_to_CALSPEC_Diff_RMS_Blur(i)
  write (6,1049) Calibrated_to_CALSPEC_Diff_RMS(i), Calibrated_to_CALSPEC_Diff_RMS_Blur(i)
  1049 format ('Normalized RMS          = ', f6.3,3x,'Blurred = ',f6.3)

enddo   ! End of loop over standard stars

! Compute the average of the average differences and the RMS of the RMS differences for all stars
Calibrated_to_CALSPEC_Diff_Average_of_Averages      = Calibrated_to_CALSPEC_Diff_Average_of_Averages / Spectra_Standard
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs              = sqrt ( Calibrated_to_CALSPEC_Diff_RMS_of_RMSs / Spectra_Standard )
Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur = Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur / Spectra_Standard
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur         = sqrt ( Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur / Spectra_Standard )

! Compute the reduced values if there are more than two standard stars
if ( Spectra_Standard > 2 ) then
  RMS_Reduction_Factor = sqrt ( Spectra_Standard - 2. )
  Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Reduced = Calibrated_to_CALSPEC_Diff_RMS_of_RMSs / RMS_Reduction_Factor
  Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur_Reduced = Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur / RMS_Reduction_Factor
endif

! Display biases and RMSs
write (0,2047)
write (6,2047)
2047 format (/,'Statistics over all stars:')
write (0,2048) Calibrated_to_CALSPEC_Diff_Average_of_Averages, Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur
write (6,2048) Calibrated_to_CALSPEC_Diff_Average_of_Averages, Calibrated_to_CALSPEC_Diff_Average_of_Averages_Blur
2048 format ('Average of normalized averages biases = ', f6.3,3x,'Blurred = ',f6.3)
write (0,2049) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs, Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur
write (6,2049) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs, Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur
2049 format ('RMS of normalized RMSs                = ', f6.3,3x,'Blurred = ',f6.3)

! Display reduced RMSs if there are more than two standard stars
write (0,1111) Spectra_Standard
write (6,1111) Spectra_Standard
1111 format (/,'Number of standard stars              = ', i3)
if ( Spectra_Standard > 2 ) then
  write (0,1112) RMS_Reduction_Factor
  write (6,1112) RMS_Reduction_Factor
  1112 format ('RMS reduction factor                  = ', f6.3)
  write (0,1113) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Reduced, Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur_Reduced
  write (6,1113) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Reduced, Calibrated_to_CALSPEC_Diff_RMS_of_RMSs_Blur_Reduced
  1113 format ('Reduced RMS overall                   = ', f6.3,3x,'Blurred = ',f6.3)
else
  write (0,1114) 
  write (6,1114)
  1114 format ('Too far stars to compute a reduced RMS')
endif

! *****************************************************************************
! **** Part 8: Assess the Program Star Calibrations (for simulation only) *****
! *****************************************************************************

! Skip if this is not a simulation run
if ( .not. Sim ) goto 3412

! Diagnostic
write (0,2919)
write (6,2919)
2919 format (/,'Assessment of program star calibrations:')

! Initialize the accumulating variables for the RMS of the average differences and the RMS of the RMS differences
Calibrated_to_CALSPEC_Diff_Average_of_Averages = 0.0
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs         = 0.0

! Loop over program stars
do i = 1, Spectra_Program

  ! Identify the corresponding CALSPEC star
  call ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, Spectra_Program_File(i), CALSPEC_Number(i), .false.)

  ! Initialize the accumulating variable for differences between calibrated standard star fluxes and CALSPEC fluxes
  Calibrated_to_CALSPEC_Diff_Accum(i) = 0.0
  Calibrated_to_CALSPEC_Diff_Sq_Accum(i) = 0.0

  ! Process each wavelength/flux pair
   do j = 1, Spectra_Program_Records(i)
  
    ! Skip record if not in user-specified range
    if ( Spectra_Program_Wave(i,j) < Wave_Min_Proc .or. Spectra_Program_Wave(i,j) > Wave_Max_Proc ) goto 2092

    ! Compute index to extinction and sensitivity values
    CALSPEC_Index = Spectra_Program_Wave(i,j) - CALSPEC_Wave_Min + 1
    
    ! Compute luminosity ratio from extinction coefficient and air mass
    Extn_Ratio = 10. ** ( ( Extinction_Ang(CALSPEC_Index,2) * Spectra_Program_AM(i) ) / 2.5 )
    
    ! Apply correction for extinction
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Raw(i,j) * Extn_Ratio
    
    ! Apply the correction for exposure
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Cal(i,j) / Spectra_Program_Exposure(i)

    ! Apply the correction for sensitivity
    Spectra_Program_Flux_Cal(i,j) = Spectra_Program_Flux_Cal(i,j) / Sensitivity_Ang(CALSPEC_Index)
        
    ! Determine normalized calibrated-minus-CALSPEC flux differences and its square
    Calibrated_to_CALSPEC_Diff(i,j) = ( Spectra_Program_Flux_Cal(i,j) - CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index) ) / CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
    Calibrated_to_CALSPEC_Diff_Sq(i,j) = Calibrated_to_CALSPEC_Diff(i,j) * Calibrated_to_CALSPEC_Diff(i,j)

    ! Accumulate the differences
    Calibrated_to_CALSPEC_Diff_Accum(i) = Calibrated_to_CALSPEC_Diff_Accum(i) + Calibrated_to_CALSPEC_Diff(i,j)
    Calibrated_to_CALSPEC_Diff_Sq_Accum(i) = Calibrated_to_CALSPEC_Diff_Sq_Accum(i) + Calibrated_to_CALSPEC_Diff_Sq(i,j)
  
    ! Diagnostic
    if ( int(j/100)*100==j .and. .false. ) then
      write (0,*) 'i, j, CALSPEC_Index             = ', i, j, CALSPEC_Index
      write (0,*) 'Spectra_Program_Wave(i,j)       = ', Spectra_Program_Wave(i,j)
      write (0,*) 'Spectra_Program_Flux_Raw(i,j)   = ', Spectra_Program_Flux_Raw(i,j)
      write (0,*) 'Extinction_Ang(CALSPEC_Index,2) = ', Extinction_Ang(CALSPEC_Index,2)
      write (0,*) 'Spectra_Program_AM(i)           = ', Spectra_Program_AM(i)
      write (0,*) 'Extn_Ratio                      = ', Extn_Ratio
      write (0,*) 'Spectra_Program_Exposure(i)     = ', Spectra_Program_Exposure(i)
      write (0,*) 'Sensitivity_Ang(CALSPEC_Index)  = ', Sensitivity_Ang(CALSPEC_Index)
      write (0,*) 'Spectra_Program_Flux_Cal(CALSPEC_Index,j)   = ', Spectra_Program_Flux_Cal(i,j)
      write (0,*) 'CALSPEC_Number(i)                               = ', CALSPEC_Number(i)
      write (0,*) 'CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)   = ', CALSPEC_Flux(CALSPEC_Number(i),CALSPEC_Index)
      write (0,*) 'Spectra_Program_Flux_Cal(i,j)         = ', Spectra_Program_Flux_Cal(i,j)
      write (0,*) 'Calibrated_to_CALSPEC_Diff(i,j)                 = ', Calibrated_to_CALSPEC_Diff(i,j)
      write (0,*) 'Calibrated_to_CALSPEC_Diff_Accum(i)             = ', Calibrated_to_CALSPEC_Diff_Accum(i)
      ! stop
    endif
    
    ! Skip here if wavelength is outside of user-specified range
    2092 continue

  enddo   ! End of loop over calibrations

  ! Compute the normalized average difference and the RMS
  Calibrated_to_CALSPEC_Diff_Average(i) = Calibrated_to_CALSPEC_Diff_Accum(i) / Spectra_Program_Records(i)
  Calibrated_to_CALSPEC_Diff_RMS(i)     = sqrt ( Calibrated_to_CALSPEC_Diff_Sq_Accum(i) / Spectra_Program_Records(i) )
  
  ! Add to the accumulating variables for the RMS of the average differences and the RMS of the RMS differences
  Calibrated_to_CALSPEC_Diff_Average_of_Averages = Calibrated_to_CALSPEC_Diff_Average_of_Averages + Calibrated_to_CALSPEC_Diff_Average(i)
  Calibrated_to_CALSPEC_Diff_RMS_of_RMSs         = Calibrated_to_CALSPEC_Diff_RMS_of_RMSs + Calibrated_to_CALSPEC_Diff_RMS(i) ** 2

  ! Display
  write (0,1211) i, Spectra_Program_File(i)
  write (6,1211) i, Spectra_Program_File(i)
  write (0,1233) Spectra_Program_AM(i)
  write (6,1233) Spectra_Program_AM(i)
  write (0,1048) Calibrated_to_CALSPEC_Diff_Average(i)
  write (6,1048) Calibrated_to_CALSPEC_Diff_Average(i)
  write (0,1049) Calibrated_to_CALSPEC_Diff_RMS(i)
  write (6,1049) Calibrated_to_CALSPEC_Diff_RMS(i)

enddo   ! End of loop over standard stars

! Compute the average of the average differences and the RMS of the RMS differences for all stars
Calibrated_to_CALSPEC_Diff_Average_of_Averages = Calibrated_to_CALSPEC_Diff_Average_of_Averages / Spectra_Program
Calibrated_to_CALSPEC_Diff_RMS_of_RMSs         = sqrt ( Calibrated_to_CALSPEC_Diff_RMS_of_RMSs / Spectra_Program )

! Display
write (0,2047)
write (6,2047)
write (0,2048) Calibrated_to_CALSPEC_Diff_Average_of_Averages
write (6,2048) Calibrated_to_CALSPEC_Diff_Average_of_Averages
write (0,2049) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs
write (6,2049) Calibrated_to_CALSPEC_Diff_RMS_of_RMSs

3412 continue   ! Skip here if not a simulation run

! *****************************************************************************
! ********************* Part 9: Display Advisory Messages *********************
! *****************************************************************************

if ( Advisory_Flag_Cumulative ) then
  write (0,1056) 
  write (6,1056) 
  1056 format (/,'*** There are one or more advisory messages above ***')
endif

! *****************************************************************************
! **************************** End of Main Program ****************************
! *****************************************************************************

stop
end


subroutine LaGrange ( Target_Wave_Interp, Target_Flux_Interp, Input_Wave_Interp, Input_Flux_Interp )
! LaGrangian interpolation in the input fluxes to the target wavelength

implicit none
double precision             Input_Wave_Interp(3)                ! Three values of input wavelengths for quadratic interpolation
double precision             Input_Flux_Interp(3)                ! Three values of input fluxes for quadratic interpolation
double precision             Target_Wave_Interp                  ! Target value of wavelength
double precision             Target_Flux_Interp                  ! Target value of fluxes

Target_Flux_Interp = &
  Input_Flux_Interp(1) * ( ( Target_Wave_Interp - Input_Wave_Interp(2) ) / ( Input_Wave_Interp(1) - Input_Wave_Interp(2) ) &
                       *   ( Target_Wave_Interp - Input_Wave_Interp(3) ) / ( Input_Wave_Interp(1) - Input_Wave_Interp(3) ) ) + &
  Input_Flux_Interp(2) * ( ( Target_Wave_Interp - Input_Wave_Interp(1) ) / ( Input_Wave_Interp(2) - Input_Wave_Interp(1) ) &
                       *   ( Target_Wave_Interp - Input_Wave_Interp(3) ) / ( Input_Wave_Interp(2) - Input_Wave_Interp(3) ) ) + &
  Input_Flux_Interp(3) * ( ( Target_Wave_Interp - Input_Wave_Interp(1) ) / ( Input_Wave_Interp(3) - Input_Wave_Interp(1) ) &
                       *   ( Target_Wave_Interp - Input_Wave_Interp(2) ) / ( Input_Wave_Interp(3) - Input_Wave_Interp(2) ) )

! Diagnostic
if ( .false. ) then
  write (0,*) Input_Wave_Interp
  write (0,*) Input_Flux_Interp
  write (0,*) Target_Wave_Interp
  write (0,*) Target_Flux_Interp
  stop
endif

! Finish
return
end


subroutine Precess (JD, RA_in, DC_in, RA_out, DC_out, Pi)
! Precess from J2000 to current time

implicit none
double precision          JD                                          ! Julian date
double precision          RA_in, RA_out, DC_in, DC_out                ! RA and Dec input and output
double precision          zeta, z, theta, a, b, c                     ! Intermediate values
double precision          Tcap                                        ! Julian centuries from fixed epoch to J2000
double precision          Tsmall                                      ! Julian centuries from epoch of date to fixed epoch
double precision          Pi                                          ! Math constant

! Test values for 2015 April 29 22:00 UT
if ( .false. ) then
  write (0,*)
  write (0,*) 'In subroutine Precess'
  write (0,*) 'Using test values'
  JD = 2457142.416D0
endif

! Julian centuries
Tsmall = (JD - 2451545.0) / 36525.0
Tcap   = 0.0

! Compute intermediate values
zeta=(2306.2181 + 1.39656*Tcap - 0.000139*Tcap*Tcap)*Tsmall + (0.30188 - 0.000344*Tcap)*Tsmall*Tsmall + (0.017998)*Tsmall*Tsmall*Tsmall
z=(2306.2181 + 1.39656*Tcap - 0.000139*Tcap*Tcap)*Tsmall + (1.09468 + 0.000066*Tcap)*Tsmall*Tsmall + (0.018203)*Tsmall*Tsmall*Tsmall
theta=(2004.3109 - 0.85330*Tcap - 0.000217*Tcap*Tcap)*Tsmall + (-0.42665 - 0.000217*Tcap)*Tsmall*Tsmall + (-0.041833)*Tsmall*Tsmall*Tsmall
  
! Convert to radians
zeta = zeta * PI / (180.*3600.);
z = z *  PI / (180.*3600.);
theta = theta * PI / (180.*3600.);
 
! Calculate the precession in radians
a = sin(RA_in + zeta)*cos(DC_in)
b = cos(RA_in + zeta)*cos(theta)*cos(DC_in) - sin(theta)*sin(DC_in)
c = cos(RA_in + zeta)*sin(theta)*cos(DC_in) + cos(theta)*sin(DC_in)
if (c >  +0.9                ) DC_out =  acos(sqrt(a*a+b*b))
if (c <  -0.9                ) DC_out = -acos(sqrt(a*a+b*b))  
if (c >= -0.9 .and. c <= +0.9) DC_out = asin(c)
RA_out = atan2(a,b) + z
if (RA_out < 0.0 ) RA_out = RA_out + 2.0 * Pi
  
! Diagnostic
if ( .false. ) then
  write (0,*) 'Testing with HD74000'
  write (0,*) '2015 April 29 22:00 UT'
  write (0,*) 'TheSky gives:'
  write (0,*) 'RA = 08:41:33  = 130.39'
  write (0,*) 'DC = -16 24 20 = -16.40'
  write (0,*) 'Pi                     = ', Pi
  write (0,*) 'Tsmall                 = ', Tsmall
  write (0,*) 'RA input (radians)     = ', RA_in
  write (0,*) 'DC input (radians)     = ', DC_in  
  write (0,*) 'RA precessed (degrees) = ', RA_out * 180.  / Pi
  write (0,*) 'DC precessed (degrees) = ', DC_out * 180. / Pi  
  ! stop
endif

! Finish
return
end


subroutine CalcJD(Yr, Mo, Da, JD)
! Calcaulte JD at 0h UT from year, month and day

integer               Yr, Mo, Da                     ! Year, month and day
double precision      A, B, C, D                     ! Intermediate values
double precision      Day                            ! Intermediate values
integer               Year, Month                    ! Intermediate values
double precision      JD                             ! Julian date

Day = float(Da)
Year = Yr
Month = Mo
if ((Month == 1) .or. (Month == 2)) then
  Year = Year - 1
  Month = Month + 12
endif

if ((float(Year) + Month / 12.0 + Day / 365.25) >= (1582.0 + 10.0 / 12.0 + 15.0 / 365.25)) then
  A = int (Year / 100.0)
  B = 2.0 - A + int((A / 4.0))
else 
  B = 0.0;
endif

if (Year < 0.0) then
  C = int ((365.25 * float(Year) ) - 0.75)
else 
  C = int (365.25 * float (Year) )
endif

D = int ((30.6001 * float (Month + 1)))
JD = B + C + D + Day + 1720994.5D0

! Diagnostic
if ( .false. ) then
  write (0,*) 
  write (0,*) 'Yr = ', Yr
  write (0,*) 'Mo = ', Mo
  write (0,*) 'Da = ', Da
  write (0,*) 'JD = ', JD
  stop
endif

! Finish
return
end


subroutine LatLong (Latitude, Longitude, Lat, Lng, Rad2Deg)
! Convert latitude and longitude strings to radians

implicit none
character*9             Latitude, Longitude                              ! Observatory latitude and longitude strings
double precision        Lat, Lng                                         ! Observatory latitude and longitude in radians
double precision        Rad2Deg                                          ! Radian to degrees converter
double precision        Temp                                             ! Temporary variable

! Parse latitude
read (Latitude(2:3),*) Lat
read (Latitude(5:6),*) Temp
Lat = Lat + Temp / 60.0
read (Latitude(8:9),*) Temp
Lat = Lat + Temp / 3600.
if ( Latitude(1:1) == "-" ) Lat = -Lat

! Convert latitude to radians
Lat = Lat / Rad2Deg

! Parse longitude
read (Longitude(1:3),*) Lng
read (Longitude(5:6),*) Temp
Lng = Lng + Temp / 60.0
read (Longitude(8:9),*) Temp
Lng = Lng + Temp / 3600.

! Convert Longitude to radians
Lng = Lng / Rad2Deg

! Diagnostic
if ( .false. ) then
  write (0,*) 'Lat = ', Lat
  write (0,*) 'Lng = ', Lng
  stop
endif

! Finish
return
end


subroutine AirMass (Lat, Lng, ST0_site, Time_String, RA, DC, Pi, AM, Advisory_Air_Mass_Limit, Advisory_Flag, Advisory_String, Advisory_Flag_Cumulative)
! Compute the air mass of the star

implicit none

double precision             Lat, Lng                 ! Latitude and longitude
double precision             ST0_site                 ! Sidereal time at site at 0h UT
character*8                  Time_String              ! Time of observation
double precision             RA, DC                   ! Right ascension and declination
double precision             Pi                       ! Math constant
double precision             AM                       ! Air mass
double precision             ST_site                  ! Sidereal time at site
double precision             DayFr                    ! Fraction of a UT day
double precision             HA                       ! Hour angle
double precision             Temp                     ! Temporary variable
double precision             Advisory_Air_Mass_Limit  ! An air mass exceeding this value will generate an advisory message
logical                      Advisory_Flag            ! 'T' indicates that there is an advisory message
logical                      Advisory_Flag_Cumulative ! 'T' indicates that there was at least one advisory message in this program run
character*100                Advisory_String          ! Advisory message returned from subroutine

! Initialize advisory flag
Advisory_Flag = .false.

! Compute the fraction of a day
read (Time_String(1:2),*) Temp
DayFr = Temp / 24.0
read (Time_String(4:5),*) Temp
DayFr = DayFr + Temp / 1440.0
read (Time_String(7:8),*) Temp
DayFr = DayFr + Temp / 86400.0

! Sidereal time at site
ST_site = mod ( ST0_site + ( DayFr * 1.002738 ) * 2.D0 * Pi, 2.0 * Pi )

! Hour angle
HA = ST_site - RA

! Air mass
AM = 1.D0 / ( sin(Lat) * sin(DC) + cos(Lat) * cos(DC) * cos(HA) )

! Warning message
if ( AM > Advisory_Air_Mass_Limit ) then
  Advisory_Flag = .true.
  Advisory_Flag_Cumulative = .true.
  Advisory_String = '*** Air mass is greater than advised limit ***                                                      '
endif

! Warning message
if ( AM < 1.0 ) then
  Advisory_Flag = .true.
  Advisory_Flag_Cumulative = .true.
  Advisory_String = '*** Air mass is less than 1.0 ***                                                                   '
endif

! Diagnostic
if ( .false. ) then
  write (0,*)
  write (0,*) 'ST0_site, DayFr, ST_site = '
  write (0,*) ST0_site, DayFr, ST_site
  write (0,*) 'RA, HA, AM = '
  write (0,*) RA, HA, AM
  ! stop
endif

! Finish
return
end


subroutine Extinction (Wavelength, Airmass, Sim_Ext_Const, Brightness_Fraction)
! Compute the extinction factor

implicit none
double precision               Wavelength               ! Wavelength
double precision               Airmass                  ! Air mass
double precision               Sim_Ext_Const            ! Extinction constant for simulation
double precision               Coefficient              ! Extinction coefficient in magnitudes per air mass
double precision               Magnitude_Dimming        ! Dimming in magnitudes
double precision               Brightness_Fraction      ! Extinction brightness divided by full brightness

! Determine the extinction coefficient for this wavelength
Coefficient = Sim_Ext_Const / Wavelength ** 2

! Compute the dimming in magnitudes
Magnitude_Dimming = Coefficient * Airmass

! Compute the brightness fraction
Brightness_Fraction = 10.0 ** ( -Magnitude_Dimming / 2.5 )

! Diagnostic
if ( .false. ) then
  write (0,*)
  write (0,*) 'Wavelength             ', Wavelength
  write (0,*) 'Airmass                ', Airmass
  write (0,*) 'Sim_Ext_Const          ', Sim_Ext_Const
  write (0,*) 'Coefficient            ', Coefficient
  write (0,*) 'MagnitudeDimming       ', Magnitude_Dimming
  write (0,*) 'BrightnessFraction     ', Brightness_Fraction
  stop
endif

! Finish
return
end

subroutine Simulate (i, CALSPEC_Flux, Exposure, AM, CALSPEC_Number, Wave_Min_Proc, Wave_Max_Proc, &
                     Sim_Angstrom_Spacing, Rad2Deg, Linear, Half_Width, Wave_Array, Flux_Array, Pair_Counter)
! Simulate and output wavelengths and fluxes

implicit none
include "AbsFlux_Parameters.f90"
double precision        CALSPEC_Flux(Spectra_Max,CALSPEC_Pairs)          ! Array of fluxes for each CALSPEC star
double precision        Exposure                                         ! Exposure duration
integer                 CALSPEC_Number                                   ! Number of the CALSPEC star corresponding with the standard or program star
double precision        RA, DC                                           ! Right ascension and declination
integer                 Pair_Counter                                     ! Counter for pairs of wavelength and flux
integer                 Wave_Index                                       ! Index of the wavelength in the CALSPEC array
double precision        Wave_Min_Proc, Wave_Max_Proc                     ! Minimum and maximum wavelengths to process
double precision        Sim_Angstrom_Spacing                             ! Spacing between wavelengths
double precision        Wave                                             ! Generic wavelength
double precision        Flux                                             ! Generic flux
double precision        Wave_Array(Spectra_Pairs)                        ! Simulated wavelength
double precision        Flux_Array(Spectra_Pairs)                        ! Simulated flux
double precision        Triad_Wave(3)                                    ! Three wavelengths used for LaGrangian interpolation
double precision        Triad_Flux(3)                                    ! Three fluxes used for LaGrangian interpolation
double precision        Lat, Lng                                         ! Observatory latitude and longitude in radians
double precision        ST0_site                                         ! Siderial time at site coordinates at JD
double precision        DayFr                                            ! Fraction of a day
double precision        AM                                               ! Air mass
double precision        Extinction_Fraction                              ! Fraction of brightness remaining after extinction
character*8             Time                                             ! Times of spectrum
integer                 i, j, k, m                                       ! Loop indices
integer                 io_flag                                          ! Return value from IO operation
double precision        Rad2Deg                                          ! Radian to degrees converter
double precision        Frac_Wave                                        ! Fractional wavelength, used for linear interpolation
logical                 Linear                                           ! Linear interpolation, rather than LaGrangian
integer                 Half_Width                                       ! Half the resolution of the spectra in Angstroms, simulated and observed

! Loop over wavelengths to process
Pair_Counter = 1
do
  
  ! Compute the wavelength
  Wave = Wave_Min_Proc - Sim_Wave_Margin + ( Pair_Counter - 1 ) * Sim_Angstrom_Spacing
  
  ! Different offset for each star
  Wave = Wave + i
 
  ! Compare to ending wavelength
  if ( Wave > Wave_Max_Proc + Sim_Wave_Margin ) goto 1062

  ! Determine the index for triads of values in the CALSPEC flux array
  Wave_Index = nint( Wave - CALSPEC_Wave_Min) + 1

  ! Check against limits of CALSPEC array
  if ( Wave_Index < 2 .or. Wave_Index > CALSPEC_Pairs - 1 ) then
    if ( .false. ) then
      write (0,*) 'Observed wavelength out of range in CALSPEC array'
      write (0,*) 'Wave, Wave_Index, CALSPEC_Wave_Min'
      write (0,*)  Wave, Wave_Index, CALSPEC_Wave_Min
      stop
    endif
    ! Skip to next wavelength
    goto 1061
  endif

  ! Sum over the half-width ( Sim_Angstrom_Spacing / 2, note that this is not normalized )
  Flux = 0.0
  do j = -Half_Width, Half_Width
    Flux = Flux + CALSPEC_Flux(CALSPEC_Number,Wave_Index + i)
  enddo

  ! Adjust for the exposure duration
  Flux = Flux * Exposure

  ! Adjust for the sensitity
  Flux = Flux * ( 1.0 - abs ( (  Wave - Sim_Sens_Wave_Zero ) / Sim_Sens_Wave_Zero ) )
 
  ! Compute the extinction factor
  Call Extinction (Wave, AM, Sim_Ext_Const, Extinction_Fraction)

  ! Adjust for extinction
  Flux = Flux * Extinction_Fraction
  
  ! Output the wavelength/flux pair
  ! write (4,*) Wave, Flux
  ! write (0,*) 'Flux = ', Flux
  ! stop
  
  ! Assign to array
  Wave_Array(Pair_Counter) = Wave
  Flux_Array(Pair_Counter) = Flux
    
  ! Increment counter
  Pair_Counter = Pair_Counter + 1
  
  ! Skip to here if wavelength is out of range
  1061 continue
    
enddo
1062 continue

! Adjust the pair counter
Pair_Counter = Pair_Counter - 1

! Finish
return
end


subroutine Linear_Fit (N, X, Y, Intercept, Slope, Intercept_Uncertainty, Slope_Uncertainty)
! Find the intercept and slope and the uncertainties of a best fit line to arrays of Xs and Ys
! From Bevington

implicit none
integer                     N                                                   ! Number of pairs
double precision            X(*)                                                ! Air masses
double precision            Y(*)                                                ! Magnitudes
double precision            sumX, sumX2, sumY, sumY2, sumXY, delta, varnce      ! Intermediate values
double precision            Intercept, Slope                                    ! Intercept and slope of the best fit line
double precision            Intercept_Uncertainty, Slope_Uncertainty            ! Uncertainties of intercept and slope of the best fit line
integer                     i                                                   ! Loop index

sumX  = 0.
sumY  = 0.
sumX2 = 0.
sumY2 = 0.
sumXY = 0.

do i= 1, N
  sumX  = sumX  + x(i)
  sumY  = sumY  + y(i)
  sumX2 = sumX2 + x(i)**2
  sumY2 = sumY2 + y(i)**2
  sumXY = sumXY + x(i)*y(i)
enddo
delta = N * sumX2 - sumX * sumX
Intercept = (sumX2*sumY  - sumX*sumXY) / delta
Slope     = (sumXY*N - sumX*sumY) / delta
varnce = (sumY2 + Intercept**2 * N + Slope**2 * sumX2 - 2.0 * (Intercept*SumY + Slope*SumXY - Intercept*Slope*sumX) ) / (N - 2.0)
Intercept_Uncertainty = dsqrt ( varnce * sumX2 / delta )
Slope_Uncertainty = dsqrt ( varnce * N / delta )

! Finish
return
end


subroutine Input_Raw_Spectrum ( i, Wave, Flux, Wave_Min_Proc, Wave_Max_Proc, Spectra_Records, Pavlov_Format )
! Input the raw wavelengths and fluxes

implicit none
include "AbsFlux_Parameters.f90"
double precision            Wave(Spectra_Max,Spectra_Pairs)                  ! Wavelength array, as observed
double precision            Flux(Spectra_Max,Spectra_Pairs)                  ! Raw flux array, as observed
double precision            Wave_Min_Proc, Wave_Max_Proc                     ! Minimum and maximum wavelengths to process
integer                     Spectra_Records(Spectra_Max)                     ! Number of records read for this star
integer                     i, j                                             ! index and counter
integer                     io_flag                                          ! IO error number
character*50                string
logical                     Pavlov_Format                                    ! Special format for input file

! If this is a Pavlov format file, read through the header lines beginning with '#'
do
    read (4,*,iostat=io_flag) string
    ! write (0,*) string
    if ( string(1:1) .ne. '#' ) goto 1249
enddo
1249 continue

! Initialize a counter
j = 1

! Loop over records
do
    
  ! Read a wavelength/flux record
  read (4,*,iostat=io_flag) Wave(i,j), Flux(i,j)
  ! if ( int (j/100)*100 .eq. j ) write (0,*) Wave(i,j), Flux(i,j)
  
  ! Check for end of file
  if ( io_flag .ne. 0 ) goto 1948
    
  ! First pair: check that wavelength is not greater than the user supplied minimum
  if ( j == 1 .and. Wave(i,j) > Wave_Min_Proc ) then
    write (0,*) 'First wavelength is greater than the user supplied minimum'
    write (0,*) 'Wave_Min_Proc, Wave(i,j) = ', Wave_Min_Proc, Wave(i,j)
    stop
  endif
  
  ! Not end of file
  goto 1853

  ! End of file
  1948 continue
    
  ! Last pair: check that wavelength is not less than the user supplied maximum
  if ( Wave(i,j-1) < Wave_Max_Proc ) then
    write (0,*) 'Last wavelength is less than the user supplied maximum'
    write (0,*) 'Wave_Max_Proc, Wave = ', Wave_Max_Proc, Wave(i,j-1)
    stop
  else
    goto 1949
  endif
    
  ! Increment the counter
  1853 continue
  j = j + 1

enddo  
  
! Assign the number of records for this star
1949 Spectra_Records(i) = j - 1
! write (0,*) 'i, Spectra_Records(i) = ', i, Spectra_Records(i)

! Finish
return
end


subroutine Build_Path_File_Name ( Path_Name, File_Name, Path_File_Name, Blank100 )
! Build the path+file name of the standard star file

implicit none
character*100           Path_Name                                   ! Path + file name for the file
character*100           File_Name                                   ! Path + file name for the file
character*100           Path_File_Name                              ! Path + file name for the file
character*100           Blank100                                    ! A string of 100 blank characters
integer                 Path_Length                                 ! Length of path name
integer                 File_Length                                 ! Length of file name
integer                 i                                           ! Loop index

! Determine the lengths of the path and file
Path_Length = index(Path_Name,' ')
File_Length = index(File_Name,' ')

! Determine the lengths of the path and file (cannot use index function because of blanks)
do i = 1, 100
  if ( Path_Name(i:i) .ne. ' ' ) Path_Length = i + 1
  if ( File_Name(i:i) .ne. ' ' ) File_Length = i + 1
enddo

! Diagnostic
if ( .false. ) then
  write (0,*) Path_Length, File_Length
  stop
endif

! Initialize the path+file name to all blanks  
Path_File_Name = Blank100

! Build the path+file name
Path_File_Name(1:Path_Length) = Path_Name(1:Path_Length)
Path_File_Name(Path_Length:Path_Length+File_Length-1) = File_Name(1:File_Length-1)

! Diagnostic
if ( .false. ) then
  write (0,*) 'Path_Name       = ', Path_Name
  write (0,*) 'Path_Length     = ', Path_Length
  write (0,*) 'File_Name       = ', File_Name
  write (0,*) 'File_Length     = ', File_Length
  write (0,*) 'Path_File_Name  = ', Path_File_Name
  stop
endif
  
! Finish
return
end


subroutine ID_CALSPEC_Star (CALSPEC_Stars, CALSPEC_Star, File, CALSPEC_Number, Pavlov_Format)
! Identify the CALSPEC star corresponding to a standard star (or a program star for simulation)

implicit none
include "AbsFlux_Parameters.f90"
character*10            CALSPEC_Star(Spectra_Max)                        ! Names of the CALSPEC stars
integer                 CALSPEC_Stars                                    ! Number of CALSPEC stars
character*100           File                                             ! Names of star spectrum file
integer                 CALSPEC_Number                                   ! Number of the CALSPEC star corresponding with the standard star (or a program star for simulation)
integer                 j, k                                             ! Loop indices
logical                 Pavlov_Format                                    ! Special format for input file

! Loop over the CALSPEC star names
do j = 1, CALSPEC_Stars
  ! Loop over the 10 characters in a CALSPEC star name
  do k = 1, 10
    if ( Pavlov_Format ) then
      if ( CALSPEC_Star(j)(k:k) .ne. File(k+22:k+22) ) goto 1842
    else
      if ( CALSPEC_Star(j)(k:k) .ne. File(k:k) ) goto 1842
    endif
    if ( k == 10 ) then
      ! write (0,*) CALSPEC_Star(j), Spectra_Standard_File(i)
      CALSPEC_Number = j
    endif
  enddo
 1842 continue
enddo

! Diagnostic
if (.false.) then
  write (0,*) 'CalSpec_Number = ', CalSpec_Number
  stop
endif

! Finish
return
end


! File unit numbers
!  1 - Namelist input 'AbsFlux_Input.txt'
!  2 - CALSPEC master file 'AbsFluxCALSPECstars.txt'
!  3 - CALSPEC spectra input
!  4 - Standard star spectra input (also output for simulation mode)
!  5 - Program star spectra input (also output for simulation mode)
!  6 - General output
!  7 - Extinction function output
!  8 - Sensitivity function output
!  9 - Program star calibrated output
! 10 - Standard star evaluation profiles
! 11 - Program star evaluation profiles


! Subroutines:

! subroutine LaGrange ( Target_Wave_Interp, Target_Flux_Interp, Input_Wave_Interp, Input_Flux_Interp )
! LaGrangian interpolation in the input fluxes to the target wavelength

! subroutine Precess (JD, RA_in, DC_in, RA_out, DC_out, Pi)
! Precess from J2000 to current time

! subroutine CalcJD(Yr, Mo, Da, JD)
! Calcaulte JD at 0h UT from year, month and day

! subroutine LatLong (Latitude, Longitude, Lat, Lng, Rad2Deg)
! Convert latitude and longitude strings to radians

! subroutine AirMass (Lat, Lng, ST0_site, Time_String, RA, DC, Pi, AM, Advisory_Flag, Advisory_String)
! Compute the air mass of the star

! subroutine Extinction (Wavelength, Airmass, Sim_Ext_Const, Brightness_Fraction)
! Compute the extinction factor

! subroutine Simulate (i, CALSPEC_Flux, Exposure, AM, CALSPEC_Number, Wave_Min_Proc, Wave_Max_Proc, ...
! Simulate and output wavelengths and fluxes

! subroutine Linear_Fit (N, X, Y, Intercept, Slope)
! Find the slope of a best fit line to arrays of Xs and Ys

! subroutine Input_Raw_Spectrum ( i, Wave, Flux, Wave_Min_Proc, Wave_Max_Proc, Spectra_Records )
! Input the raw wavelengths and fluxes

! subroutine Build_Path_File_Name ( Path_Name, File_Name, Path_File_Name, Blank100 )
! Build the path+file name of the standard star file


! Sections of the program:
! 1: Initialize
! 2: Input Wavelengths and Fluxes from CALSPEC files
! 3: Simulate Standard Stars (optional)
! 4: Simulate Program Stars (optional)
! 5: Derive Extinction and Sensitivity Functions from Standard Stars
! 6: Apply Calibration to Program Stars
! 7: Assess the Standard Star Calibrations
! 8: Assess the Program Star Calibrations (for simulation only)
! 9: Display Advisory Messages
