/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#define FRACTIONAL_BITS 20

struct EncoderPair{
	int probability; // Symbol probability * (1<<FRACTIONAL_BITS)
	int cprobability; // Cumulative probability of all more frequent symbols * (1<<FRACTIONAL_BITS)
};
struct DecoderPair{
	int decoded_value; // Symbol that this entry decodes to
	int cprobability; // This will be probability when used by encoder, cumulative probability when used by decoder
};

#ifndef _SIZE_T_DEFINED
#ifdef  _WIN64
typedef unsigned __int64    size_t;
#else
typedef unsigned int   size_t;
#endif
#define _SIZE_T_DEFINED
#endif

template <typename T, size_t size>
T* begin(T (&c)[size])
{
    return &c[0];
}

template <typename T, size_t size>
T* end(T (&c)[size])
{
    return &c[0] + size;
}

class Compressor{
private:
	int width;
	int height;
	int table_entries;
	int frequencies[0x10000]; // need (max possible number of symbols) entries
	EncoderPair encoder_table[0x10000]; // need (max possible number of symbols) entries
	DecoderPair decoder_table[0x10001]; // need (max possible number of symbols + 1) entries

	void PrepareTables(unsigned short * uncompressed, int uncompressed_symbol_count);
	int StoreDecompressionTable(void * compressed);
	int LoadDecompressionTable(void * compressed);
	
public:
	// frame_width and frame_height must be > 0
	Compressor(int frame_width, int frame_height);
	
	/*
	Compressed buffer must be frame_width*frame_height*sizeof(short) + 0x20000 bytes 
	to ensure it cannot be overrun with pathalogical input.
	Returns the number of bytes written to the compressed buffer.
	*/
	int CompressData(unsigned short * uncompressed, void * compressed);
	
	// Returns the number of bytes used in the compressed buffer, or a
	// negative value if an error occurred
	int DecompressData(void * compressed, unsigned short * uncompressed);
};
