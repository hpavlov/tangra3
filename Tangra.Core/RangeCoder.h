/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once
#include "Compressor.h"

int RangeCompress(const unsigned short * src, void * dest,int length, EncoderPair * encoder_table);
int RangeDecompress(const void * source, unsigned short * dest,int length, DecoderPair * decoder_table, const int * const hash );

