using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Asap
{
    public class Model
    {
        public List<Node> Nodes;
        public List<Element> Elements;
        public List<GHload> Loads;
        public List<Point3d> Positions;
        public List<Vector3d> Displacements;
        public List<int> StartIndices;
        public List<int> EndIndices;
        public List<int> FreeIndices;
        public List<int> SupportIndices;

        public Model()
        {

        }

        public Model(List<Node> nodes, List<Element> elements, List<GHload> loads, List<Point3d> positions, List<Vector3d> displacements, List<int> startIndices, List<int> endIndices, List<int> freeIndices, List<int> supportIndices)
        {
            Nodes = nodes;
            Elements = elements;
            Loads = loads;
            Positions = positions;
            Displacements = displacements;
            StartIndices = startIndices;
            EndIndices = endIndices;
            FreeIndices = freeIndices;
            SupportIndices = supportIndices;
        }
    }
}
