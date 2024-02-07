using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Drawing
{
    public class AlignExtrude : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtrudeSection class.
        /// </summary>
        public AlignExtrude()
          : base("AlignExtrude", "AlExtrude",
              "Extrude a brep with planar alignment",
              "DSutilities", "Drawing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry to extrude as planar brep", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Plane of geometry to extrude", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "Crv", "Curve to extrude along", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Extrusion", "Extr", "Extrusion result", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Initialize
            Brep geo = null;
            Plane plane = Plane.WorldXY;
            Curve curve = null;
            //assign
            if (!DA.GetData(0, ref geo)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref curve)) return;

            //Align brep to curve frame at start point
            curve.FrameAt(0, out Plane target_plane);
            Transform p2p = Transform.PlaneToPlane(plane, target_plane);
            geo.Transform(p2p);

            //extract face
            BrepFace face = geo.Faces[0];

            //extrude
            Brep extr = face.CreateExtrusion(curve, true);

            DA.SetData(0, extr);
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