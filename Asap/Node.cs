using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Asap
{
    public class Node
    {
        public bool IsTruss;
        public Point3d Position;
        public List<bool> DOF;
        public int NodeID;
        public List<int> GlobalIDs;
        public List<int> LoadIDs;
        public List<double> Reactions;
        public List<double> Displacement;
        public string ID;
        public double X => Position.X;
        public double Y => Position.Y;
        public double Z => Position.Z;

        public Node() { }
        public Node(Point3d position, string id)
        {
            Position = position;
            ID = id;
            DOF = new List<bool> { true, true, true, true, true, true };
        }

    }
}
