﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Section
{
    internal static class Analysis
    {
        public static double tol = 1e-2;

        /// <summary>
        /// Checks if sections are closed and planar
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static bool ValidCheck(List<Curve> curves)
        {
            for (int i = 0; i < curves.Count; i++)
            {
                Curve curve = curves[i];
                if (curve.IsClosed && curve.IsPlanar())
                {
                    continue;
                }
                else
                {
                    return false; //if any of the curves are open or non-planar, return false
                }
            }

            return true; //else return true
        }

        /// <summary>
        /// Computes the total area of an arbitrary section
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static double TotalArea(List<Curve> sections)
        {
            double Atotal = 0.0;
            for (int i = 0; i < sections.Count; i++)
            {
                Curve curve = sections[i];
                var curveProp = AreaMassProperties.Compute(curve);
                Atotal += curveProp.Area;
            }

            return Atotal;
        }

        /// <summary>
        /// Get the bounding box of a collection of curves
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static BoundingBox GetBB(List<Curve> curves)
        {
            BoundingBox bb = curves[0].GetBoundingBox(true);

            if (curves.Count > 1)
            {
                for (int i = 1; i < curves.Count; i++)
                {
                    bb.Union(curves[i].GetBoundingBox(true));
                }
            }

            return bb;
        }

        /// <summary>
        /// Reference planes of analysis: first letter is the "Strong" axis (assumed horizontal)
        /// </summary>
        public enum AnalysisPlane
        {
            XY,
            YZ,
            XZ
        }


        public static int StrongDir(Analysis.AnalysisPlane plane)
        {
            switch (plane)
            {
                case Analysis.AnalysisPlane.XY:
                    return 1;
                case Analysis.AnalysisPlane.YZ:
                    return 2;
                case Analysis.AnalysisPlane. XZ:
                    return 2;
                default: return 1;
            }
        }

        public static int WeakDir(Analysis.AnalysisPlane plane)
        {
            switch (plane)
            {
                case Analysis.AnalysisPlane.XY:
                    return 0;
                case Analysis.AnalysisPlane.YZ:
                    return 1;
                case Analysis.AnalysisPlane.XZ:
                    return 0;
                default: return 0;
            }
        }

    }
}