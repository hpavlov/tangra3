#include "RangeCoder.h"
#include "Compressor.h"
#include <assert.h>

#define TOP_VALUE		0x80000000
#define BOTTOM_VALUE	0x00800000
#define SHIFT_BITS		23
#define SHIFT			FRACTIONAL_BITS

int RangeCompress(const unsigned short * src, void * dest,int length, EncoderPair * encoder_table){

	unsigned int low = 0;
    unsigned int range = TOP_VALUE;
	const unsigned short * const end = src+length;
	unsigned char * dst = (unsigned char *)dest;

	do {
		// encode the current symbol
		range >>= SHIFT;
		unsigned int i = *src++;
		low += range * encoder_table[i].cprobability;
		range *= encoder_table[i].probability;
		
		// if the range is too small, we need to output bytes
		if( range <= BOTTOM_VALUE ) {
			// check if there was an overflow that needs to be carried to outputted bytes
			if ( low & TOP_VALUE ){
				int pos=-1;
				while (dst[pos]==255){
					dst[pos]=0;
					pos--;
				}
				dst[pos]++;
			}
			// output bytes until range is large enough
			do {
				range <<= 8;
				assert(range != 0);
				*dst = low>>SHIFT_BITS;
				low <<= 8;
				low &= (TOP_VALUE-1);
				dst++;
			} while (range <= BOTTOM_VALUE);
		}
	} while ( src<end );

	// flush the encoder
	if ( low & TOP_VALUE ){
		int pos=-1;
		while (dst[pos]==255){
			dst[pos]=0;
			pos--;
		}
		dst[pos]++;
	}
	dst[0] = low>>23;
	dst[1] = low>>15;
	dst[2] = low>>7;
	dst[3] = low<<1;
	dst+=4;
	
	// Encoder output is shifted 1 bit with respect to the decoder.
	unsigned char * ending = dst;
	dst = (unsigned char *)dest;
	unsigned int prev=0;
	while ( dst < ending ){
		unsigned int temp = *dst;
		unsigned int bit = temp&1;
		temp += prev;
		temp>>=1;
		*dst=temp;
		dst++;
		prev = bit<<8;
	};

	return ending-(unsigned char *)dest;
}

int RangeDecompress( const void * source, unsigned short * dest,int length, DecoderPair * decoder_table, const int * const hash ){
	

	// Create a lookup table to speed up decoding.
	// Looking up a starting position for a linear search based on the top
	// bits of the search value is faster than doing a binary search
	const int lookup_size=4096;
	const int lookup_shift = SHIFT - 12;
	unsigned short lookup[lookup_size];
	int h=1;
	for ( int a=0;a<lookup_size;a++){
		for ( ; decoder_table[h].cprobability<=(a<<lookup_shift);h++);
		lookup[a]=h-1;
	}

	// initialize the decoder variables
	const unsigned char * src = (const unsigned char *)source;
	unsigned short * ending = dest+length;
	unsigned int range=TOP_VALUE;
	unsigned int low=(src[0]<<24)+(src[1]<<16)+(src[2]<<8)+src[3];
	src+=4;
	unsigned short * dst = dest;

	while ( dst < ending ){
		// if the range gets too small, read in bytes
		while ( range <= BOTTOM_VALUE){
			range <<= 8;
			assert( range != 0 );
			low <<= 8;
			low += src[0];
			src++;
		}

		// decode the current symbol
		unsigned int help = range >> SHIFT;
		int x =lookup[low/help>>lookup_shift];
		for ( ; decoder_table[x+1].cprobability*help<=low;x++){};
		low -= decoder_table[x].cprobability*help;
		*dst++=decoder_table[x].decoded_value;
		range = (decoder_table[x+1].cprobability-decoder_table[x].cprobability)*help;
	}

	return true;

}