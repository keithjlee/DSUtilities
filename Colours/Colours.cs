using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.GUI.Gradient;

namespace DSUtilities.Colours
{
    internal static class Colours
    {
        public static Color Blue = Color.FromArgb(37, 89, 245);
        public static Color Pink = Color.FromArgb(245, 37, 101);
        public static Color Green = Color.FromArgb(88, 188, 130);
        public static Color Orange = Color.FromArgb(245, 187, 37);

        public static List<Color> MakeGradient(List<double> values, double factor, Color min_color, Color max_color, bool center_zero)
        {
            var extremes = GetMinMax(values, center_zero, factor);
            double minval = extremes.Item1;
            double maxval = extremes.Item2;

            GH_Gradient grad = new GH_Gradient();
            grad.AddGrip(minval, min_color);
            grad.AddGrip(maxval, max_color);

            return ColorValues(values, grad);
        }

        public static List<Color> MakeGradient(List<double> values, double factor, Color min_color, Color neutral_color, Color max_color, bool center_zero)
        {
            var extremes = GetMinMax(values, center_zero, factor);
            double minval = extremes.Item1;
            double maxval = extremes.Item2;

            double neutralval = 0.5 * (minval + maxval);

            GH_Gradient grad = new GH_Gradient();
            grad.AddGrip(minval, min_color);
            grad.AddGrip(neutralval, neutral_color);
            grad.AddGrip(maxval, max_color);

            return ColorValues(values, grad);
        }

        public static Tuple<double, double> GetMinMax(List<double> values, bool center, double factor)
        {
            double minval = values.Min();
            double maxval = values.Max();

            // "center" only applies if min is negative and max is positive
            if (minval < 0 && maxval > 0 && center)
            {
                double max_abs_val = Math.Max(Math.Abs(minval), Math.Abs(maxval));
                minval = -max_abs_val * factor;
                maxval = max_abs_val * factor;
            }
            else
            {
                double diff = maxval - minval;
                double offset = diff / 2 * factor;

                minval += offset;
                maxval -= offset;
            }

            return Tuple.Create(minval, maxval);
        }

        public static List<Color> ColorValues(List<double> values, GH_Gradient grad)
        {
            List<Color> colors = new List<Color>();
            foreach (double val in values)
            {
                colors.Add(grad.ColourAt(val));
            }

            return colors;
        }


    }
}
