using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    public class XGroundStructure : GroundStructure
    {
        public XGroundStructure(Point3d origin, Vector3d u, int nx, double dx, Vector3d v, int ny, double dy)
        {
            //populate
            Nx = nx;
            Dx = dx;
            Ny = ny;
            Dy = dy;

            //Get the list of points and a matrix of point indices
            List<Point3d> points = GroundStructureGeneration.GetPointGrid(nx, dx, ny, dy, origin, u, v, out int[,] igrid, out List<int> _);

            // get X grid lines
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();
            List<Line> lines = new List<Line>();

            // populate with grid
            GroundStructureGeneration.GetOrthogonals(igrid, nx, ny, points, istart, iend, lines);

            // populate with diagonals
            GroundStructureGeneration.GetDiagonals(igrid, nx, ny, points, istart, iend, lines);

            //populate
            Igrid = igrid;
            Istart = istart;
            Iend = iend;
            Nodes = points;
            Lines = lines;
        }
    }
}
