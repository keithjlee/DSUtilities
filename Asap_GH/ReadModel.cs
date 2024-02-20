using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;
using DSUtilities.Asap;
using System.Linq;

namespace DSUtilities.Asap_GH
{
    public class ReadModel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReadModel class.
        /// </summary>
        public ReadModel()
          : base("ReadModel", "ReadModel",
              "Read an AsapModel",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FilePath", "Path", "Path to .json file", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("NodePositions", "Pos", "Positions of nodes", GH_ParamAccess.list);
            pManager.AddVectorParameter("NodeDisplacements", "Disp", "Displacement vectors of nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iStart", "iStart", "Start indices", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iEnd", "iEnd", "End indices", GH_ParamAccess.list);
            pManager.AddNumberParameter("AxialForces", "Faxial", "Axial forces", GH_ParamAccess.list);
            pManager.AddGenericParameter("Model", "Model", "Asap Model", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string fn = "";

            if (!DA.GetData(0, ref fn)) return;

            string data = System.IO.File.ReadAllText(fn);

            GHmodel model = JsonConvert.DeserializeObject<GHmodel>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            Model asap_model = model.ToModel();
            List<double> forces = asap_model.Elements.Select(element => element.AxialForce).ToList();

            DA.SetDataList(0, asap_model.Positions);
            DA.SetDataList(1, asap_model.Displacements);
            DA.SetDataList(2, asap_model.StartIndices);
            DA.SetDataList(3, asap_model.EndIndices);
            DA.SetDataList(4, forces);
            DA.SetData(5, asap_model);
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
            get { return new Guid("FD79099E-DA82-424A-A1DA-23E56645439C"); }
        }
    }
}