/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Config;
using Tangra.Model.Astro;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Astrometry.Engine
{
	internal class CalibrationContext
	{
		// Input
		public AstroPlate PlateConfig { get; set; }
		public IAstrometricFit PreliminaryFit { get; set; }
		public IStarMap StarMap { get; set; }
		public Rectangle FitExcludeArea { get; set; }
		public FieldSolveContext FieldSolveContext { get; set; }


		internal IAstrometricFit PreliminaryAstrometricFit { get; set; }
		public TangraConfig.PersistedPlateConstants PlateConstants { get; set; }

		public LeastSquareFittedAstrometry InitialFirstAstrometricFit { get; set; }
		public LeastSquareFittedAstrometry InitialSecondAstrometricFit { get; set; }

		public LeastSquareFittedAstrometry FirstAstrometricFit { get; set; }
		public LeastSquareFittedAstrometry SecondAstrometricFit { get; set; }
		public LeastSquareFittedAstrometry DistanceBasedFit { get; set; }
		public LeastSquareFittedAstrometry ImprovedDistanceBasedFit { get; set; }
		public PlateConstantsSolver ConstantsSolver { get; set; }

		private static int SERIALIZATION_VERSION = 3;

		public byte[] Serialize()   
		{
			BinaryFormatter fmt = new BinaryFormatter();

			using (MemoryStream memStr = new MemoryStream())
			{
				fmt.Serialize(memStr, SERIALIZATION_VERSION);
				fmt.Serialize(memStr, PlateConfig);

				StarMap sMap = StarMap as StarMap;
				fmt.Serialize(memStr, sMap);

				fmt.Serialize(memStr, FitExcludeArea.Top);
				fmt.Serialize(memStr, FitExcludeArea.Left);
				fmt.Serialize(memStr, FitExcludeArea.Width);
				fmt.Serialize(memStr, FitExcludeArea.Height);

				fmt.Serialize(memStr, FieldSolveContext);

				DirectTransRotAstrometry trRot = PreliminaryFit as DirectTransRotAstrometry;
                fmt.Serialize(memStr, trRot != null);
                if (trRot != null) fmt.Serialize(memStr, trRot);

                fmt.Serialize(memStr, FirstAstrometricFit != null);
				if (FirstAstrometricFit != null) fmt.Serialize(memStr, FirstAstrometricFit);

                fmt.Serialize(memStr, SecondAstrometricFit != null);
                if (SecondAstrometricFit != null) fmt.Serialize(memStr, SecondAstrometricFit);

                fmt.Serialize(memStr, DistanceBasedFit != null);
                if (DistanceBasedFit != null) fmt.Serialize(memStr, DistanceBasedFit);

                fmt.Serialize(memStr, ImprovedDistanceBasedFit != null);
                if (ImprovedDistanceBasedFit != null) fmt.Serialize(memStr, ImprovedDistanceBasedFit);


				FocalLengthFit flf = null;
				if (ConstantsSolver != null)
				{
					flf = ConstantsSolver.ComputeFocalLengthFit();
				}

				fmt.Serialize(memStr, flf != null);
				if (flf != null) fmt.Serialize(memStr, flf);


				memStr.Flush();

				return memStr.ToArray();
			}
		}

		public static CalibrationContext Deserialize(byte[] data)
		{
			CalibrationContext ctx = new CalibrationContext();

			BinaryFormatter fmt = new BinaryFormatter();

			using (MemoryStream memStr = new MemoryStream(data))
			using (StreamReader rdr = new StreamReader(memStr))
			{
				int version = (int)fmt.Deserialize(memStr);
				if (version > 0)
				{
					object obj = fmt.Deserialize(memStr);
					try
					{
						ctx.PlateConfig = (AstroPlate)obj;
					}
					catch (Exception)
					{
						ctx.PlateConfig = AstroPlate.FromReflectedObject(obj);
					}
					

					obj = fmt.Deserialize(memStr);
					try
					{
						ctx.StarMap = (StarMap)obj;
					}
					catch (Exception)
					{
						ctx.StarMap = Tangra.Model.Astro.StarMap.FromReflectedObject(obj);
					}
					

					int top = (int)fmt.Deserialize(memStr);
					int left = (int)fmt.Deserialize(memStr);
					int width = (int)fmt.Deserialize(memStr);
					int height = (int)fmt.Deserialize(memStr);

					ctx.FitExcludeArea = new Rectangle(top, left, width, height);

					obj = fmt.Deserialize(memStr);
					try
					{
						ctx.FieldSolveContext = (FieldSolveContext)obj;
					}
					catch(Exception)
					{
						ctx.FieldSolveContext = FieldSolveContext.FromReflectedObject(obj);
					}

					if (version > 1)
					{
						bool noNull = (bool)fmt.Deserialize(memStr);
						if (noNull)
						{
							obj = fmt.Deserialize(memStr);
							try
							{
								ctx.PreliminaryFit = (DirectTransRotAstrometry)obj;
							}
							catch (Exception)
							{
								ctx.PreliminaryFit = DirectTransRotAstrometry.FromReflectedObject(obj);
							}
							
						}
				
                        noNull = (bool)fmt.Deserialize(memStr);
                        if (noNull)
                        {
							obj = fmt.Deserialize(memStr);
							try
							{
								ctx.FirstAstrometricFit = (LeastSquareFittedAstrometry)obj;
							}
							catch (Exception)
							{
								ctx.FirstAstrometricFit = LeastSquareFittedAstrometry.FromReflectedObject(obj);
							}
                        }

                        noNull = (bool)fmt.Deserialize(memStr);
						if (noNull)
						{
							obj = fmt.Deserialize(memStr);
							try
							{
								ctx.SecondAstrometricFit = (LeastSquareFittedAstrometry)obj;
							}
							catch (Exception)
							{
								ctx.SecondAstrometricFit = LeastSquareFittedAstrometry.FromReflectedObject(obj);
							}
						}
                        
                        noNull = (bool)fmt.Deserialize(memStr);
						if (noNull)
						{
							obj = fmt.Deserialize(memStr);
							try
							{
								ctx.DistanceBasedFit = (LeastSquareFittedAstrometry)obj;
							}
							catch (Exception)
							{
								ctx.DistanceBasedFit = LeastSquareFittedAstrometry.FromReflectedObject(obj);
							}
						}

                        noNull = (bool)fmt.Deserialize(memStr);
						if (noNull)
						{
							obj = fmt.Deserialize(memStr);
							try
							{
								ctx.ImprovedDistanceBasedFit = (LeastSquareFittedAstrometry)obj;
							}
							catch (Exception)
							{
								ctx.ImprovedDistanceBasedFit = LeastSquareFittedAstrometry.FromReflectedObject(obj);
							}
						}

						if (version > 2)
						{
							FocalLengthFit flf = null;

							noNull = (bool)fmt.Deserialize(memStr);
							if (noNull)
							{
								obj = fmt.Deserialize(memStr);
								try
								{
									flf = (FocalLengthFit)obj;
								}
								catch (Exception)
								{
									flf = FocalLengthFit.FromReflectedObject(obj);
								}
							}

							ctx.ConstantsSolver = new PlateConstantsSolver(ctx.PlateConfig);
							ctx.ConstantsSolver.SetFocalLengthFit(flf);
						}
					}
					else
					{
						ctx.PreliminaryFit = (DirectTransRotAstrometry)fmt.Deserialize(memStr);
					}
				}
			}

			return ctx;
		}
	}
}
