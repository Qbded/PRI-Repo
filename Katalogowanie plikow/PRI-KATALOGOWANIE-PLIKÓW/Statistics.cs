using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    internal class Statistics
    {
        protected double[] Quartile(int range, List<float> input)
        {
            if (range > 3) return null;
            double[] quartile = new double[2];
            double[] median = new double[2];
            median = this.Median(input);
            input.Sort();
            if (range == 2) quartile = median;
            else
            {
                if (input.Count % 2 == 0)
                {
                    if (input[input.Count * range / 4 - 1] == input[input.Count * range / 4])
                    {
                        quartile[0] = input[input.Count * range / 4 - 1];
                        quartile[1] = quartile[0];
                    }
                    quartile[0] = input[input.Count * range / 4 - 1];
                    quartile[1] = input[input.Count * range / 4];

                }
                else
                {
                    quartile[0] = input[input.Count * range / 4];
                    quartile[1] = quartile[0];
                }
            }

            return quartile;
        }

        private double[] Median(List<float> input)
        {
            double[] median = new double[2];
            input.Sort();
            if (input.Count % 2 == 0)
            {
                median[0] = input[(input.Count - 1) / 2 - 1];
                median[1] = input[input.Count / 2];
            }
            else median[0] = input[input.Count / 2];

            return median;
        }
        
        protected double Average(List<float> input)
        {
            return input.Average();
        }

        protected double Mode(List<float> input)
        {
            var mode = input.GroupBy(x => x)
                            .OrderByDescending(x => x.Count())
                            .Select(x => x.Key)
                            .First();
            return mode;
        }
    }
}
