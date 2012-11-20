##
## Auto Generated makefile by CodeLite IDE
## any manual changes will be erased      
##
## Debug
ProjectName            :=TangraCore
ConfigurationName      :=Debug
WorkspacePath          := "C:\Hristo\Tangra 3\Tangra.Core"
ProjectPath            := "C:\Hristo\Tangra 3\Tangra.Core"
IntermediateDirectory  :=./Debug
OutDir                 := $(IntermediateDirectory)
CurrentFileName        :=
CurrentFilePath        :=
CurrentFileFullPath    :=
User                   :=hpavlov
Date                   :=20/11/2012
CodeLitePath           :="C:\Program Files (x86)\CodeLite"
LinkerName             :=g++
SharedObjectLinkerName :=g++ -shared -fPIC
ObjectSuffix           :=.o
DependSuffix           :=.o.d
PreprocessSuffix       :=.o.i
DebugSwitch            :=-gstab
IncludeSwitch          :=-I
LibrarySwitch          :=-l
OutputSwitch           :=-o 
LibraryPathSwitch      :=-L
PreprocessorSwitch     :=-D
SourceSwitch           :=-c 
OutputFile             :=$(IntermediateDirectory)/$(ProjectName).dll
Preprocessors          :=
ObjectSwitch           :=-o 
ArchiveOutputSwitch    := 
PreprocessOnlySwitch   :=-E 
ObjectsFileList        :="C:\Hristo\Tangra 3\Tangra.Core\TangraCore.txt"
PCHCompileFlags        :=
MakeDirCommand         :=makedir
LinkOptions            :=  Tangra.Core.def -Wl,--subsystem,windows,--out-implib,./Debug/TangraCoredll.lib,--enable-stdcall-fixup
IncludePath            :=  $(IncludeSwitch). $(IncludeSwitch). 
IncludePCH             := 
RcIncludePath          := 
Libs                   := 
ArLibs                 :=  
LibPath                := $(LibraryPathSwitch). 

##
## Common variables
## AR, CXX, CC, CXXFLAGS and CFLAGS can be overriden using an environment variables
##
AR       := ar rcus
CXX      := g++
CC       := gcc
CXXFLAGS :=  -g -D_FILE_OFFSET_BITS=64 $(Preprocessors)
CFLAGS   :=  -g -D_FILE_OFFSET_BITS=64 $(Preprocessors)


##
## User defined environment variables
##
CodeLiteDir:=C:\Program Files (x86)\CodeLite
UNIT_TEST_PP_SRC_DIR:=C:\Program Files (x86)\UnitTest++-1.3
PATH:=%PATH%;c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE;c:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin
Objects=$(IntermediateDirectory)/adv_file$(ObjectSuffix) $(IntermediateDirectory)/adv_frames_index$(ObjectSuffix) $(IntermediateDirectory)/adv_image_layout$(ObjectSuffix) $(IntermediateDirectory)/adv_image_section$(ObjectSuffix) $(IntermediateDirectory)/adv_status_section$(ObjectSuffix) $(IntermediateDirectory)/IntegrationUtils$(ObjectSuffix) $(IntermediateDirectory)/IotaVtiOcr$(ObjectSuffix) $(IntermediateDirectory)/PixelMapUtils$(ObjectSuffix) $(IntermediateDirectory)/PreProcessing$(ObjectSuffix) $(IntermediateDirectory)/quicklz$(ObjectSuffix) \
	$(IntermediateDirectory)/Tangra.Orc.IotaVti$(ObjectSuffix) $(IntermediateDirectory)/Tangra.Pixelmap$(ObjectSuffix) $(IntermediateDirectory)/TangraADV$(ObjectSuffix) $(IntermediateDirectory)/utils$(ObjectSuffix) 

##
## Main Build Targets 
##
.PHONY: all clean PreBuild PrePreBuild PostBuild
all: $(OutputFile)

$(OutputFile): $(IntermediateDirectory)/.d $(Objects) 
	@$(MakeDirCommand) $(@D)
	@echo "" > $(IntermediateDirectory)/.d
	@echo $(Objects) > $(ObjectsFileList)
	$(SharedObjectLinkerName) $(OutputSwitch)$(OutputFile) @$(ObjectsFileList) $(LibPath) $(Libs) $(LinkOptions)
	@$(MakeDirCommand) "C:\Hristo\Tangra 3\Tangra.Core\.build-debug"
	@echo rebuilt > "C:\Hristo\Tangra 3\Tangra.Core\.build-debug\TangraCore"

$(IntermediateDirectory)/.d:
	@$(MakeDirCommand) "./Debug"

PreBuild:


##
## Objects
##
$(IntermediateDirectory)/adv_file$(ObjectSuffix): adv_file.cpp $(IntermediateDirectory)/adv_file$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/adv_file.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/adv_file$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/adv_file$(DependSuffix): adv_file.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/adv_file$(ObjectSuffix) -MF$(IntermediateDirectory)/adv_file$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/adv_file.cpp"

