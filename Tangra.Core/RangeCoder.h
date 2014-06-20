#pragma once
#include "Compressor.h"

int RangeCompress(const unsigned short * src, void * dest,int length, EncoderPair * encoder_table);
int RangeDecompress(const void * source, unsigned short * dest,int length, DecoderPair * decoder_table, const int * const hash );

