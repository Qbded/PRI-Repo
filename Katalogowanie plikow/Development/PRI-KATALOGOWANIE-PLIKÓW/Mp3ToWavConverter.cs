using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public static class Mp3ToWavConverter
    {
        public static void Convert(string filename)
        {
            using (Mp3FileReader reader = new Mp3FileReader(filename))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(filename.Replace(".mp3", ".wav"), pcm);
                }
            }
        }
    }
}
