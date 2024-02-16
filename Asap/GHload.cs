using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Asap
{
    public abstract class GHload
    {
        public List<double> value {  get; set; }
        public string id {  get; set; }

    }

    public class GHnodeforce : GHload
    {
        public int iNode {  get; set; }


    }

    public class GHnodemoment : GHload
    {
        public int iNode { get; set; }
    }

    public class GHlineload : GHload
    {
        public int iElement { get; set; }
    }

    public class GHpointload : GHload
    {
        public int iElement { get; set; }
        public double x { get; set; }
    }
}
