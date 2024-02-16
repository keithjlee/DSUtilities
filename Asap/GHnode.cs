using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Asap
{
    public class GHnode
    {
        public List<double> position { get; set; }
        public List<bool> dof { get; set; }
        public int nodeID { get; set; }
        public List<double> reaction { get; set; }
        public List<double> u {  get; set; }
        public List<double> displacement { get; set; }
        public string id { get; set; }

        public Node ToNode()
        {
            Point3d pos = new Point3d(position[0], position[1], position[2]);
            Vector3d force_reaction = new Vector3d(reaction[0], reaction[1], reaction[2]);
            Vector3d disp = new Vector3d(displacement[0], displacement[1], displacement[2]);

            return new Node(pos, dof, nodeID, reaction, force_reaction, u, disp, id);
        }
    }
}
