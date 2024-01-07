using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Asap
{
    public class Section
    {
        public double A;
        public double E;
        public double G;
        public double Ix;
        public double Iy;
        public double J;

        public Section()
        {

        }

        public Section(double A, double E)
        {
            this.A = A;
            this.E = E;
            G = 1;
            Ix = 1;
            Iy = 1;
            J = 1;
        }

        public Section(double A, double E, double G, double Ix, double Iy, double J)
        {
            this.A = A;
            this.E = E;
            this.G = G;
            this.Ix = Ix;
            this.Iy = Iy;
            this.J = J;
        }
    }
}
