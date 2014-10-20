/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "Compressor.h"
#include <memory.h>
#include <algorithm>
#include <iterator>
#include "ProbabilityCoder.h"
#include "RangeCoder.h"
#include <assert.h>

Compressor::Compressor(int frame_width, int frame_height){
	assert(frame_width>0);
	assert(frame_height>0);
	width = frame_width;
	height = frame_height;
}

bool compare(const DecoderPair &a, const DecoderPair &b){
	return a.cprobability>b.cprobability;
}

void Compressor::PrepareTables(unsigned short * uncompressed, int uncompressed_symbol_count){
	memset(frequencies,0,sizeof(frequencies));
	memset(decoder_table,0,sizeof(decoder_table));
	table_entries=0;
	
	// get symbol frequencies
	for ( int a=0;a<uncompressed_symbol_count;a++){
		frequencies[uncompressed[a]]++;
	}

	// compact the table (remove entries with 0 frequency) and scale values so that that the 
	// probability entry represents a fraction with 1<<FRACTIONAL_BITS as denominator. cprobability will
	// store the symbol probability for now.
	int total = uncompressed_symbol_count;
	int nt=0;
	for ( int a=0;a<0x10000;a++){
		if ( frequencies[a] ){
			double ll = frequencies[a];
			ll*=1<<FRACTIONAL_BITS;
			int v = (int)(ll/total+0.5);
			if ( v == 0 ){
				v=1;
			}
			nt += v;
			decoder_table[table_entries].cprobability = v;
			decoder_table[table_entries].decoded_value = a;
			table_entries++;
		}
	}

	// sort the values so they are arranged from most frequent to least
	std::sort(begin(decoder_table),end(decoder_table),compare);

	// correct rounding errors in the probabilty scaling
	total = 1<<FRACTIONAL_BITS;
	if ( nt > total ){
		int pos = table_entries-1;
		do {
			while (decoder_table[pos].cprobability==1){ pos--; }
			for ( int a=pos; nt>total && a>=0;a--){
				decoder_table[a].cprobability--;
				nt--;
			}
		} while ( nt > total );
	} else {
		do {
			for ( int a=0;a<table_entries && nt != total;a++){
				decoder_table[a].cprobability++;
				nt++;
			}
		} while ( nt < total );
	}

	// set up the encoder table from the decoder table
	int low=0;
	memset(encoder_table,0,sizeof(encoder_table));
	for ( int a=0;a<table_entries;a++){
		int range = decoder_table[a].cprobability;
		int i = decoder_table[a].decoded_value;
		encoder_table[i].cprobability=low;
		encoder_table[i].probability=range;
		low+=range;			
	}
}

inline void WriteShort(void * dest,unsigned short val){
	unsigned char * d = (unsigned char *)dest;
	d[0]=(unsigned char)val;
	d[1]=(unsigned char)(val>>8);
}

inline unsigned short ReadShort(void * src){
	unsigned char * s = (unsigned char *)src;
	return s[0]+(s[1]<<8);
}

int Compressor::StoreDecompressionTable(void * comp){
	unsigned short * compressed = (unsigned short *)comp;
	
	// store number of table entries, must be at least 1
	WriteShort(compressed,table_entries-1);

	// store the values the entries decode to
	for ( int a=0;a<table_entries;a++){
		WriteShort(compressed+a+1,decoder_table[a].decoded_value);
	}
	if ( table_entries == 1){
		return 2*sizeof(unsigned short);
	}

	// store the probability of each entry
	ProbabilityCoder prob(&compressed[table_entries+1],FRACTIONAL_BITS);
	for ( int a=0;a<table_entries;a++){
		prob.WriteSymbol(decoder_table[a].cprobability);
	}

	return (table_entries+1)*sizeof(unsigned short)+prob.GetBytesUsed();
}

int Compressor::LoadDecompressionTable(void * comp){
	unsigned short * compressed = (unsigned short *)comp;
	
	// load number of table entries 
	table_entries = ReadShort(compressed)+1;

	// load the values the entries decode to
	for ( int a=0;a<table_entries;a++){
		decoder_table[a].decoded_value = ReadShort(compressed+a+1);
	}
	if ( table_entries == 1){
		return 2*sizeof(unsigned short);
	}

	// store the probability of each entry
	ProbabilityCoder prob(&compressed[table_entries+1],FRACTIONAL_BITS);
	int cp=0;
	for (int a=0;a<table_entries;a++){
		decoder_table[a].cprobability = cp;
		int v = prob.ReadSymbol();
		assert(v);
		if ( v == 0 )
			return -1;
		cp += v;
	}

	assert(cp==(1<<FRACTIONAL_BITS));
	if ( cp != (1<<FRACTIONAL_BITS)){
		return -1;
	}
	decoder_table[table_entries].cprobability = 1<<FRACTIONAL_BITS;
	return (table_entries+1)*sizeof(unsigned short)+prob.GetBytesUsed();
}

int Compressor::CompressData(unsigned short * uncompressed, void * compressed){
	
	int compressed_size = 0;
	PrepareTables(uncompressed,width*height);
	compressed_size = StoreDecompressionTable(compressed);
	if ( table_entries > 1){
		compressed_size += RangeCompress(uncompressed,((unsigned char*)compressed)+compressed_size,width*height,encoder_table);
	}

	// If the data didn't compress, store the raw data
	if ( compressed_size >= width*height*sizeof(unsigned short)){ 
		unsigned char * temp = (unsigned char *)compressed;
		// This is an special case invalid header that indicates the data could not be compressed
		memset(temp,0,8);
		memcpy(temp+8,uncompressed,width*height*sizeof(unsigned short));

		compressed_size = 8+width*height*sizeof(unsigned short);
	}

	return compressed_size;
}

int Compressor::DecompressData(void * compressed,unsigned short * uncompressed){

	{ // Check for special case header indicating the data could not be compressed
		unsigned char * temp = (unsigned char *)compressed;
		int a=0;
		for ( ;a<8 && temp[a]==0; a++){};
		if ( a == 8 ){
			memcpy(uncompressed,temp+8,width*height*sizeof(unsigned short));
			return 8+width*height*sizeof(unsigned short);
		}
		
	}
 
	int compressed_size = LoadDecompressionTable(compressed);
	if ( compressed_size > 0 ){
		if ( table_entries > 1 ){
			compressed_size += RangeDecompress(((unsigned char*)compressed)+compressed_size,uncompressed,width*height,decoder_table,NULL);
		} else {
			const int v = decoder_table[0].decoded_value;
			for ( int a=0;a<width*height;a++){
				uncompressed[a]=v;
			}
		}
	}
	return compressed_size;
}
