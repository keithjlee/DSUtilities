using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Topology
{
    internal class TopologyObject
    {
        public List<double> X;
        public List<double> Y;
        public List<double> Z;
        public List<int> Istarts;
        public List<int> Iends;
        public List<int> Inodes;

        public TopologyObject(List<double> x, List<double> y, List<double> z, List<int> istarts, List<int> iends, List<int> inodes)
        {
            X = x;
            Y = y;
            Z = z;
            Istarts = istarts;
            Iends = iends;
            Inodes = inodes;
        }
    }
}
