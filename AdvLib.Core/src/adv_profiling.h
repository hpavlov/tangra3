/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_PROFILING
#define ADV_PROFILING

extern long ADVRPF_COMPRESSION_MICROSECS;
extern int ADVRPF_PROCESSED_FRAMES;

extern long long ADVRPF_HDDWRITE_BYTES_WRITTEN;

extern long ADVRPF_TOTAL_PROCESSING_MICROSECS;
extern long ADVRPF_TOTAL_ADV_PROCESSING_MICROSECS;
extern long ADVRPF_HDD_OPERATION_MICROSECS;
extern long ADVRPF_BYTE_PROCESSING_MICROSECS;
extern long ADVRPF_MEMORY_ALLOCATION_MICROSECS;
extern long ADVRPF_TESTING_MICROSECS;

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

#endif