using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComboBox = System.Windows.Forms.ComboBox;

namespace Tangra.Config.SettingPannels
{
	public class SettingsPannel : UserControl
	{
		public virtual void LoadSettings() { }
		public virtual void SaveSettings() { }
		public virtual void OnPostSaveSettings() { }
		public virtual void Reset() { }
		public virtual bool ValidateSettings()
		{
			return true;
		}		
	}
}