$(IntermediateDirectory)/adv_file$(PreprocessSuffix): adv_file.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/adv_file$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/adv_file.cpp"

$(IntermediateDirectory)/adv_frames_index$(ObjectSuffix): adv_frames_index.cpp $(IntermediateDirectory)/adv_frames_index$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/adv_frames_index.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/adv_frames_index$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/adv_frames_index$(DependSuffix): adv_frames_index.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/adv_frames_index$(ObjectSuffix) -MF$(IntermediateDirectory)/adv_frames_index$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/adv_frames_index.cpp"

$(IntermediateDirectory)/adv_frames_index$(PreprocessSuffix): adv_frames_index.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/adv_frames_index$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/adv_frames_index.cpp"

$(IntermediateDirectory)/adv_image_layout$(ObjectSuffix): adv_image_layout.cpp $(IntermediateDirectory)/adv_image_layout$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/adv_image_layout.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/adv_image_layout$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/adv_image_layout$(DependSuffix): adv_image_layout.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/adv_image_layout$(ObjectSuffix) -MF$(IntermediateDirectory)/adv_image_layout$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/adv_image_layout.cpp"

$(IntermediateDirectory)/adv_image_layout$(PreprocessSuffix): adv_image_layout.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/adv_image_layout$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/adv_image_layout.cpp"

$(IntermediateDirectory)/adv_image_section$(ObjectSuffix): adv_image_section.cpp $(IntermediateDirectory)/adv_image_section$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/adv_image_section.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/adv_image_section$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/adv_image_section$(DependSuffix): adv_image_section.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/adv_image_section$(ObjectSuffix) -MF$(IntermediateDirectory)/adv_image_section$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/adv_image_section.cpp"

$(IntermediateDirectory)/adv_image_section$(PreprocessSuffix): adv_image_section.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/adv_image_section$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/adv_image_section.cpp"

$(IntermediateDirectory)/adv_status_section$(ObjectSuffix): adv_status_section.cpp $(IntermediateDirectory)/adv_status_section$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/adv_status_section.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/adv_status_section$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/adv_status_section$(DependSuffix): adv_status_section.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/adv_status_section$(ObjectSuffix) -MF$(IntermediateDirectory)/adv_status_section$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/adv_status_section.cpp"

$(IntermediateDirectory)/adv_status_section$(PreprocessSuffix): adv_status_section.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/adv_status_section$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/adv_status_section.cpp"

$(IntermediateDirectory)/IntegrationUtils$(ObjectSuffix): IntegrationUtils.cpp $(IntermediateDirectory)/IntegrationUtils$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/IntegrationUtils.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/IntegrationUtils$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/IntegrationUtils$(DependSuffix): IntegrationUtils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/IntegrationUtils$(ObjectSuffix) -MF$(IntermediateDirectory)/IntegrationUtils$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/IntegrationUtils.cpp"

$(IntermediateDirectory)/IntegrationUtils$(PreprocessSuffix): IntegrationUtils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/IntegrationUtils$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/IntegrationUtils.cpp"

$(IntermediateDirectory)/IotaVtiOcr$(ObjectSuffix): IotaVtiOcr.cpp $(IntermediateDirectory)/IotaVtiOcr$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/IotaVtiOcr.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/IotaVtiOcr$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/IotaVtiOcr$(DependSuffix): IotaVtiOcr.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/IotaVtiOcr$(ObjectSuffix) -MF$(IntermediateDirectory)/IotaVtiOcr$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/IotaVtiOcr.cpp"

$(IntermediateDirectory)/IotaVtiOcr$(PreprocessSuffix): IotaVtiOcr.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/IotaVtiOcr$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/IotaVtiOcr.cpp"

$(IntermediateDirectory)/PixelMapUtils$(ObjectSuffix): PixelMapUtils.cpp $(IntermediateDirectory)/PixelMapUtils$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/PixelMapUtils.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/PixelMapUtils$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/PixelMapUtils$(DependSuffix): PixelMapUtils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/PixelMapUtils$(ObjectSuffix) -MF$(IntermediateDirectory)/PixelMapUtils$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/PixelMapUtils.cpp"

$(IntermediateDirectory)/PixelMapUtils$(PreprocessSuffix): PixelMapUtils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/PixelMapUtils$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/PixelMapUtils.cpp"

$(IntermediateDirectory)/PreProcessing$(ObjectSuffix): PreProcessing.cpp $(IntermediateDirectory)/PreProcessing$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/PreProcessing.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/PreProcessing$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/PreProcessing$(DependSuffix): PreProcessing.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/PreProcessing$(ObjectSuffix) -MF$(IntermediateDirectory)/PreProcessing$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/PreProcessing.cpp"

$(IntermediateDirectory)/PreProcessing$(PreprocessSuffix): PreProcessing.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/PreProcessing$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/PreProcessing.cpp"

$(IntermediateDirectory)/quicklz$(ObjectSuffix): quicklz.cpp $(IntermediateDirectory)/quicklz$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/quicklz.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/quicklz$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/quicklz$(DependSuffix): quicklz.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/quicklz$(ObjectSuffix) -MF$(IntermediateDirectory)/quicklz$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/quicklz.cpp"

