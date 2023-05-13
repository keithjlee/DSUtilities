using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class DfromA : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CfromA class.
        /// </summary>
        public DfromA()
          : base("DepthFromArea", "DfromA",
              "Get the depth required to provide a given area",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Section", "Section to analyze", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "A", "Target area", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Direction", "Direction", "Direction of depth (-1: top, 1:bottom)", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("RelativeTolerance", "RelTol", "Relative tolerance stopping criteria", GH_ParamAccess.item, .05);
            pManager.AddIntegerParameter("MaximumIterations", "MaxIter", "Maximum number of iterations for solution", GH_ParamAccess.item, 250);

            Param_Integer param = pManager[2] as Param_Integer;
            param.AddNamedValue("Downwards", -1);
            param.AddNamedValue("Upwards", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Depth", "D", "Required depth", GH_ParamAccess.item);
            pManager.AddNumberParameter("RelativeError", "RelErr", "Relative error of solution (Asolution - Atarget) / Atarget", GH_ParamAccess.item) ;
            pManager.AddCurveParameter("Region", "Region", "Associated region", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Section section = new Section();
            double target = 0;
            int dir = -1;
            double tol = .05;
            int maxiter = 250;

            if (!DA.GetData(0, ref section)) return;
            if (!DA.GetData(1, ref target)) return;
            DA.GetData(2, ref dir);
            DA.GetData(3, ref tol);
            DA.GetData(4, ref maxiter);

            if (section.Area < target)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Target area exceeds total area of section.");
            }

            //total depth of section
            double dMax = section.Corners[0].DistanceTo(section.Corners[2]);

            //starting points for rectangle
            Point3d startpoint;
            Point3d endpoint;
            Vector3d vec = Analysis.UnitVector(section.Plane);
            Plane refplane = Analysis.APtoWorld((int)section.Plane);

            if (dir == -1)
            {
                startpoint = section.Corners[0];
                endpoint = section.Corners[1];
            }
            else
            {
                startpoint = section.Corners[2];
                endpoint = section.Corners[3];
            }

            double area = 0;
            double err = Math.Abs(target - area) / target;
            int iter = 1;
            double dupper = dMax;
            double dlower = 0;
            double d = (dupper + dlower) / 2;


            Rectangle3d intersector = new Rectangle3d(refplane, startpoint, endpoint + dir * d * vec);

            while (iter <= maxiter || err > tol)
            {
                area = Analysis.GetArea(section, intersector.ToNurbsCurve());

                double diff = target - area;

                err = Math.Abs(diff) / target;

                if (diff > 0)
                {
                    dlower = d;
                }
                else
                {
                    dupper = d;
                }

                d = (dupper + dlower) / 2;

                intersector = new Rectangle3d(refplane, startpoint, endpoint + dir * d * vec);

                iter++;
            }

            DA.SetData(0, d);
            DA.SetData(1, err);
            DA.SetData(2, intersector);

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
            get { return new Guid("90EF36F0-98DF-4ACB-B0EC-4565FBDE7A42"); }
        }
    }
}