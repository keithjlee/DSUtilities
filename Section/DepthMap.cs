using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class DepthMap : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DepthMap class.
        /// </summary>
        public DepthMap()
          : base("DepthMap", "DvA",
              "Extract the depth-area relationship",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Section", "Section to analyze", GH_ParamAccess.item);
            pManager.AddIntegerParameter("NumberOfSamples", "Nsamples", "Number of samples to take", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("Direction", "Direction", "Direction of depth (-1: top, 1:bottom)", GH_ParamAccess.item, -1);

            Param_Integer param = pManager[2] as Param_Integer;
            param.AddNamedValue("Downwards", -1);
            param.AddNamedValue("Upwards", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Depth", "D", "Sampled depth points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "A(D)", "Associated area", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Section section = new Section();
            int n = 100;
            int dir = -1;

            if (!DA.GetData(0, ref section)) return;
            DA.GetData(1, ref n);
            DA.GetData(2, ref dir);

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

            List<double> areas = new List<double> { 0};
            List<double> depths = new List<double> { 0};

            double H = section.Corners[0].DistanceTo(section.Corners[2]);

            double dincrement = H / n;
            double d;

            for (int i = 1; i <= n; i++)
            {
                d = dincrement * i;
                Rectangle3d intersector = new Rectangle3d(refplane, startpoint, endpoint + dir * d * vec);
                double area = Analysis.GetArea(section, intersector.ToNurbsCurve());
                areas.Add(area);
                depths.Add(d);
            }

            DA.SetDataList(0, depths);
            DA.SetDataList(1, areas);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DepthMap;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("60E353A9-367E-474C-A2E4-CEF991F2E971"); }
        }
    }
}