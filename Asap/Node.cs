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
        public Point3d Position;
        public List<bool> DOF;
        public int NodeID;
        public List<double> Reaction;
        public Vector3d ForceReaction;
        public List<double> U;
        public Vector3d Displacement;
        public string ID;

        public Node()
        {

        }

        public Node(Point3d position, List<bool> dOF, int nodeID, List<double> reaction, Vector3d forceReaction, List<double> u, Vector3d displacement, string iD)
        {
            Position = position;
            DOF = dOF;
            NodeID = nodeID;
            Reaction = reaction;
            ForceReaction = forceReaction;
            U = u;
            Displacement = displacement;
            ID = iD;
        }
    }
}
