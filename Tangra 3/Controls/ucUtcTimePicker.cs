using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Controls
{
    public partial class ucUtcTimePicker : UserControl
    {
        private DateTime m_Utc = DateTime.Now.ToUniversalTime();

        public DateTime DateTimeUtc
        {
            get { return m_Utc; }
            set 
            {
                m_Utc = value;
                SetControls();
            }
        }

        public EventHandler<DateTimeChangeEventArgs> OnDateChanged;
        public EventHandler<DateTimeChangeEventArgs> OnDateTimeChanged;
        public EventHandler OnDateTimeInputComplete;

        public ucUtcTimePicker()
        {
            InitializeComponent();

            SetControls();
        }

        private void SetControls()
        {
			if (m_Utc.Year < 1950) m_Utc = DateTime.Now;

            dtpDate.Value = m_Utc;
            tbxHours.Text = m_Utc.Hour.ToString();
            tbxMinutes.Text = m_Utc.Minute.ToString();
            tbxSeconds.Text = m_Utc.Second.ToString();
            tbxMiliseconds.Text = m_Utc.Millisecond.ToString("000");
        }

        private void SetNewDateTime(TextBox senderTextbox)
        {
            if (OnDateTimeChanged != null)
                OnDateTimeChanged(this, new DateTimeChangeEventArgs(m_Utc));

            if (senderTextbox != null && senderTextbox.Text.Length == 2)
            {
                if (senderTextbox == tbxHours)
                {
                    tbxMinutes.Focus();
                    tbxMinutes.SelectAll();
                }
                else if (senderTextbox == tbxMinutes)
                {
                    tbxSeconds.Focus();
                    tbxSeconds.SelectAll();

                }
                else if (senderTextbox == tbxSeconds)
                {
                    tbxMiliseconds.Focus();
                    tbxMiliseconds.SelectAll();
                }
            }
        }

        public void FocusHourControl()
        {
            tbxHours.Select();
            tbxHours.Focus();
            tbxHours.SelectAll();
        }

        private void tbxHours_TextChanged(object sender, EventArgs e)
        {
            int intVal;
            bool failed = true;
            if (int.TryParse(tbxHours.Text, out intVal))
            {
                if (intVal >= 0 &&
                    intVal <= 23)
                {
                    m_Utc = new DateTime(
                        m_Utc.Year, m_Utc.Month, m_Utc.Day, intVal, m_Utc.Minute, m_Utc.Second, m_Utc.Millisecond);

                    failed = false;

                    SetNewDateTime(sender as TextBox);
                }
            }

            if (failed) SetControls();
        }

        private void tbxMinutes_TextChanged(object sender, EventArgs e)
        {
            int intVal;
            bool failed = true;
            if (int.TryParse(tbxMinutes.Text, out intVal))
            {
                if (intVal >= 0 &&
                    intVal <= 59)
                {
                    m_Utc = new DateTime(
                        m_Utc.Year, m_Utc.Month, m_Utc.Day, m_Utc.Hour, intVal, m_Utc.Second, m_Utc.Millisecond);

                    failed = false;

					SetNewDateTime(sender as TextBox);
                }
            }

            if (failed) SetControls();
        }

        private void tbxSeconds_TextChanged(object sender, EventArgs e)
        {
            int intVal;
            bool failed = true;
            if (int.TryParse(tbxSeconds.Text, out intVal))
            {
                if (intVal >= 0 &&
                    intVal <= 59)
                {
                    m_Utc = new DateTime(
                        m_Utc.Year, m_Utc.Month, m_Utc.Day, m_Utc.Hour, m_Utc.Minute, intVal, m_Utc.Millisecond);

                    failed = false;

					SetNewDateTime(sender as TextBox);
                }
            }

            if (failed) SetControls();
        }

        private void tbxMiliseconds_TextChanged(object sender, EventArgs e)
        {
            int intVal;
            bool failed = true;
            if (int.TryParse(tbxMiliseconds.Text, out intVal))
            {
                if (intVal >= 0 &&
                    intVal <= 999)
                {
                    m_Utc = new DateTime(
                        m_Utc.Year, m_Utc.Month, m_Utc.Day, m_Utc.Hour, m_Utc.Minute, m_Utc.Second, intVal);

                    failed = false;

                    if (OnDateTimeChanged != null)
                        OnDateTimeChanged(this, new DateTimeChangeEventArgs(m_Utc));

                    if (tbxMiliseconds.Text.Length == 3)
                    {
                        if (OnDateTimeInputComplete != null)
                            OnDateTimeInputComplete(this, EventArgs.Empty);
                    }
                }
            }

            if (failed) SetControls();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            m_Utc = new DateTime(
				dtpDate.Value.Year, dtpDate.Value.Month, dtpDate.Value.Day,
                m_Utc.Hour, m_Utc.Minute, m_Utc.Second, m_Utc.Millisecond);

            if (OnDateChanged != null)
                OnDateChanged(this, new DateTimeChangeEventArgs(m_Utc));
        }

        public void EnterTimeAtTheSameDate()
        {
            Focus();
            tbxHours.Focus();
            tbxHours.SelectAll();
        }

        private void tbxInput_Enter(object sender, EventArgs e)
        {
            TextBox tbx = sender as TextBox;
            if (tbx != null)
                tbx.SelectAll();
        }
    }

    public class DateTimeChangeEventArgs : EventArgs
    {
        internal DateTimeChangeEventArgs(DateTime utcDateTime)
        {
            UtcDateTime = utcDateTime;
        }

        public readonly DateTime UtcDateTime;
    }

}
