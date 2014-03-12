#include "StdAfx.h"
#include "adv_profiling.h"

long ADVRPF_COMPRESSION_MICROSECS = 0;
int ADVRPF_PROCESSED_FRAMES = 0;
long long ADVRPF_HDDWRITE_BYTES_WRITTEN = 0;
long ADVRPF_TOTAL_ADV_PROCESSING_MICROSECS = 0;
long ADVRPF_TOTAL_PROCESSING_MICROSECS = 0;
long ADVRPF_HDD_OPERATION_MICROSECS = 0;
long ADVRPF_BYTE_PROCESSING_MICROSECS = 0;
long ADVRPF_MEMORY_ALLOCATION_MICROSECS = 0;
long ADVRPF_TESTING_MICROSECS = 0;
long long ADVRPF_PROFILING_STARTED_TIMESTAMP;

long long t1;

void perfcounters_microsec_ticks_start()
{
   //t1 = microsec_clock::local_time();
};

long perfcounters_microsec_ticks_stop()
{
   //ptime t2(microsec_clock::local_time());
   //time_period tp(t1, t2);
   //return tp.length().total_microseconds();
	return 0;
};

void AdvProfiling_ResetPerformanceCounters()
{
	ADVRPF_COMPRESSION_MICROSECS = 0;
	ADVRPF_PROCESSED_FRAMES = 0;
	ADVRPF_HDDWRITE_BYTES_WRITTEN = 0;
	ADVRPF_TOTAL_ADV_PROCESSING_MICROSECS = 0;
	ADVRPF_HDD_OPERATION_MICROSECS = 0;
	ADVRPF_BYTE_PROCESSING_MICROSECS = 0;
	ADVRPF_MEMORY_ALLOCATION_MICROSECS = 0;
	ADVRPF_TESTING_MICROSECS = 0;
	
	//ADVRPF_PROFILING_STARTED_TIMESTAMP = microsec_clock::local_time();
};

void AdvProfiling_StartFrameCompression()
{
	 perfcounters_microsec_ticks_start();
}

void AdvProfiling_EndFrameCompression()
{	
	ADVRPF_COMPRESSION_MICROSECS += perfcounters_microsec_ticks_stop();
}

void AdvProfiling_NewFrameProcessed()
{
   ADVRPF_PROCESSED_FRAMES++;
   
  // ptime t2(microsec_clock::local_time());   
}

//ptime currGenericTicksStart;

void AdvProfiling_StartGenericProcessing()
{
	//currGenericTicksStart = microsec_clock::local_time();
}

void AdvProfiling_EndGenericProcessing()
{
   //ptime t2(microsec_clock::local_time());   
   //time_period tp(currGenericTicksStart, t2);
   //ADVRPF_TOTAL_ADV_PROCESSING_MICROSECS += tp.length().total_microseconds();
}

//ptime currTotalTicksStart;

void AdvProfiling_StartProcessing()
{
	//currTotalTicksStart = microsec_clock::local_time();
}

void AdvProfiling_EndProcessing()
{
   //ptime t2(microsec_clock::local_time());   
   //time_period tp(currTotalTicksStart, t2);
   //ADVRPF_TOTAL_PROCESSING_MICROSECS += tp.length().total_microseconds();
}

void AdvProfiling_StartHddOperation()
{
	perfcounters_microsec_ticks_start();
}

void AdvProfiling_EndHddOperation()
{
	ADVRPF_HDD_OPERATION_MICROSECS += perfcounters_microsec_ticks_stop();
}

//ptime testingStartTime;

void AdvProfiling_StartTestingOperation()
{
	//testingStartTime = microsec_clock::local_time();
}

void AdvProfiling_EndTestingOperation()
{
   //ptime t2(microsec_clock::local_time());   
   //time_period tp(testingStartTime, t2);
   //ADVRPF_TESTING_MICROSECS += tp.length().total_microseconds();
}

//ptime bytesOperationStartTime;

void AdvProfiling_StartBytesOperation()
{
	//bytesOperationStartTime = microsec_clock::local_time();
}

void AdvProfiling_EndBytesOperation()
{
   //ptime t2(microsec_clock::local_time());   
   //time_period tp(bytesOperationStartTime, t2);
   //ADVRPF_BYTE_PROCESSING_MICROSECS += tp.length().total_microseconds();
}

//ptime memoryAllocationStartTime;

void AdvProfiling_StartMemoryAllocation()
{
	//memoryAllocationStartTime = microsec_clock::local_time();
}

void AdvProfiling_EndMemoryAllocation()
{
   //ptime t2(microsec_clock::local_time());   
   //time_period tp(memoryAllocationStartTime, t2);
   //ADVRPF_MEMORY_ALLOCATION_MICROSECS += tp.length().total_microseconds();	
}

int advfclose(FILE* file)
{
	AdvProfiling_StartHddOperation();	
	fclose(file);
	AdvProfiling_EndHddOperation();
	return 0;
}

FILE* advfopen(const char* fileName, const char* modes)
{
	AdvProfiling_StartHddOperation();
	FILE* file = fopen(fileName, modes);
	AdvProfiling_EndHddOperation();
	return file;
}

size_t advfwrite(const void* pData, size_t size, size_t count, FILE* file)
{
	AdvProfiling_StartHddOperation();
	size_t written = fwrite(pData, size, count, file);
	ADVRPF_HDDWRITE_BYTES_WRITTEN += size * count;
	AdvProfiling_EndHddOperation();
	return written;
}

void advfgetpos64(FILE* file, __int64* pos)
{
	AdvProfiling_StartHddOperation();
	*pos = _ftelli64(file);
	AdvProfiling_EndHddOperation();
}

int advfsetpos64(FILE* file, const __int64* pos, int origin)
{
	AdvProfiling_StartHddOperation();
	int rv = _fseeki64(file, *pos, origin);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfsetpos64(FILE* file, const __int64* pos)
{
	AdvProfiling_StartHddOperation();
	int rv = _fseeki64(file, *pos, SEEK_SET);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfseek(FILE* file, __int64 off, int whence)
{
	AdvProfiling_StartHddOperation();
	int rv = advfsetpos64(file, &off, whence);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfflush(FILE* file)
{
	AdvProfiling_StartHddOperation();
	int rv = fflush(file);
	AdvProfiling_EndHddOperation();
	return rv;
}