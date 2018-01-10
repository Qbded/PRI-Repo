using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    internal class WaveOf : Statistics
    {
        List<float> input;
        TimeSpan totalTime; 
        public WaveOf(string filename)
        {
            input = new List<float>();
            this.GetParametersFromWaveOf(filename);
            totalTime = this.GetParametersFromWaveOf(filename).TotalTime;
        }
        public double this[string statistics]
        {
            get
            {
                double ret = 0;
                if (statistics.Equals("Average")) ret = this.Average(input);
                else if (statistics.Equals("Q1-Minimum")) ret = this.Quartile(1, input)[0];
                else if (statistics.Equals("Q1-Maximum")) ret = this.Quartile(1, input)[1];
                else if (statistics.Equals("Q2-Minimum")) ret = this.Quartile(2, input)[0];
                else if (statistics.Equals("Q2-Maximum")) ret = this.Quartile(2, input)[1];
                else if (statistics.Equals("Q3-Minimum")) ret = this.Quartile(3, input)[0];
                else if (statistics.Equals("Q3-Maximum")) ret = this.Quartile(3, input)[1];
                else if (statistics.Equals("QDev-Maximum"))
                    ret = 0.5 * (this.Quartile(3, input)[1] - this.Quartile(1, input)[1]);
                else if (statistics.Equals("QDev-Minimum"))
                    ret = 0.5 * (this.Quartile(3, input)[0] - this.Quartile(1, input)[0]);
                else if (statistics.Equals("Mode")) ret = this.Mode(input);
                else if (statistics.Equals("Total time")) ret = totalTime.TotalMilliseconds;
                return ret;
            }
            set { }
        }
        private WaveChannel32 GetParametersFromWaveOf(string filename)
        {
            WaveChannel32 wave = new WaveChannel32(new WaveFileReader(filename));
            byte[] buffer = new byte[16384];
            int reader = 0;
            while (wave.Position < wave.Length)
            {
                reader = wave.Read(buffer, 0, 16384);
                for (int i = 0; i < reader / 15; i++)
                {
                    input.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }

            return wave;
        }
    }
}
