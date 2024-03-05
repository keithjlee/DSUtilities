using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Generators
{
    public class GroundStructure
    {
        public int Nx;
        public double Dx;
        public double Lx => Nx * Dx;
        public int Ny;
        public double Dy;
        public double Ly => Ny * Dy;
        public int[,] Igrid;
        public List<int> Istart;
        public List<int> Iend;
        public List<Point3d> Nodes;
        public List<Line> Lines;
    }


}
