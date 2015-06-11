using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.ViewSpectraStates
{
	public class SpectraViewerStateBase
	{
		protected MasterSpectra m_MasterSpectra;
		protected SpectraViewerStateManager m_StateManager;
		protected PictureBox m_View;

		public void SetMasterSpectra(MasterSpectra masterSpectra)
		{
			m_MasterSpectra = masterSpectra;
		}

		public virtual void Initialise(SpectraViewerStateManager manager, PictureBox view)
		{
			m_StateManager = manager;
			m_View = view;
		}

		public virtual void Finalise()
		{ }

		public virtual void PreDraw(Graphics g)
		{ }

		public virtual void PostDraw(Graphics g)
		{ }

		public virtual void MouseClick(object sender, MouseEventArgs e)
		{ }

		public virtual void MouseDown(object sender, MouseEventArgs e)
		{ }

		public virtual void MouseEnter(object sender, EventArgs e)
		{ }

		public virtual void MouseLeave(object sender, EventArgs e)
		{ }

		public virtual void MouseMove(object sender, MouseEventArgs e)
		{ }

		public virtual void MouseUp(object sender, MouseEventArgs e)
		{ }
	}
}
