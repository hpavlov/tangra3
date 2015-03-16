/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.PInvoke;

namespace Tangra
{
    partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            this.Text = string.Format("About {0}", VersionHelper.AssemblyTitle);
            this.textBoxDescription.Text = VersionHelper.AssemblyDescription;
            if (!string.IsNullOrEmpty(VersionHelper.AssemblyReleaseDate))
            {
                this.lblProductName.Text = string.Format("{0} v{1}{2}, Released on {3}", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion, VersionHelper.IsBetaRelease ? " BETA" : "", VersionHelper.AssemblyReleaseDate);
            }
            else
                this.lblProductName.Text = string.Format("{0} v{1}, Unreleased ALPHA Version", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion);
        }

        private void btnSystemInfo_Click(object sender, EventArgs e)
        {
            var frm = new frmSystemInfo();
            frm.ShowDialog(this);
        }
    }
}