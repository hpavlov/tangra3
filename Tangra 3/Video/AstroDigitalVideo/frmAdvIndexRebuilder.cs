﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tangra.Video.AstroDigitalVideo
{
	public partial class frmAdvIndexRebuilder : Form
	{
		private string m_FileName;
		private AdvFile m_AdvFile;

		private Thread m_CalcThread;
		internal string NewFileName;

		public frmAdvIndexRebuilder(string fileName)
		{
			InitializeComponent();

			m_FileName = fileName;

			pbar1.Maximum = 100;
			pbar1.Minimum = 0;
			pbar1.Value = 0;
		}

		private void frmAdvIndexRebuilder_Load(object sender, EventArgs e)
		{
			actionTimer.Enabled = true;			
		}

		private void actionTimer_Tick(object sender, EventArgs e)
		{
			actionTimer.Enabled = false;
			ProcessAdvFile();
		}

		private void ProcessAdvFile()
		{
			try
			{
				m_AdvFile = AdvFile.OpenFile(m_FileName);
				if (!m_AdvFile.IsCorrupted)
				{
					MessageBox.Show(
						this,
						string.Format("{0} does not need to be repaired.", Path.GetFileName(m_FileName)),
						"Tangra",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);

					DialogResult = DialogResult.Cancel;
					Close();
				}

				m_CalcThread = new Thread(RepairAdvFile);
				m_CalcThread.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					this, 
					ex.Message, 
					"Tangra",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				
				DialogResult = DialogResult.Abort;
				Close();
			}
		}

		private delegate void UpdateUIDelegate(string process, string status, int percent);

		private void UpdateUI(string process, string status, int percent)
		{
			lblProcess.Text = process;
			lblStatus.Text = status;

			pbar1.Value = percent;
			pbar1.Update();

			Update();
			Application.DoEvents();
		}

		private void InvokeUpdateUI(string process, string status, int percentDone)
		{
			Invoke(new UpdateUIDelegate(UpdateUI), new object[] { process, status, percentDone });						
		}

		private void RepairAdvFile(object state)
		{
			InvokeUpdateUI("Searching for ADV frames ...", "", 0);

			m_AdvFile.SearchFramesByMagicPhase1(
				delegate(int percentDone, int framesFound)
					{
						string process = "Searching for ADV frames ...";
						string status = string.Format("{0} potential frames found", framesFound);

						InvokeUpdateUI(process, status, percentDone);
					});

			InvokeUpdateUI("Searching for ADV frames ...", "", 100);

			InvokeUpdateUI("Recovering ADV frames ...", "", 0);

			m_AdvFile.SearchFramesByMagicPhase2(
				delegate(int percentDone, int framesRecovered)
				{
					string process = "Recovering ADV frames ...";
					string status = string.Format("{0} frames recovered successfully", framesRecovered);

					InvokeUpdateUI(process, status, percentDone);
				});

			InvokeUpdateUI("Recovering ADV frames ...", "", 100);
			Invoke(new ProcInvoker(SaveRecoveredFile));
		}

		private delegate void ProcInvoker();

		private void SaveRecoveredFile()
		{
			if (m_AdvFile.Index.Index.Count == 0)
			{
				MessageBox.Show(this, "No frames were recovered. Is this a newer/different ADV file format?", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				saveFileDialog.Title = string.Format("Save {0} recovered ADV frames as ...", m_AdvFile.Index.Index.Count);
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					NewFileName = saveFileDialog.FileName;
					m_AdvFile.SaveRecoveredFileAs(NewFileName);
					DialogResult = DialogResult.OK;
				}
				else
					DialogResult = DialogResult.Cancel;				
			}
			
			Close();
		}

		private void AbortProcessing()
		{
			if (m_CalcThread != null && m_CalcThread.IsAlive)
			{
				m_CalcThread.Abort();
				m_CalcThread.Join(10000);

				m_CalcThread = null;
			}
		}

		private void frmAdvIndexRebuilder_FormClosing(object sender, FormClosingEventArgs e)
		{
			AbortProcessing();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			AbortProcessing();

			DialogResult = DialogResult.Abort;
			Close();
		}
	}
}