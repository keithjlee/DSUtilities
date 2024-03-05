using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    internal static class GroundStructureGeneration
    {
        /// <summary>
        /// Get a list of point positions and a matrix of point indices in grid format
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="dx"></param>
        /// <param name="ny"></param>
        /// <param name="dy"></param>
        /// <param name="origin"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="igrid"></param>
        /// <returns></returns>
        public static List<Point3d> GetPointGrid(int nx, double dx, int ny, double dy, Point3d origin, Vector3d u, Vector3d v, out int[,] igrid, out List<int> iflat)
        {
            //indexers igrid[i][j] = index of point at dx * (i-1), dy * (j-1)
            igrid = new int[nx, ny];
            iflat = new List<int>();

            List<Point3d> points = new List<Point3d>();

            //populate points and indices
            int index = 0;
            for (int i = 0; i < nx; i++)
            {

                for (int j = 0; j < ny; j++)
                {
                    Point3d point = origin + u * dx * (i - 1) + v * dy * (j - 1);

                    //populate
                    points.Add(point);
                    igrid[i, j] = index;
                    iflat.Add(index);

                    index++;
                }
            }

            return points;
        }

        public static void GetDense(List<int> iflat, List<Point3d> points, List<int> istart, List<int> iend, List<Line> lines)
        {

            for (int i = 0; i < points.Count-1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    istart.Add(iflat[i]);
                    iend.Add(iflat[j]);

                    lines.Add(new Line(points[i], points[j]));
                }
            }
        }

        /// <summary>
        /// make ortholinear gridlines on a grid of points
        /// </summary>
        /// <param name="igrid"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="points"></param>
        /// <param name="istart"></param>
        /// <param name="iend"></param>
        /// <param name="lines"></param>
        public static void GetOrthogonals(int[,] igrid, int nx, int ny, List<Point3d> points, List<int> istart, List<int> iend, List<Line> lines)
        {
            //horizontal lines
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx - 1; i++)
                {
                    int i1 = igrid[i, j];
                    int i2 = igrid[i + 1, j];

                    Line line = new Line(points[i1], points[i2]);

                    istart.Add(i1);
                    iend.Add(i2);
                    lines.Add(line);
                }
            }

            //vertical lines
            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; i < ny - 1; j++)
                {
                    int i1 = igrid[i, j];
                    int i2 = igrid[i, j + 1];

                    Line line = new Line(points[i1], points[i2]);

                    istart.Add(i1);
                    iend.Add(i2);
                    lines.Add(line);
                }
            }
        }

        /// <summary>
        /// Get the X diagonals from a grid of points
        /// </summary>
        /// <param name="igrid"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="points"></param>
        /// <param name="istart"></param>
        /// <param name="iend"></param>
        /// <param name="lines"></param>
        public static void GetDiagonals(int[,] igrid, int nx, int ny, List<Point3d> points, List<int> istart, List<int> iend, List<Line> lines)
        {
            //first diagonal set
            for (int i = 0; i < nx - 1; i++ )
            {
                for (int j = 0; j < ny - 1; j++)
                {
                    int i1 = igrid[i, j];
                    int i2 = igrid[i + 1, j + 1];

                    Line line = new Line(points[i1], points[i2]);

                    istart.Add(i1);
                    iend.Add(i2);
                    lines.Add(line);
                }
            }

            //second diagonal set
            for (int i = 0; i < nx - 1; i++)
            {
                for (int j = 1; j < ny; j++)
                {
                    int i1 = igrid[i, j];
                    int i2 = igrid[i + 1, j - 1];

                    Line line = new Line(points[i1], points[i2]);

                    istart.Add(i1);
                    iend.Add(i2);
                    lines.Add(line);
                }
            }
        }

        public static void GetPerimeterIndices(int[,] igrid, out List<int> ix1, out List<int> ix2, out List<int> iy1, out List<int> iy2)
        {
            ix1 = new List<int>();
            ix2 = new List<int>();
            iy1 = new List<int>();
            iy2 = new List<int>();

            //
            int nx = igrid.GetLength(0);
            int ny = igrid.GetLength(1);

            for (int i = 0; i < nx; i++)
            {
                ix1.Add(igrid[i, 0]);
                ix2.Add(igrid[i, ny - 1]);
            }

            for (int j = 0; j < ny; j++)
            {
                iy1.Add(igrid[0, j]);
                iy2.Add(igrid[nx-1, j]);
            }
        }

        public static List<int> GetInteriorIndices(int[,] igrid)
        {
            List<int> indices = new List<int>();

            for (int i = 1; i < igrid.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < igrid.GetLength(1) - 1; j++)
                {
                    indices.Add(igrid[i, j]);
                }
            }

            return indices;
        }

    }
}
