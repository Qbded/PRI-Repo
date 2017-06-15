using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractTest
{
  public class ExtractionOptions
  {
    private decimal samplingFrequency;
    private string language;
    private List<Tuple<TimeSpan, TimeSpan>> timeRanges;

    public ExtractionOptions()
    {
      samplingFrequency = 0.5m;
      language = "eng";
      timeRanges = new List<Tuple<TimeSpan, TimeSpan>>();
    }
    public ExtractionOptions(decimal samplingFrequency, string language)
    {
      this.samplingFrequency = samplingFrequency;
      this.language = language;
      timeRanges = new List<Tuple<TimeSpan, TimeSpan>>();
    }


    public void contractTimeRanges()
    {
      timeRanges.Sort();
    }


    public decimal getSamplingFrequency()
    {
      return samplingFrequency;
    }
    
    public string getLanguage()
    {
      return language;
    }


    public void setSamplingFrequency(decimal frequency)
    {
      this.samplingFrequency = frequency;
    }

    public void setLanguage(string language)
    {
      this.language = language;
    }

    public void addTimeRange(Tuple<TimeSpan, TimeSpan> timeRange)
    {
      timeRanges.Add(timeRange);
    }


    public void DEBUG_displayTimeRanges()
    {
      foreach(var tr in timeRanges)
      {
        Console.WriteLine(tr.ToString());
      }
      Console.WriteLine("Done dTR()");
    }
  }
}
