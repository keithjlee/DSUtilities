using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Asap
{
    public class Element
    {
        public Section Section;
        public Node StartNode;
        public Node EndNode;
        public int ElementID;
        public List<int> GlobalIDs;
        public List<int> LoadIDs;
        public double Psi;
        public List<double> LocalForces;
        public string ID;
        public double A => Section.A;
        public double E => Section.E;
        public double G => Section.G;
        public double Ix => Section.Ix;
        public double Iy => Section.Iy;
        public double J => Section.J;
        public int Istart => StartNode.NodeID;
        public int Iend => EndNode.NodeID;
        public double Length => StartNode.Position.DistanceTo(EndNode.Position);
    }
}
