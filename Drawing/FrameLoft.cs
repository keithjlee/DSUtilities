using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Drawing
{
    public class FrameLoft : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtrudeSection class.
        /// </summary>
        public FrameLoft()
          : base("FrameLoft", "FrameLoft",
              "Loft a brep along a curve's perpendicular frames",
              "DSutilities", "Drawing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Geometry", "Geo", "Geometry to loft as planar brep", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Plane of geometry to loft", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "Crv", "Curve to loft along", GH_ParamAccess.item);
            pManager.AddIntegerParameter("NumDiscretizations", "n", "Number of discretizations of curve. Use more for more accurate loft", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Frame rotation", "Rot", "Rotation of initial curve frames [radians]", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Extrusion", "Extr", "Extrusion result", GH_ParamAccess.item);
            pManager.AddPlaneParameter("CurveFrames", "Frames", "Curve frames used for loft", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Initialize
            Curve geo = null;
            Plane plane = Plane.WorldXY;
            Curve curve = null;
            int n = 2;
            double theta = 0;

            //assign
            if (!DA.GetData(0, ref geo)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref curve)) return;
            DA.GetData(3, ref n);
            DA.GetData(4, ref theta);

            //ensure minimum n
            n = n < 2 ? 2 : n;

            //divide target curve
            double[] t = curve.DivideByCount(n, true);

            //get perpendicular frames
            Plane[] frames_init = curve.GetPerpendicularFrames(t);
            List<Plane> frames = new List<Plane>();

            //rotate frames
            foreach (Plane frame  in frames_init)
            {
                frame.Rotate(theta, frame.Normal);
                frames.Add(frame);
            }

            //get transformations from brep to frame
            List<Transform> transforms = new List<Transform>();

            foreach (Plane frame in frames)
            {
                transforms.Add(Transform.PlaneToPlane(plane, frame));
            }

            //get list of transformed curves
            List<Curve> aligned_curves = new List<Curve>();
            foreach (Transform transform in transforms)
            {
                Curve newcurve = geo.DuplicateCurve();
                newcurve.Transform(transform);
                aligned_curves.Add(newcurve);
            }

            Brep loft = Brep.CreateFromLoft(aligned_curves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0].CapPlanarHoles(1e-6);

            DA.SetData(0, loft);
            DA.SetDataList(1, frames);
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
            get { return new Guid("503674CC-5E7B-4FBD-8CDA-7F0033C5C02C"); }
        }
    }
}