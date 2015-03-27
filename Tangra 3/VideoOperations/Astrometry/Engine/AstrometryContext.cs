using System;/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.VideoOperations.Astrometry.MPCReport;

namespace Tangra.VideoOperations.Astrometry.Engine
{
	public class AstrometryContext
	{
		public static AstrometryContext Current = new AstrometryContext();

		public FieldSolveContext FieldSolveContext { get; set; }
		public StarMap StarMap { get; set; }
		public StarMapInternalConfig StarMapConfig { get; set; }
		public TangraConfig.PersistedPlateConstants PlateConstants { get; set; }

		private Rectangle m_OSDRectToExclude = Rectangle.Empty;
		
		public Rectangle OSDRectToExclude
		{
			get
			{
			    if (m_OSDRectToExclude == Rectangle.Empty)
			    {
                    m_OSDRectToExclude = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.OSDExclusionArea;
                    LimitByInclusion = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.IsInclusionArea;
			    }

			    return m_OSDRectToExclude;
			}
			set
			{
				m_OSDRectToExclude = value;
			}
		}

		private Rectangle m_RectToInclude = Rectangle.Empty;

		public Rectangle RectToInclude
		{
			get
			{
                if (m_RectToInclude == Rectangle.Empty)
                {
                    m_RectToInclude = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.InclusionArea;
                    LimitByInclusion = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.IsInclusionArea;
                }

				return m_RectToInclude;
			}
			set
			{
				m_RectToInclude = value;
			}
		}

		public bool LimitByInclusion { get; set; }

		public AstrometricState AstrometricState { get; set; }
		public VideoCamera VideoCamera { get; set; }

		public IAstrometricFit CurrentAstrometricFit { get; set; }

		public StarMagnitudeFit CurrentPhotometricFit { get; set; }

		public MPCReportFile CurrentReportFile { get; set; }

		public int BytesPerPixel
		{
			get { return 3; /* We use 24bit bitmaps */ }
		}

		public Rectangle FullFrame
		{
			get { return new Rectangle(0, 0, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight); }
		}

		public void Reset()
		{
			CurrentAstrometricFit = null;
			CurrentPhotometricFit = null;
			AstrometricState = null;
			VideoCamera = null;
			m_OSDRectToExclude = Rectangle.Empty;
			m_RectToInclude = Rectangle.Empty;
			FieldSolveContext = null;
			PlateConstants = null;

			StarMap = null;
			StarMapConfig = StarMapInternalConfig.Default;
		}
	}
}
