using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.Model.Astro
{
	public class FakeIMeasuredObject : IMeasurableObject
	{
		private PSFFit m_PsfFit;

		public FakeIMeasuredObject(PSFFit psfFit)
		{
			m_PsfFit = psfFit;
		}

		public bool IsOccultedStar
		{
			get { return false; }
		}

		public bool MayHaveDisappeared
		{
			get { return false; }
		}

		public int PsfFittingMatrixSize
		{
			get { return m_PsfFit.MatrixSize; }
		}

		public int PsfGroupId
		{
			get { return -1; }
		}

		public IImagePixel Center
		{
			get { return new ImagePixel(m_PsfFit.XCenter, m_PsfFit.YCenter); }
		}
	}
}
