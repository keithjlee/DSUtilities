﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Generators
{
    internal class GridGroundStructure : GroundStructure
    {
        public GridGroundStructure(Point3d origin, Vector3d u, int nx, double dx, Vector3d v, int ny, double dy)
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

            //populate
            Igrid = igrid;
            Istart = istart;
            Iend = iend;
            Nodes = points;
            Lines = lines;
        }

        public GridGroundStructure(Surface surface, int nx, int ny)
        {
            GroundStructureGeneration.InitializeDomain(surface, nx, ny, out double dx, out double dy);

            //populate
            Nx = nx;
            Dx = dx;
            Ny = ny;
            Dy = dy;

            List<Point3d> points = GroundStructureGeneration.GetPointGrid(surface, nx, dx, ny, dy, out int[,] igrid, out List<int> _);

            // get X grid lines
            List<int> istart = new List<int>();
            List<int> iend = new List<int>();
            List<Line> lines = new List<Line>();

            // populate with grid
            GroundStructureGeneration.GetOrthogonals(igrid, nx, ny, points, istart, iend, lines);

            //populate
            Igrid = igrid;
            Istart = istart;
            Iend = iend;
            Nodes = points;
            Lines = lines;
        }
    }
}
