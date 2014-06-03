/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "adv_frames_index.h"
#include "stdio.h"

namespace AdvLib
{
	
AdvFramesIndex::AdvFramesIndex()
{
	m_IndexEntries = new vector<IndexEntry*>();
}

AdvFramesIndex::~AdvFramesIndex()
{
	m_IndexEntries->clear();
	delete m_IndexEntries;
}

void AdvFramesIndex::AddFrame(unsigned int frameNo, unsigned int elapedTime, __int64 frameOffset, unsigned int  bytesCount)
{
	IndexEntry *entry = new IndexEntry();
	entry->BytesCount = bytesCount;
	entry->FrameOffset = frameOffset;
	entry->ElapsedTime = elapedTime;
	
	m_IndexEntries->push_back(entry);
}

void AdvFramesIndex::WriteIndex(FILE *pFile)
{
	unsigned int framesCount = m_IndexEntries->size();
	fwrite(&framesCount, 4, 1, pFile);

	vector<IndexEntry*>::iterator curr = m_IndexEntries->begin();
	while (curr != m_IndexEntries->end()) 
	{
		unsigned int elapedTime = (*curr)->ElapsedTime;
		__int64 frameOffset = (*curr)->FrameOffset;
		unsigned int  bytesCount = (*curr)->BytesCount;
		
		fwrite(&elapedTime, 4, 1, pFile);
		fwrite(&frameOffset, 8, 1, pFile);
		fwrite(&bytesCount, 4, 1, pFile);
		
		curr++;
	}
}

}

