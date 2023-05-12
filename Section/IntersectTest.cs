using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class IntersectTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IntersectTest class.
        /// </summary>
        public IntersectTest()
          : base("IntersectTest", "Intersect",
              "Description",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "Curves", "Set of curves to intersect", GH_ParamAccess.list);
            pManager.AddCurveParameter("Intersector", "Intersector", "Curve to intersect with", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("NewCurves", "NewCurves", "Solution", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "NewArea", "Area", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare input parameters
            List<Curve> curves = new List<Curve>();
            Curve slicer = null;

            // Retrieve input parameters
            DA.GetDataList(0, curves);
            DA.GetData(1, ref slicer);

            // Declare output parameters
            List<Curve> output = new List<Curve>();
            double area = 0;

            //get intersects
            foreach (Curve curve in curves)
            {
                Curve[] intersects = Curve.CreateBooleanIntersection(curve, slicer, Analysis.tol);

                if (intersects.Length > 0)
                {
                    foreach (Curve inter in intersects)
                    {
                        output.Add(inter);

                        var amp = AreaMassProperties.Compute(inter);
                        area += amp.Area;
                    }
                }

            }

            // Output result
            DA.SetDataList(0, output);
            DA.SetData(1, area);
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
            get { return new Guid("E0331DD9-4002-459C-BB92-9B75621246F2"); }
        }
    }
}