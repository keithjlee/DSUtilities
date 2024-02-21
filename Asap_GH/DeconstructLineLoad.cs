﻿using System;
using System.Collections.Generic;
using DSUtilities.Asap;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Asap_GH
{
    public class DeconstructLineLoad : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructLineLoad class.
        /// </summary>
        public DeconstructLineLoad()
          : base("DeconstructLineLoad", "DeconLineLoad",
              "Deconstruct a line load applied to an element",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("LineLoad", "LineLoad", "Line load to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("ElementIndex", "iElement", "Index of element that load is applied to", GH_ParamAccess.item);
            pManager.AddVectorParameter("Load", "Load", "Load vector", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GHlineload load = new GHlineload();
            if (!DA.GetData(0, ref load)) return;

            Vector3d w = new Vector3d(load.value[0], load.value[1], load.value[2]);

            DA.SetData(0, load.iElement);
            DA.SetData(1, w);
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
            get { return new Guid("5A5DF86D-6F6E-4126-A50D-6E3FAA4CF9AA"); }
        }
    }
}