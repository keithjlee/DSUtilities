using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Collections;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    internal class SpaceFrame
    {
        public int Nx;
        public double Dx;
        public int Ny;
        public double Dy;
        public double Dz;
        public int[,] Igrid1;
        public int[,] Igrid2;
        public List<Point3d> Nodes;
        public List<Line> Lines;
        public List<int> Istart;
        public List<int> Iend;
        public List<int> Ichord1;
        public List<int> Ichord2;
        public List<int> Iweb;


        public SpaceFrame(Plane plane, int nx, double dx, int ny, double dy, double dz, List<Point3d> targets)
        {
            //Populate
            Nx = nx;
            Dx = dx;
            Ny = ny;
            Dy = dy;
            Dz = dz;

            //get nodes
            List<Point3d> points = GetPlanarPointGrid(plane, nx, dx, ny, dy, dz, out int[,] igrid1, out int[,] igrid2);

            //adjust nodes
            if (targets.Count > 0) AdjustToTargetPoints(points, targets);

            //Orthogonal lines
            // get X grid lines
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();
            List<Line> lines = new List<Line>();

            //initialize
            int element_index_start = lines.Count;

            //main chord
            GroundStructureGeneration.GetOrthogonals(igrid1, nx, ny, points, istart, iend, lines);

            //indices of main chord elements
            List<int> ichord1 = Enumerable.Range(element_index_start, lines.Count).ToList();
            element_index_start = lines.Count;

            //offset chord
            GroundStructureGeneration.GetOrthogonals(igrid2, nx - 1, ny - 1, points, istart, iend, lines);

            //indices of offset chord elements
            List<int> ichord2 = Enumerable.Range(element_index_start, lines.Count-element_index_start).ToList();
            element_index_start = lines.Count;

            //web
            GetWebs(igrid1, igrid2, points, istart, iend, lines);

            //indices of web elements
            List<int> iweb = Enumerable.Range(element_index_start, lines.Count - element_index_start).ToList();

            Igrid1 = igrid1;
            Igrid2 = igrid2;
            Nodes = points;
            Lines = lines;
            Istart = istart;
            Iend = iend;
            Ichord1 = ichord1;
            Ichord2 = ichord2;
            Iweb = iweb;
        }

        public SpaceFrame(Surface surface, int nx, int ny, double dz, List<Point3d> targets)
        {
            //get domain parameters
            GroundStructureGeneration.InitializeDomain(surface, nx, ny, out double dx, out double dy);

            //Populate
            Nx = nx;
            Dx = dx;
            Ny = ny;
            Dy = dy;
            Dz = dz;

            //offset surface
            Surface offset_surface = surface.Offset(dz, 1e-6);

            //get points
            List<Point3d> points = GetSurfacePointGrid(surface, offset_surface, nx, dx, ny, dy, out int[,] igrid1, out int[,] igrid2);

            //adjust nodes
            if (targets.Count > 0) AdjustToTargetPoints(points, targets);

            //Orthogonal lines
            // get X grid lines
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();
            List<Line> lines = new List<Line>();

            //initialize
            int element_index_start = lines.Count;

            //main chord
            GroundStructureGeneration.GetOrthogonals(igrid1, nx, ny, points, istart, iend, lines);

            //indices of main chord elements
            List<int> ichord1 = Enumerable.Range(element_index_start, lines.Count).ToList();
            element_index_start = lines.Count;

            //offset chord
            GroundStructureGeneration.GetOrthogonals(igrid2, nx - 1, ny - 1, points, istart, iend, lines);

            //indices of offset chord elements
            List<int> ichord2 = Enumerable.Range(element_index_start, lines.Count - element_index_start).ToList();
            element_index_start = lines.Count;

            //web
            GetWebs(igrid1, igrid2, points, istart, iend, lines);

            //indices of web elements
            List<int> iweb = Enumerable.Range(element_index_start, lines.Count - element_index_start).ToList();

            Igrid1 = igrid1;
            Igrid2 = igrid2;
            Nodes = points;
            Lines = lines;
            Istart = istart;
            Iend = iend;
            Ichord1 = ichord1;
            Ichord2 = ichord2;
            Iweb = iweb;

        }

        /// <summary>
        /// For each point in target_points, adjust the closest point in initial_points such that they coincide
        /// </summary>
        /// <param name="initial_points"></param>
        /// <param name="target_points"></param>
        public void AdjustToTargetPoints(List<Point3d> initial_points, List<Point3d> target_points)
        {
            Point3dList initial = new Point3dList(initial_points);
            
            foreach (Point3d target in target_points)
            {
                int index = initial.ClosestIndex(target);

                initial_points[index] = new Point3d(target);
            }


        }

        public List<Point3d> GetSurfacePointGrid(Surface surface1, Surface surface2, int nx, double dx, int ny, double dy, out int[,] igrid1, out int[,] igrid2)
        {
            //initialize
            int index = 0;
            List<Point3d> points = new List<Point3d>();
            igrid1 = new int[nx, ny];
            igrid2 = new int[nx - 1, ny - 1];

            //main grid
            for (int i = 0; i < nx; i++)
            {

                for (int j = 0; j < ny; j++)
                {
                    Point3d point = surface1.PointAt(dx * i, dy * j);

                    //populate
                    points.Add(point);
                    igrid1[i, j] = index;

                    index++;
                }
            }

            //offset grid
            for (int i = 0; i < nx-1; i++)
            {

                for (int j = 0; j < ny-1; j++)
                {
                    Point3d point = surface2.PointAt(dx * i + dx/2, dy * j + dy/2);

                    //populate
                    points.Add(point);
                    igrid2[i, j] = index;

                    index++;
                }
            }

            return points;
        }

        public List<Point3d> GetPlanarPointGrid(Plane plane, int nx, double dx, int ny, double dy, double dz, out int[,] igrid1, out int[,] igrid2)
        {
            //initialize
            int index = 0;
            List<Point3d> points = new List<Point3d>();
            igrid1 = new int[nx, ny];
            igrid2 = new int[nx - 1, ny - 1];

            //main chord
            for (int i = 0; i < nx; i++)
            {

                for (int j = 0; j < ny; j++)
                {
                    Point3d point = plane.Origin + plane.XAxis * dx * i + plane.YAxis * dy * j;

                    //populate
                    points.Add(point);
                    igrid1[i, j] = index;

                    index++;
                }
            }

            //offset chord
            Vector3d offset = dx / 2 * plane.XAxis + dy / 2 * plane.YAxis + dz * plane.ZAxis;

            for (int i = 0; i < nx-1; i++)
            {
                for (int j = 0; j < ny-1; j++)
                {
                    Point3d point = plane.Origin + plane.XAxis * dx * i + plane.YAxis * dy * j + offset;

                    //populate
                    points.Add(point);
                    igrid2[i, j] = index;

                    index++;
                }
            }

            return points;

        }

        public static void GetWebs(int[,] igrid1, int[,] igrid2, List<Point3d> points, List<int> istart, List<int> iend, List<Line> lines)
        {
            int nx = igrid2.GetLength(0);
            int ny = igrid2.GetLength(1);

            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    int top_index = igrid2[i, j];

                    int[] bottom_indices = new int[4] { igrid1[i, j], igrid1[i + 1, j], igrid1[i, j + 1], igrid1[i + 1, j + 1] };

                    foreach (int bottom_index in bottom_indices)
                    {
                        istart.Add(top_index);
                        iend.Add(bottom_index);

                        lines.Add(new Line(points[bottom_index], points[top_index]));

                    }
                }
            }
        }
    }
}
