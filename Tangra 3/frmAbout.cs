using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Tangra.PInvoke;

namespace Tangra
{
    partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

	        string tangraCoreVersion = TangraCore.GetTangraCoreVersion();
	        string tangraVideoEngineVersion = TangraVideo.GetVideoEngineVersion();
	        string environment = 
#if WIN32
		        "Windows";
#elif __linux__
				"Linux";
#elif MAC
				"Mac";
#else
				INVALID BUILD TYPE
#endif
		        

            this.Text = String.Format("About {0}", AssemblyTitle);
			this.lblProductName.Text = String.Format("{0}, Version {1}, {2} Build", AssemblyProduct, AssemblyVersion, environment);
            this.lblCopyright.Text = AssemblyCopyright;
			this.lblComponentVersions.Text = string.Format("Tangra Core v{0}\n\rTangra Video Engine v{1}", tangraCoreVersion, tangraVideoEngineVersion);
	        this.textBoxDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessorsw

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}