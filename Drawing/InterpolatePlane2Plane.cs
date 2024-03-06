using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Drawing
{
    public class InterpolatePlane2Plane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the InterpolateTransformation class.
        /// </summary>
        public InterpolatePlane2Plane()
          : base("InterpolatePlanarTransform", "InterpP2P",
              "Interpolate a plane-to-plane transform from 0 to 1",
              "DSutilities", "Drawing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("SourcePlane", "PlaneFrom", "Source plane.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("TargetPlane", "PlaneTo", "Target plane.", GH_ParamAccess.item);
            pManager.AddNumberParameter("t", "t", "Parameter value: [0, 1]", GH_ParamAccess.item, 0.5);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTransformParameter("Transform", "T(t)", "Interpolated transformation", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane P1 = new Plane();
            Plane P2 = new Plane();
            double t = 0.5;

            if (!DA.GetData(0, ref P1)) return;
            if (!DA.GetData(1, ref P2)) return;
            DA.GetData(2, ref t);

            //clamp
            t = t < 0 ? 0 : t;
            t = t > 1 ? 1 : t;

            //get rotation axis and angle
            Quaternion q = Quaternion.Rotation(P1, P2);
            q.GetRotation(out double angle, out Vector3d axis);

            //clamp angle
            angle = angle > Math.PI ? angle - 2 * Math.PI : angle;

            //make new interpolated transformation
            Plane interp_plane = new Plane(P1);
            interp_plane.Rotate(t * angle, axis, interp_plane.Origin);
            interp_plane.Translate(t * (P2.Origin - P1.Origin));

            //make interpolated transformation
            Transform T = Transform.PlaneToPlane(P1, interp_plane);

            DA.SetData(0, T);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("85D2F9C9-4CF0-4798-B6E8-919286FBBB87"); }
        }
    }
}