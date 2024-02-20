using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Asap
{
    public class GHelement
    {
        public int iStart {  get; set; }
        public int iEnd { get; set; }
        public int elementID { get; set; }
        public Section section { get; set; }
        public List<bool> release {  get; set; }
        public double psi {  get; set; }
        public List<double> localx { get; set; }
        public List<double> localy { get; set; }
        public List<double> localz { get; set; }
        public List<double> forces { get; set; }
        public double axialforce { get; set; }
        public string id { get; set; }

        public Element ToElement()
        {
            Vector3d X = new Vector3d(localx[0], localx[1], localx[2]);
            Vector3d Y = new Vector3d(localy[0], localy[1], localy[2]);
            Vector3d Z = new Vector3d(localz[0], localz[1], localz[2]);

            return new Element(iStart, iEnd, elementID, section, release, psi, X, Y, Z, forces, axialforce, id);
        }
    }
}
