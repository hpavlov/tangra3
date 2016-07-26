/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "adv2_frames_index.h"
#include "utils.h"

namespace AdvLib2
{

Adv2FramesIndex::Adv2FramesIndex()
{
	m_MainIndexEntries = new vector<Index2Entry*>();
	m_CalibrationIndexEntries = new vector<Index2Entry*>();
}

Adv2FramesIndex::~Adv2FramesIndex()
{
	m_MainIndexEntries->clear();
	delete m_MainIndexEntries;

	m_CalibrationIndexEntries->clear();
	delete m_CalibrationIndexEntries;
}

Adv2FramesIndex::Adv2FramesIndex(FILE* pFile)
{
	m_MainIndexEntries = new vector<Index2Entry*>();
	m_CalibrationIndexEntries = new vector<Index2Entry*>();

	unsigned char numIndexes;
	advfread(&numIndexes, 1, 1, pFile);

	unsigned int buffOffsetIndex1;
	advfread(&buffOffsetIndex1, 4, 1, pFile);

	unsigned int buffOffsetIndex2;
	advfread(&buffOffsetIndex2, 4, 1, pFile);

	int framesCount;
	advfread(&framesCount, 4, 1, pFile);
	for (int i = 0; i < framesCount; i++)
	{
		__int64 elapsedTicks;
		__int64 frameOffset;
		unsigned int  bytesCount;

		advfread(&elapsedTicks, 8, 1, pFile);
		advfread(&frameOffset, 8, 1, pFile);
		advfread(&bytesCount, 4, 1, pFile);

		Index2Entry *entry = new Index2Entry();
		entry->BytesCount = bytesCount;
		entry->FrameOffset = frameOffset;
		entry->ElapsedTicks = elapsedTicks;
	
		m_MainIndexEntries->push_back(entry);
	}

	advfread(&framesCount, 4, 1, pFile);
	for (int i = 0; i < framesCount; i++)
	{
		__int64 elapsedTicks;
		__int64 frameOffset;
		unsigned int  bytesCount;

		advfread(&elapsedTicks, 8, 1, pFile);
		advfread(&frameOffset, 8, 1, pFile);
		advfread(&bytesCount, 4, 1, pFile);

		Index2Entry *entry = new Index2Entry();
		entry->BytesCount = bytesCount;
		entry->FrameOffset = frameOffset;
		entry->ElapsedTicks = elapsedTicks;
	
		m_CalibrationIndexEntries->push_back(entry);
	}
}

void Adv2FramesIndex::WriteIndex(FILE *file)
{
	unsigned char buffInt8 = 2;
	advfwrite(&buffInt8, 1, 1, file);

	unsigned int buffOffset = 9;
	advfwrite(&buffOffset, 4, 1, file);

	buffOffset = (unsigned int)m_MainIndexEntries->size() * 20 + 10;
	advfwrite(&buffOffset, 4, 1, file);

	unsigned int framesCount = (unsigned int)m_MainIndexEntries->size();
	advfwrite(&framesCount, 4, 1, file);

	vector<Index2Entry*>::iterator curr = m_MainIndexEntries->begin();
	while (curr != m_MainIndexEntries->end()) 
	{
		__int64 elapedTime = (*curr)->ElapsedTicks;
		__int64 frameOffset = (*curr)->FrameOffset;
		unsigned int  bytesCount = (*curr)->BytesCount;
		
		advfwrite(&elapedTime, 8, 1, file);
		advfwrite(&frameOffset, 8, 1, file);
		advfwrite(&bytesCount, 4, 1, file);
		
		curr++;
	}


	framesCount = (unsigned int)m_CalibrationIndexEntries->size();
	advfwrite(&framesCount, 4, 1, file);

	curr = m_CalibrationIndexEntries->begin();
	while (curr != m_CalibrationIndexEntries->end()) 
	{
		__int64 elapedTime = (*curr)->ElapsedTicks;
		__int64 frameOffset = (*curr)->FrameOffset;
		unsigned int  bytesCount = (*curr)->BytesCount;
		
		advfwrite(&elapedTime, 8, 1, file);
		advfwrite(&frameOffset, 8, 1, file);
		advfwrite(&bytesCount, 4, 1, file);
		
		curr++;
	}
}

unsigned int Adv2FramesIndex::GetFramesCount(unsigned char streamId)
{
	if (streamId == 0)
		return (unsigned int)m_MainIndexEntries->size();
	else
		return (unsigned int)m_CalibrationIndexEntries->size();
}


void Adv2FramesIndex::AddFrame(unsigned char streamId, unsigned int frameNo, __int64 elapsedTicks, __int64 frameOffset, unsigned int  bytesCount)
{
	Index2Entry *entry = new Index2Entry();
	entry->BytesCount = bytesCount;
	entry->FrameOffset = frameOffset;
	entry->ElapsedTicks = elapsedTicks;
	
	if (streamId == 0)
		m_MainIndexEntries->push_back(entry);
	else
		m_CalibrationIndexEntries->push_back(entry);
}

Index2Entry* Adv2FramesIndex::GetIndexForFrame(unsigned char streamId, unsigned int frameNo)
{
	Index2Entry* indexEntryAtPos = streamId == 0 
		? m_MainIndexEntries->at(frameNo)
		: m_CalibrationIndexEntries->at(frameNo);

	return indexEntryAtPos;
}

}