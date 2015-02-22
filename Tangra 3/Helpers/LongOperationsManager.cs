/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry.Recognition;
using Tangra.Controller;

namespace Tangra.Helpers
{
	internal class LongOperationsManager
	{
		public static byte MSG_ID_INITIALISE = 1;
		public static byte MSG_ID_BEGIN_OPERATION = 2;
		public static byte MSG_ID_END_OPERATION = 3;

		private VideoController m_VideoController;
		private Form m_MainForm;
		private IntPtr m_MainFormHandle;

		private frmLongRunningOperation m_BlockUIForm;

		public LongOperationsManager(frmMain parentForm, VideoController videoController)
		{
			m_BlockUIForm = new frmLongRunningOperation();
			IntPtr handleHack = m_BlockUIForm.Handle;
			Trace.WriteLine("BlockUI Initialised: " + handleHack.ToInt32());

			m_VideoController = videoController;
			m_MainForm = parentForm;
			m_MainFormHandle = parentForm.Handle;
			m_BlockUIForm.Initialise(m_MainForm);
		}

		public void BeginLongOperation(string userDisplayText)
		{
			if (m_VideoController != null)
			{
				m_VideoController.StatusChanged(userDisplayText);
				m_VideoController.SetPictureBoxCursor(Cursors.WaitCursor);
				m_MainForm.Refresh();

				m_BlockUIForm.BeginOperation(userDisplayText);
			}
		}

		public void EndLongOperation()
		{
			if (m_VideoController != null)
			{
				m_VideoController.StatusChanged("Ready");
				m_VideoController.SetPictureBoxCursor(Cursors.Default);
				m_MainForm.Refresh();

				m_BlockUIForm.EndOperation();
			}
		}
	}
}
