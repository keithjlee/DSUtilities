using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Drawing
{
    public class Arrows : GH_Component
    {

        /// <summary>
        /// Initializes a new instance of the Arrows class.
        /// </summary>
        /// 
        public Arrows()
          : base("Arrows", "Arrow3D",
              "Draw an arrow based on a curve",
              "DSutilities", "Drawing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to convert", GH_ParamAccess.item);
            pManager.AddNumberParameter("PipeRadius", "rPipe", "Radius of arrow body", GH_ParamAccess.item);
            pManager.AddNumberParameter("ConeRadius", "rArrow", "Radius of arrow cone base", GH_ParamAccess.item);
            pManager.AddNumberParameter("ConeLength", "LArrow", "Length of arrow cone", GH_ParamAccess.item);
            pManager.AddBooleanParameter("BothSides", "Both", "Draw arrow on both sides", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Arrow", "Arrow", "Arrow", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Initialize
            Curve curve = null;
            double r_pipe = 0;
            double r_cone = 0;
            double l_cone = 0;
            bool both = false;

            //assign
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref r_pipe)) return;
            if (!DA.GetData(2, ref r_cone)) return;
            if (!DA.GetData(3, ref l_cone)) return;
            DA.GetData(4, ref both);

            //check curve validity
            if (curve == null || curve.GetLength() <= double.Epsilon) return;

            //make pipe
            List<Brep> breps = Brep.CreatePipe(curve, r_pipe, false, PipeCapMode.Flat, true, 1e-5, 1e-5).ToList();

            //Forward cone
            //curve.FrameAt(curve.GetLength(), out Plane end_frame);
            //Brep end_cone = MakeCone(end_frame, r_cone, l_cone, false);

            Plane frame1 = new Plane(curve.PointAtEnd + curve.TangentAtEnd * l_cone, -curve.TangentAtEnd);
            Brep end_cone = new Cone(frame1, l_cone, r_cone).ToBrep(true);

            //optional backwards arrow
            if (both)
            {
                //backwards cone
                Plane frame2 = new Plane(curve.PointAtStart - curve.TangentAtStart * l_cone, curve.TangentAtStart);
                Brep start_cone = new Cone(frame2, l_cone, r_cone).ToBrep(true);

                breps.Add(end_cone);
                breps.Add(start_cone);
            }
            else
            {
                breps.Add(end_cone);
            }

            Brep arrow = Brep.CreateBooleanUnion(breps, 1e-5)[0];
            DA.SetData(0, arrow);
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
            get { return new Guid("C6249ED5-FA9E-4429-84D2-3D1F7A5C7DC7"); }
        }
    }
}