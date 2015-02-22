using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class CCDMetrics
	{
		public double CellWidth;
		public double CellHeight;
		public short MatrixWidth;
		public short MatrixHeight;
	}

	public class VideoCamera
	{
		public VideoCamera()
		{
			CCDMetrics = new CCDMetrics();
		}

		public string Model;

		public CCDMetrics CCDMetrics;

		public bool Integrating;
		public bool ReadOnly = false;

		public override string ToString()
		{
			return Model;
		}

		public static void AddCameraConfigurations(List<VideoCamera> cameras)
		{
			// Load the default configuration
			VideoCamera cam;

			cam = new VideoCamera();
			cam.Model = "GSTAR";
			cam.CCDMetrics.CellWidth = 8.3;
			cam.CCDMetrics.CellHeight = 8.6;
			cam.CCDMetrics.MatrixWidth = 752;
			cam.CCDMetrics.MatrixHeight = 584;
			cam.Integrating = true;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "PC164C (NTSC)";
			cam.CCDMetrics.CellWidth = 9.6;
			cam.CCDMetrics.CellHeight = 7.5;
			cam.CCDMetrics.MatrixWidth = 510;
			cam.CCDMetrics.MatrixHeight = 492;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "PC164-EX (NTSC)";
			cam.CCDMetrics.CellWidth = 6.35;
			cam.CCDMetrics.CellHeight = 7.4;
			cam.CCDMetrics.MatrixWidth = 768;
			cam.CCDMetrics.MatrixHeight = 494;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);

			cam = new VideoCamera();
			cam.Model = "Stella Cam EX";
			cam.CCDMetrics.CellWidth = 8.3;
			cam.CCDMetrics.CellHeight = 8.6;
			cam.CCDMetrics.MatrixWidth = 752;
			cam.CCDMetrics.MatrixHeight = 584;
			cam.Integrating = true;
			cam.ReadOnly = true;
			cameras.Add(cam);

			cam = new VideoCamera();
			cam.Model = "WAT 120N+ (PAL)";
			cam.CCDMetrics.CellWidth = 8.6;
			cam.CCDMetrics.CellHeight = 8.3;
			cam.CCDMetrics.MatrixWidth = 752;
			cam.CCDMetrics.MatrixHeight = 582;
			cam.Integrating = true;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "WAT 120N+ (NTSC)";
			cam.CCDMetrics.CellWidth = 8.4;
			cam.CCDMetrics.CellHeight = 9.8;
			cam.CCDMetrics.MatrixWidth = 768;
			cam.CCDMetrics.MatrixHeight = 494;
			cam.Integrating = true;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "WAT 902H (PAL)";
			cam.CCDMetrics.CellWidth = 8.6;
			cam.CCDMetrics.CellHeight = 8.3;
			cam.CCDMetrics.MatrixWidth = 752;
			cam.CCDMetrics.MatrixHeight = 582;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "WAT 902H (NTSC)";
			cam.CCDMetrics.CellWidth = 8.4;
			cam.CCDMetrics.CellHeight = 9.8;
			cam.CCDMetrics.MatrixWidth = 768;
			cam.CCDMetrics.MatrixHeight = 494;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "WAT 902H Ultimate (PAL)";
			cam.CCDMetrics.CellWidth = 6.5;
			cam.CCDMetrics.CellHeight = 6.25;
			cam.CCDMetrics.MatrixWidth = 752;
			cam.CCDMetrics.MatrixHeight = 582;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);


			cam = new VideoCamera();
			cam.Model = "WAT 902H Ultimate (NTSC)";
			cam.CCDMetrics.CellWidth = 6.35;
			cam.CCDMetrics.CellHeight = 7.4;
			cam.CCDMetrics.MatrixWidth = 768;
			cam.CCDMetrics.MatrixHeight = 494;
			cam.Integrating = false;
			cam.ReadOnly = true;
			cameras.Add(cam);	
		}
	}
}
