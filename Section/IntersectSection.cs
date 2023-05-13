using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class IntersectSection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IntersectTest class.
        /// </summary>
        public IntersectSection()
          : base("IntersectSection", "IntersectSec",
              "Generate a new section from the intersect with a closed region",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddCurveParameter("Curves", "Curves", "Set of curves to intersect", GH_ParamAccess.list);
            //pManager.AddCurveParameter("Intersector", "Intersector", "Curve to intersect with", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Section", "Section to intersect", GH_ParamAccess.item);
            pManager.AddCurveParameter("Intersector", "Intersector", "Region to intersect", GH_ParamAccess.item);
            pManager.AddBooleanParameter("OutputSection", "NewSection", "Return a new section generated from enclosed region", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("InterSolid", "Solids", "Intersection of solid regions", GH_ParamAccess.list);
            pManager.AddBrepParameter("InterVoid", "Voids", "Intersection of void regions", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "NewArea", "Area", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "NewSection", "New section created by overlapping region", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare input parameters
            Section section = new Section();
            Curve slicer = null;
            bool makenew = false;

            // Retrieve input parameters
            DA.GetData(0, ref section);
            if (!DA.GetData(1, ref slicer)) return;
            DA.GetData(2, ref makenew);

            // Declare output parameters
            List<Curve> solids = new List<Curve>();
            List<Curve> voids = new List<Curve>();
            double area = 0;

            foreach (Curve curve in section.Solids)
            {
                Curve[] intersects = Curve.CreateBooleanIntersection(curve, slicer, Analysis.tol);

                if (intersects.Length > 0)
                {
                    foreach (Curve inter in intersects)
                    {
                        solids.Add(inter);

                        area += AreaMassProperties.Compute(inter).Area;
                    }
                }

            }

            if (section.Voids.Count > 0)
            {
                foreach (Curve curve in section.Voids)
                {
                    Curve[] intersects = Curve.CreateBooleanIntersection(curve, slicer, Analysis.tol);

                    if (intersects.Length > 0)
                    {
                        foreach (Curve inter in intersects)
                        {
                            voids.Add(inter);

                            area -= AreaMassProperties.Compute(inter).Area;
                        }
                    }

                }
            }

            // Output result
            DA.SetDataList(0, solids);
            DA.SetDataList(1, voids);
            DA.SetData(2, area);

            if (makenew)
            {
                Section newsection = new Section(solids, voids, section.E, section.Plane);
                DA.SetData(3, newsection);
            }
            
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Intersect;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("E0331DD9-4002-459C-BB92-9B75621246F2"); }
        }
    }
}