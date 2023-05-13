using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Section
{
    public class MultiSectionAnalysis
    {

        /// <summary>
        /// Makes sure all sections in a multisection are in the same plane
        /// </summary>
        /// <param name="section"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static bool ValidCheck(Section section, List<Section> sections)
        {
            var plane = section.Plane;

            foreach (Section sec in sections)
            {
                if (plane != sec.Plane) return false;
            }

            return true;
        }

        public double Atotal(Section section, List<Section> sections)
        {
            var A = section.Area;

            foreach (Section sec in sections)
            {
                A += sec.Area;
            }

            return A;
        }

        public static bool ValidCheck2(Section section, List<Section> sections)
        {
            bool flag = false; //are the E values different?

            foreach (Section sec in sections)
            {
                if (sec.E != section.E) flag = true;
            }

            return flag;
        }

    }
}
