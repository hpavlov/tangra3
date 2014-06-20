
#pragma once

/*
This class stores or loads probability values which must be in decending order. Each value is required to
be less than or equal to the preceeding value. The max number of bits per value is entered in the
constructor, and this value is decreased when ever a symbol is written or read that doesn't use
all the bits.
*/
class ProbabilityCoder{
private:
	unsigned int bitpos;
	unsigned char * stream;
	unsigned int max_val;
public:
	ProbabilityCoder(void * buffer, int start_bits);
	unsigned int GetBytesUsed();
	void WriteSymbol(unsigned int symbol);
	unsigned int ReadSymbol();
};

