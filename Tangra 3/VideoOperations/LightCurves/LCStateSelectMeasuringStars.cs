using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Image;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
    internal class LCStateSelectMeasuringStars : LCState
    {
        internal LCStateSelectMeasuringStars(LCStateMachine context)
            : base(context)
        { }

        public override void Initialize()
        {
            Context.MeasuringStars.Clear();
            Context.MeasuringApertures.Clear();
            Context.PsfFitMatrixSizes.Clear();

            base.Initialize();

            Context.SetCanPlayVideo(true);
        }


        public override bool IsNewObject(ImagePixel star, bool shift, bool ctrl, ref int newOrExistingObjectId)
        {
            if (ctrl)
            {
                // Delete last object
                int starIdx = -1;
                for (int i = 0; i < Context.MeasuringStars.Count; i++)
                {
                    TrackedObjectConfig obj = Context.MeasuringStars[i];

                    if (obj.AsImagePixel == star)
                    {
                        starIdx = i;
                        break;
                    }
                }

                newOrExistingObjectId = -1;

                if (starIdx > -1)
                {
                    Context.MeasuringStars.RemoveAt(starIdx);
                    Context.MeasuringApertures.RemoveAt(starIdx);
                    Context.PsfFitMatrixSizes.RemoveAt(starIdx);

                    if (Context.MeasuringStars.Count < 4)
                        Context.VideoOperation.MaxStarsReached(false);

                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // Add
                newOrExistingObjectId = -1;
                for (int i = 0; i < Context.MeasuringStars.Count; i++)
                {
                    TrackedObjectConfig obj = Context.MeasuringStars[i];

                    if (obj.AsImagePixel == star)
                    {
                        newOrExistingObjectId = i;
                        break;
                    }
                }

                if (newOrExistingObjectId == -1)
                {
                    newOrExistingObjectId = Context.MeasuringStars.Count;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override void ObjectSelected(TrackedObjectConfig selectedObject, bool shift, bool ctrl, int selectedStarId)
        {
            if (ctrl)
            {
                // Delete
                int starIdx = -1;
                for (int i = 0; i < Context.MeasuringStars.Count; i++)
                {
                    TrackedObjectConfig obj = Context.MeasuringStars[i];

                    if (obj.AsImagePixel == selectedObject.AsImagePixel)
                    {
                        starIdx = i;
                        break;
                    }
                }

                if (starIdx > -1)
                {
                    Context.MeasuringStars.RemoveAt(starIdx);
                    Context.MeasuringApertures.RemoveAt(starIdx);
                    Context.PsfFitMatrixSizes.RemoveAt(starIdx);

                    if (Context.MeasuringStars.Count < 4)
                        Context.VideoOperation.MaxStarsReached(false);
                }
            }
            else if (shift)
            {
                Trace.Assert(false, "Overwriting is not implemented");
                //// Overwrite
                //if (MeasuringStars.Count > 0)
                //{
                //    MeasuringStars.RemoveAt(MeasuringStars.Count - 1);
                //    MeasuringApertures.RemoveAt(MeasuringStars.Count - 1);

                //    MeasuringStars.Add(star);
                //    MeasuringApertures.Add(ApertureSize(gaussian));
                //}
            }
            else
            {
                // Add/Update
				int idx = selectedStarId < 0 ? Context.MeasuringStars.IndexOf(selectedObject) : selectedStarId;
                if (idx > -1)
                {
					Context.MeasuringStars[idx] = selectedObject;
					Context.MeasuringApertures[idx] = selectedObject.ApertureInPixels;
					Context.PsfFitMatrixSizes[idx] = selectedObject.PsfFitMatrixSize;

                    Context.VideoOperation.SelectedTargetChanged(idx);
                    return;
                }
                if (Context.MeasuringStars.FindAll(obj => obj.MeasureThisObject).Count >= 4) return;

                Context.MeasuringStars.Add(selectedObject);
                Context.MeasuringApertures.Add(selectedObject.ApertureInPixels);
                Context.PsfFitMatrixSizes.Add(selectedObject.PsfFitMatrixSize);

                if (Context.MeasuringStars.Count >= 4)
                    Context.VideoOperation.MaxStarsReached(true);
            }

            Context.m_SelectedMeasuringStar = Context.MeasuringStars.Count - 1;
            Context.VideoOperation.SelectedTargetChanged(Context.m_SelectedMeasuringStar);
        }
    }
}
