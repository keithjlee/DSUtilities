using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using MathNet.Numerics;
using Rhino.Collections;

namespace DSUtilities.Section
{
    /// <summary>
    /// A geometric section
    /// </summary>
    public class Section
    {
        public Brep Geo;
        public Brep AnalysisGeo;
        public BoundingBox BoundingBox;
        public Box LocalBox;
        public Plane Plane;
        public AreaMassProperties Properties => AreaMassProperties.Compute(AnalysisGeo);
        public double Depth => BoundingBox.Corner(true, true, true).DistanceTo(BoundingBox.Corner(true, false, true));
        public double Width => BoundingBox.Corner(true, true, true).DistanceTo(BoundingBox.Corner(false, true, true));
        public double Area => Properties.Area;
        public Point3d Centroid => Plane.Origin;
        public double Ix => Properties.CentroidCoordinatesMomentsOfInertia[0];
        public double Iy => Properties.CentroidCoordinatesMomentsOfInertia[1];
        public double Rx => Properties.CentroidCoordinatesRadiiOfGyration[0];
        public double Ry => Properties.CentroidCoordinatesRadiiOfGyration[1];
        public double Xmin => BoundingBox.Corner(true, true, true).X;
        public double Xmax => BoundingBox.Corner(false, true, true).X;
        public double Ymin => BoundingBox.Corner(true, true, true).Y;
        public double Ymax => BoundingBox.Corner(true, false, true).Y;
        public double Sx1 => Ix / Math.Abs(Ymax);
        public double Sx2 => Ix / Math.Abs(Ymin);
        public double Sy1 => Iy / Math.Abs(Xmin);
        public double Sy2 => Iy / Math.Abs(Xmax);


        public Section()
        {

        }

        public Section(Brep geometry, Plane plane)
        {
            //Input geometry
            this.Geo = geometry;

            //Input plane
            this.Plane = plane;
            this.Plane.Origin = AreaMassProperties.Compute(this.Geo).Centroid;

            //transform to XY plane
            Transform p2p = Transform.PlaneToPlane(plane, Plane.WorldXY);
            this.AnalysisGeo = this.Geo.DuplicateBrep();
            this.AnalysisGeo.Transform(p2p);


            // Bounding box
            BoundingBox = Geo.GetBoundingBox(this.Plane, out this.LocalBox);
        }


    }

}