$(IntermediateDirectory)/quicklz$(PreprocessSuffix): quicklz.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/quicklz$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/quicklz.cpp"

$(IntermediateDirectory)/Tangra.Orc.IotaVti$(ObjectSuffix): Tangra.Orc.IotaVti.cpp $(IntermediateDirectory)/Tangra.Orc.IotaVti$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Orc.IotaVti.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Tangra.Orc.IotaVti$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Tangra.Orc.IotaVti$(DependSuffix): Tangra.Orc.IotaVti.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Tangra.Orc.IotaVti$(ObjectSuffix) -MF$(IntermediateDirectory)/Tangra.Orc.IotaVti$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Orc.IotaVti.cpp"

$(IntermediateDirectory)/Tangra.Orc.IotaVti$(PreprocessSuffix): Tangra.Orc.IotaVti.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Tangra.Orc.IotaVti$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Orc.IotaVti.cpp"

$(IntermediateDirectory)/Tangra.Pixelmap$(ObjectSuffix): Tangra.Pixelmap.cpp $(IntermediateDirectory)/Tangra.Pixelmap$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Pixelmap.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Tangra.Pixelmap$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Tangra.Pixelmap$(DependSuffix): Tangra.Pixelmap.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Tangra.Pixelmap$(ObjectSuffix) -MF$(IntermediateDirectory)/Tangra.Pixelmap$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Pixelmap.cpp"

$(IntermediateDirectory)/Tangra.Pixelmap$(PreprocessSuffix): Tangra.Pixelmap.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Tangra.Pixelmap$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/Tangra.Pixelmap.cpp"

$(IntermediateDirectory)/TangraADV$(ObjectSuffix): TangraADV.cpp $(IntermediateDirectory)/TangraADV$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/TangraADV.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/TangraADV$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/TangraADV$(DependSuffix): TangraADV.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/TangraADV$(ObjectSuffix) -MF$(IntermediateDirectory)/TangraADV$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/TangraADV.cpp"

$(IntermediateDirectory)/TangraADV$(PreprocessSuffix): TangraADV.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/TangraADV$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/TangraADV.cpp"

$(IntermediateDirectory)/utils$(ObjectSuffix): utils.cpp $(IntermediateDirectory)/utils$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "C:/Hristo/Tangra 3/Tangra.Core/utils.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/utils$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/utils$(DependSuffix): utils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/utils$(ObjectSuffix) -MF$(IntermediateDirectory)/utils$(DependSuffix) -MM "C:/Hristo/Tangra 3/Tangra.Core/utils.cpp"

$(IntermediateDirectory)/utils$(PreprocessSuffix): utils.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/utils$(PreprocessSuffix) "C:/Hristo/Tangra 3/Tangra.Core/utils.cpp"


-include $(IntermediateDirectory)/*$(DependSuffix)
##
## Clean
##
clean:
	$(RM) $(IntermediateDirectory)/adv_file$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/adv_file$(DependSuffix)
	$(RM) $(IntermediateDirectory)/adv_file$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/adv_frames_index$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/adv_frames_index$(DependSuffix)
	$(RM) $(IntermediateDirectory)/adv_frames_index$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_layout$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_layout$(DependSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_layout$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_section$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_section$(DependSuffix)
	$(RM) $(IntermediateDirectory)/adv_image_section$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/adv_status_section$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/adv_status_section$(DependSuffix)
	$(RM) $(IntermediateDirectory)/adv_status_section$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/IntegrationUtils$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/IntegrationUtils$(DependSuffix)
	$(RM) $(IntermediateDirectory)/IntegrationUtils$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/IotaVtiOcr$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/IotaVtiOcr$(DependSuffix)
	$(RM) $(IntermediateDirectory)/IotaVtiOcr$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/PixelMapUtils$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/PixelMapUtils$(DependSuffix)
	$(RM) $(IntermediateDirectory)/PixelMapUtils$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/PreProcessing$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/PreProcessing$(DependSuffix)
	$(RM) $(IntermediateDirectory)/PreProcessing$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/quicklz$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/quicklz$(DependSuffix)
	$(RM) $(IntermediateDirectory)/quicklz$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Orc.IotaVti$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Orc.IotaVti$(DependSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Orc.IotaVti$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Pixelmap$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Pixelmap$(DependSuffix)
	$(RM) $(IntermediateDirectory)/Tangra.Pixelmap$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/TangraADV$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/TangraADV$(DependSuffix)
	$(RM) $(IntermediateDirectory)/TangraADV$(PreprocessSuffix)
	$(RM) $(IntermediateDirectory)/utils$(ObjectSuffix)
	$(RM) $(IntermediateDirectory)/utils$(DependSuffix)
	$(RM) $(IntermediateDirectory)/utils$(PreprocessSuffix)
	$(RM) $(OutputFile)
	$(RM) $(OutputFile)
	$(RM) "C:\Hristo\Tangra 3\Tangra.Core\.build-debug\TangraCore"


