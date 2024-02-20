using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Asap
{
    public class GHmodel
    {
        public List<GHnode> nodes {  get; set; }
        public List<GHelement> elements { get; set; }
        public List<GHnodeforce> nodeforces {  get; set; }
        public List<GHnodemoment> nodemoments {  get; set; }
        public List<GHlineload> lineloads { get; set; }
        public List<GHpointload> pointloads { get; set; }
        public List<double> x { get; set; }
        public List<double> y { get; set; }
        public List<double> z { get; set; }
        public List<double> dx { get; set; }
        public List<double> dy { get; set; }
        public List<double> dz { get; set; }
        public List<int> istart { get; set; }
        public List<int> iend { get; set; }
        public List<int> i_free_nodes { get; set; }
        public List<int> i_fixed_nodes { get; set; }

        public Model ToModel()
        {
            // convert nodes
            List<Node> new_nodes = new List<Node>();
            nodes.ForEach(
                (node) =>
                {
                    new_nodes.Add(node.ToNode());
                }
                );

            // convert elements
            List<Element> new_elements = new List<Element>();
            elements.ForEach(
                (element) =>
                {
                    new_elements.Add(element.ToElement());
                }
                );

            // convert positions and displacements
            List<Point3d> positions = new List<Point3d>();
            List<Vector3d> displacements = new List<Vector3d>();

            for (int i = 0; i <  new_nodes.Count; i++)
            {
                Point3d pos = new Point3d(x[i], y[i], z[i]);
                Vector3d disp = new Vector3d(dx[i], dy[i], dz[i]);

                positions.Add(pos);
                displacements.Add(disp);
            }

            //collect loads
            List<GHload> loads = new List<GHload>();

            foreach (GHnodeforce load in nodeforces) { loads.Add(load); }
            foreach (GHnodemoment moment in nodemoments) { loads.Add(moment); }
            foreach (GHlineload lineload in lineloads) { loads.Add(lineload); }
            foreach (GHpointload pointload in pointloads) { loads.Add(pointload); }

            return new Model(new_nodes, new_elements, loads, positions, displacements, istart, iend, i_free_nodes, i_fixed_nodes);
    }
    }

    
}
