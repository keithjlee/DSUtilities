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
        public int StartIndex;
        public int EndIndex;
        public int ElementID;
        public Section Section;
        public double Psi;
        public Vector3d XAxis;
        public Vector3d YAxis;
        public Vector3d ZAxis;
        public List<double> Forces;
        public double AxialForce;
        public string ID;
        public double A => Section.A;
        public double E => Section.E;
        public double G => Section.G;
        public double Ix => Section.Ix;
        public double Iy => Section.Iy;
        public double J => Section.J;

        public Element() { }

        public Element(int startIndex, int endIndex, int elementID, Section section, double psi, Vector3d xAxis, Vector3d yAxis, Vector3d zAxis, List<double> forces, double axialForce, string iD)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            ElementID = elementID;
            Section = section;
            Psi = psi;
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
            Forces = forces;
            AxialForce = axialForce;
            ID = iD;
        }
    }
}
