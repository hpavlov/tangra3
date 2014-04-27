using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.MutualEvents
{
    public partial class frmAddOrEditMutualEventsTarget : Form
    {
        private ImagePixel m_Center;
        private int m_ObjectId;
        private float m_X0;
        private float m_Y0;
        private float m_FWHM;
        private PSFFit m_Gaussian;

        private TangraConfig.PreProcessingFilter SelectedFilter;
        private uint[,] m_ProcessingPixels;
        private byte[,] m_DisplayPixels;
        private LCStateMachine m_State;

        internal readonly TrackedObjectConfig ObjectToAdd;
        private AstroImage m_AstroImage;
        private bool m_IsEdit;

        private VideoController m_VideoController;

        public frmAddOrEditMutualEventsTarget()
        {
            InitializeComponent();
        }

        internal frmAddOrEditMutualEventsTarget(int objectId, ImagePixel center, PSFFit gaussian, LCStateMachine state, VideoController videoController)
        {
            InitializeComponent();

            m_VideoController = videoController;

            Text = "Add 'Mutual Event' Target";
            btnAdd.Text = "Add";
            btnDontAdd.Text = "Don't Add";
            btnDelete.Visible = false;
            m_IsEdit = false;

            m_ObjectId = objectId;
            m_State = state;
            m_AstroImage = m_State.VideoOperation.m_StackedAstroImage;

            ObjectToAdd = new TrackedObjectConfig();

            m_Center = new ImagePixel(center);
        }
    }
}
