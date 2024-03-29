﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Colours
{
    public class Pink : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Pink class.
        /// </summary>
        public Pink()
          : base("Pink", "Pink",
              "Pink",
              "DSutilities", "Colours")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Alpha", "Alpha", "Alpha value of colour [0, 255]", GH_ParamAccess.item, 255);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Pink", "Pink", "#F52565, (245, 37, 101)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int alpha = 255;
            DA.GetData(0, ref alpha);
            DA.SetData(0, Colours.MakePink(alpha));
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
            get { return new Guid("3ADD54FE-A96D-44F1-B5DC-64E74EE1F9C0"); }
        }
    }
}