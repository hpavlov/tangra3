#include "ProbabilityCoder.h"
#include <assert.h>

#define WRITE_BIT(x) { \
	unsigned int pos = bitpos>>3;\
	unsigned int rem = bitpos&7;\
	if ( rem==0){\
		stream[pos]=0;\
	}\
	if ( (x) !=0 ){\
		stream[pos]|=1<<rem;\
	}\
	bitpos++;\
}

ProbabilityCoder::ProbabilityCoder(void * buffer, int start_bits){
	bitpos=0;
	stream = (unsigned char *)buffer;
	max_val=1<<(start_bits-1);
}

unsigned int ProbabilityCoder::GetBytesUsed(){
	return bitpos/8+((bitpos&7)>0);
}

void ProbabilityCoder::WriteSymbol(unsigned int symbol){
	if ( max_val <= 1 ){
		assert(symbol == 1);
		return;
	}
	unsigned int new_max=0;
	for ( unsigned int a=1;a<=max_val;a<<=1){
		WRITE_BIT(symbol&a);
		if ( symbol&a ){
			new_max = a;
		}
	}
	max_val = new_max;
}

#define READ_BIT(x) {\
	unsigned int pos = bitpos>>3;\
	unsigned int rem = bitpos&7;\
	bitpos++;\
	(x) = (stream[pos]&(1<<rem))!=0;\
}

unsigned int ProbabilityCoder::ReadSymbol(){
	if ( max_val <= 1 )
		return 1;
	unsigned int symbol=0;
	unsigned int new_max=0;
	for ( unsigned int a=1;a<=max_val;a<<=1){
		bool bit;
		READ_BIT(bit);
		if ( bit ){
			new_max = a;
			symbol += a;
		}
	}
	max_val = new_max;
	return symbol;
}
