using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    public class Truss
    {
        public List<Point3d> Nodes;
        public List<Line> Lines;
        public List<int> Istart;
        public List<int> Iend;
        public List<int> Ichord1nodes;
        public List<int> Ichord2nodes;
        public List<int> Ichord1elements;
        public List<int> Ichord2elements;
        public List<int> Iwebelements;
        public List<Vector3d> Offset;
        public double Depth;

        public Truss()
        {

        }

        public Truss(List<Point3d> nodes, List<Line> lines, List<int> istart, List<int> iend, List<int> ichord1nodes, List<int> ichord2nodes, List<int> ichord1elements, List<int> ichord2elements, List<int> iweb, List<Vector3d> offset, double depth)
        {
            Nodes = nodes;
            Lines = lines;
            Istart = istart;
            Iend = iend;
            Ichord1nodes = ichord1nodes;
            Ichord2nodes = ichord2nodes;
            Ichord1elements = ichord1elements;
            Ichord2elements = ichord2elements;
            Iwebelements = iweb;
            Offset = offset;
            Depth = depth;
        }

        public void RebuildLines()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Lines[i] = new Line(Nodes[Istart[i]], Nodes[Iend[i]]);
            }
        }

        public static Truss Pratt(Curve curve, int nbays, double depth, Vector3d offset_override, double angle)
        {
            // parameters t where nodes exist
            double[] params1 = curve.DivideByCount(nbays, true, out Point3d[] _);

            // get offset params 
            double[] params2 = new double[params1.Length - 2];
            for (int i = 1; i < params1.Length - 1; i++)
            {
                params2[i-1] = params1[i];
            }

            // get offset vectors
            List<Vector3d> offset_vectors = GetOffsetVectors(curve, params2, offset_override, angle);

            // populate nodes
            int node_index = 0;
            List<int> ichord1nodes = new List<int>();
            List<int> ichord2nodes = new List<int>();
            List<Point3d> nodes = new List<Point3d>();

            //base chord
            foreach (double t in params1)
            {
                nodes.Add(curve.PointAt(t));
                ichord1nodes.Add(node_index);
                node_index++;
            }

            //offset chord
            for (int i = 0; i < params2.Length; i++)
            {
                nodes.Add(curve.PointAt(params2[i]) + depth * offset_vectors[i]);
                ichord2nodes.Add(node_index);
                node_index++;
            }

            //make edges
            int element_index = 0;
            List<Line> elements = new List<Line>();
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();

            element_index = GetChordElements(element_index, elements, nodes, istart, iend, ichord1nodes, ichord2nodes, out List<int> ichord1elements, out List<int> ichord2elements);

            _ = GetPrattWebElements(element_index, elements, nodes, istart, iend, ichord1nodes, ichord2nodes, out List<int> iwebelements);

            return new Truss(nodes, elements, istart, iend, ichord1nodes, ichord2nodes, ichord1elements, ichord2elements, iwebelements, offset_vectors, depth);
        }

        public static Truss Warren(Curve curve, int nbays, double depth, Vector3d offset_override, double angle)
        {
            // parameters t where nodes exist
            double[] params1 = curve.DivideByCount(nbays, true, out Point3d[] _);

            // get offset params 
            double[] params2 = new double[params1.Length - 1];
            for (int i = 0; i < params1.Length-1; i++)
            {
                params2[i] = (params1[i] + params1[i + 1]) / 2;
            }

            // get offset vectors
            List<Vector3d> offset_vectors = GetOffsetVectors(curve, params2, offset_override, angle);

            // populate nodes
            int node_index = 0;
            List<int> ichord1nodes = new List<int>();
            List<int> ichord2nodes = new List<int>();
            List<Point3d> nodes = new List<Point3d>();

            //base chord
            foreach (double t in params1)
            {
                nodes.Add(curve.PointAt(t));
                ichord1nodes.Add(node_index);
                node_index++;
            }

            //offset chord
            for (int i = 0; i < params2.Length; i++)
            {
                nodes.Add(curve.PointAt(params2[i]) + depth * offset_vectors[i]);
                ichord2nodes.Add(node_index);
                node_index++;
            }

            //make edges
            int element_index = 0;
            List<Line> elements = new List<Line>();
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();

            element_index = GetChordElements(element_index, elements, nodes, istart, iend, ichord1nodes, ichord2nodes, out List<int> ichord1elements, out List<int> ichord2elements);

            _ = GetWarrenWebElements(element_index, elements, nodes, istart, iend, ichord1nodes, ichord2nodes, out List<int> iwebelements);

            return new Truss(nodes, elements, istart, iend, ichord1nodes, ichord2nodes, ichord1elements, ichord2elements, iwebelements, offset_vectors, depth);
        }

        public static int GetPrattWebElements(int index, List<Line> edges, List<Point3d> nodes, List<int> istart, List<int> iend, List<int> chord1indices, List<int> chord2indices, out List<int> iwebelements)
        {
            iwebelements = new List<int>();

            //verticals
            for (int i = 0; i < chord2indices.Count; i++)
            {
                int i1 = chord1indices[i + 1];
                int i2 = chord2indices[i];

                edges.Add(new Line(nodes[i1], nodes[i2]));
                istart.Add(i1);
                iend.Add(i2);

                iwebelements.Add(index);
                index++;
            }

            int i_halfway = (int) ((chord1indices.Count - 1) / 2);
            //diagonals
            for (int i = 0; i < i_halfway; i++)
            {
                int i1 = chord1indices[i];
                int i2 = chord2indices[i];

                edges.Add(new Line(nodes[i1], nodes[i2]));
                istart.Add(i1);
                iend.Add(i2);

                iwebelements.Add(index);
                index++;

                i1 = chord1indices[chord1indices.Count - 1 - i];
                i2 = chord2indices[i1 - 2];

                edges.Add(new Line(nodes[i1], nodes[i2]));
                istart.Add(i1);
                iend.Add(i2);

                iwebelements.Add(index);
                index++;
            }

            return index;
        }

        public static int GetWarrenWebElements(int index, List<Line> edges, List<Point3d> nodes, List<int> istart, List<int> iend, List<int> chord1indices, List<int> chord2indices, out List<int> iwebelements)
        {
            iwebelements = new List<int>();

            for (int i = 0; i < chord2indices.Count; i++)
            {
                //first set of diagonals
                int i1 = chord1indices[i];
                int i2 = chord2indices[i];

                edges.Add(new Line(nodes[i1], nodes[i2]));
                istart.Add(i1);
                iend.Add(i2);

                iwebelements.Add(index);
                index++;

                i1 = chord1indices[i + 1];
                edges.Add(new Line(nodes[i1], nodes[i2]));
                istart.Add(i1);
                iend.Add(i2);

                iwebelements.Add(index);
                index++;
            }

            return index;
            
        }

        public static int GetChordElements(int index, List<Line> edges, List<Point3d> nodes, List<int> istart, List<int> iend, List<int> chord1indices, List<int> chord2indices, out List<int> ichord1elements, out List<int> ichord2elements)
        {
            ichord1elements = new List<int>();
            ichord2elements = new List<int>();

            //chord1
            for (int i = 0; i < chord1indices.Count - 1; i++)
            {
                int i1 = chord1indices[i];
                int i2 = chord1indices[i + 1];
                
                edges.Add(new Line(nodes[i1], nodes[i2]));
                
                istart.Add(i1);
                iend.Add(i2);

                ichord1elements.Add(index);
                index++;
            }

            //chord1
            for (int i = 0; i < chord2indices.Count - 1; i++)
            {
                int i1 = chord2indices[i];
                int i2 = chord2indices[i + 1];

                edges.Add(new Line(nodes[i1], nodes[i2]));

                istart.Add(i1);
                iend.Add(i2);

                ichord2elements.Add(index);
                index++;
            }

            return index;
        }

        public static List<Vector3d> GetOffsetVectors(Curve curve, double[] t, Vector3d vector_override,  double angle)
        {
            // perpendicular frames
            Plane[] perp_frames = curve.GetPerpendicularFrames(t);

            //initialize
            List<Vector3d> v_offset = new List<Vector3d>();

            //make offset vectors
            if (vector_override.Length == 0)
            {
                v_offset = perp_frames.Select(frame => frame.YAxis).ToList();
            }
            else
            {
                vector_override.Unitize();
                v_offset = Enumerable.Repeat(vector_override, perp_frames.Length).ToList();
            }

            //rotate vectors if applicable
            if (angle != 0)
            {
                List<Vector3d> rotation_axes = perp_frames.Select(frame => frame.ZAxis).ToList();

                for (int i = 0; i < perp_frames.Length; i++)
                {
                    var v = v_offset[i];
                    v.Rotate(angle, rotation_axes[i]);
                    v_offset[i] = v;
                }
            }

            return v_offset;
        }

    }
}
