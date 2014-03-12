#ifndef ADV_PROFILING
#define ADV_PROFILING

#define _FILE_OFFSET_BITS  64
#include "stdio.h"

#if _MSC_VER
#define snprintf _snprintf
#endif

extern long ADVRPF_COMPRESSION_MICROSECS;
extern int ADVRPF_PROCESSED_FRAMES;

extern long long ADVRPF_HDDWRITE_BYTES_WRITTEN;

extern long ADVRPF_TOTAL_PROCESSING_MICROSECS;
extern long ADVRPF_TOTAL_ADV_PROCESSING_MICROSECS;
extern long ADVRPF_HDD_OPERATION_MICROSECS;
extern long ADVRPF_BYTE_PROCESSING_MICROSECS;
extern long ADVRPF_MEMORY_ALLOCATION_MICROSECS;
extern long ADVRPF_TESTING_MICROSECS;

//extern ptime ADVRPF_PROFILING_STARTED_TIMESTAMP;
extern long long ADVRPF_PROFILING_STARTED_TIMESTAMP;

void AdvProfiling_ResetPerformanceCounters();

void AdvProfiling_NewFrameProcessed();

void AdvProfiling_StartFrameCompression();
void AdvProfiling_EndFrameCompression();

void AdvProfiling_StartGenericProcessing();
void AdvProfiling_EndGenericProcessing();

void AdvProfiling_StartHddOperation();
void AdvProfiling_EndHddOperation();

void AdvProfiling_StartBytesOperation();
void AdvProfiling_EndBytesOperation();

void AdvProfiling_StartMemoryAllocation();
void AdvProfiling_EndMemoryAllocation();

void AdvProfiling_StartTestingOperation();
void AdvProfiling_EndTestingOperation();

void AdvProfiling_StartProcessing();
void AdvProfiling_EndProcessing();

void perfcounters_microsec_ticks_start();
long perfcounters_microsec_ticks_stop();

FILE* advfopen(const char* fileName, const char* modes);
size_t advfwrite(const void* pData, size_t size, size_t count, FILE* file);
void advfgetpos64(FILE* file, __int64* pos);
int advfsetpos64(FILE* file, const __int64* pos);
int advfsetpos64(FILE* file, const __int64* pos, int origin);
int advfseek(FILE* stream, __int64 off, int whence);
int advfclose(FILE* file);
int advfflush(FILE* file);

#endif