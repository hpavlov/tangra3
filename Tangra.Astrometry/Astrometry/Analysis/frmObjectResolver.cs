using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Astro.Images;
using Astro.Images.MathHelpers;
using Astro.Utilities.StarCatalogues;

namespace Tangra.Astrometry.Astrometry.Analysis
{
	public partial class frmObjectResolver : Form
	{
		private AstroImage m_Image;
		private Dictionary<PSFFit, IStar> m_MappedObjects;
		private Dictionary<PSFFit, double> m_UnknownObjects;

		public frmObjectResolver(
			AstroImage img, 
			Dictionary<PSFFit, IStar> mappedObjects, 
			Dictionary<PSFFit, double> unknownObjects)
		{
			InitializeComponent();

			m_Image = img;
			m_MappedObjects = mappedObjects;
			m_UnknownObjects = unknownObjects;
		}

		private void frmObjectResolver_Load(object sender, EventArgs e)
		{
			pbPlate.Image = m_Image.Bitmap;
			pbPlate.Refresh();
		}


	}
}
