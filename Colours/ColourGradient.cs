using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;

namespace DSUtilities.Colours
{
    public class ColourGradient : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ColourGradient class.
        /// </summary>
        public ColourGradient()
          : base("ColourGradient", "ColourGrad",
              "Get the item-wise colour value within a gradient",
              "DSutilities", "Colours")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Values", "Vals", "Values to colour", GH_ParamAccess.list);
            pManager.AddNumberParameter("ScaleFactor", "Fac", "Shrink the actual range of values by a factor", GH_ParamAccess.item, 1.0);
            pManager.AddColourParameter("MinimumColour", "CMin", "Colour of smallest value", GH_ParamAccess.item, Colours.Pink);
            pManager.AddColourParameter("MiddleColour", "CMid", "Optional colour at middle value", GH_ParamAccess.item);
            pManager.AddColourParameter("MaximumColour", "CMax", "Colour of largest value", GH_ParamAccess.item, Colours.Blue);
            pManager.AddBooleanParameter("CenterAtZero", "CenterZero", "If value has both negative and positive values, center the colours such that a value of 0 is the mid point", GH_ParamAccess.item, true);

            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Colours", "C", "Value-wise colours", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> values = new List<double>();
            double factor = 1.0;
            Color cmin = new Color();
            Color cmid = new Color();
            Color cmax = new Color();
            bool center = true;

            if (!DA.GetDataList(0, values)) return;
            DA.GetData(1, ref factor);
            DA.GetData(2, ref cmin);
            DA.GetData(4, ref cmax);
            DA.GetData(5, ref center);

            //see if middle colour is provided
            bool cmid_provided = true;
            if (!DA.GetData(3, ref cmid)) cmid_provided = false;

            List<Color> colors;
            if (cmid_provided)
            {
                colors = Colours.MakeGradient(values, factor, cmin, cmid, cmax, center);
            }
            else
            {
                colors = Colours.MakeGradient(values, factor, cmin, cmax, center);
            }

            DA.SetDataList(0, colors);
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
            get { return new Guid("F86D3234-FD2B-46B5-805E-6B89B760FDB0"); }
        }
    }
}