/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
    public class AdvFramesIndexEntry
    {
        public TimeSpan ElapsedTime;
        public long Offset;
        public uint Length;
    }

    public class AdvFramesIndex
    {
        private List<AdvFramesIndexEntry> m_Index = new List<AdvFramesIndexEntry>();

        public uint NumberOfFrames { get { return (uint)m_Index.Count; } }

		public long TableOffset { get; private set; }

	    public IList<AdvFramesIndexEntry> Index
        {
            get { return m_Index; }
        }

        public AdvFramesIndex()
        { }

        private void AddFrame(long elapsedMilliseconds, long frameOffset, uint size)
        {
            m_Index.Add(new AdvFramesIndexEntry()
            {
                ElapsedTime = TimeSpan.FromMilliseconds(elapsedMilliseconds),
                Offset = frameOffset,
                Length = size
            });
        }

        public AdvFramesIndex(BinaryReader reader)
        {
	        TableOffset = reader.BaseStream.Position;

            uint count = reader.ReadUInt32();

            for (int i = 0; i < count; i++)
            {
				uint elapsedMiliseconds = reader.ReadUInt32();
                long offset = reader.ReadInt64();
                uint length = reader.ReadUInt32();                

                AddFrame(elapsedMiliseconds, offset, length);
            }
        }
    }
}
