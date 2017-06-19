using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRI_KATALOGOWANIE_PLIKÓW.classes;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
  public class ExtractionOptions
  {
    private decimal samplingFrequency;
    private string language;
    public List<TimeRange> timeRanges { get; }

    public ExtractionOptions()
    {
      samplingFrequency = 0.5m;
      language = "eng";
      timeRanges = new List<TimeRange>();
    }

    public ExtractionOptions(decimal samplingFrequency, string language)
    {
      this.samplingFrequency = samplingFrequency;
      this.language = language;
      timeRanges = new List<TimeRange>();
    }

    public void contractTimeRanges()
    {
      timeRanges.Sort();

      var tempList = new List<Tuple<TimeSpan, TimeSpan>>();
      for (int i = 0; i < timeRanges.Count - 1;)
      {
        TimeRange prevTimeRange = timeRanges.ElementAt(i);
        TimeRange nextTimeRange = timeRanges.ElementAt(i + 1);
        TimeRange newTimeRange = new TimeRange(prevTimeRange.start, prevTimeRange.finish);
        bool TimeRangesOverlap = false;

        if (prevTimeRange.start.CompareTo(nextTimeRange.start) <= 0 && nextTimeRange.start.CompareTo(prevTimeRange.finish) <= 0)
        {
          TimeRangesOverlap = true;
          newTimeRange.start = prevTimeRange.start;
          if (prevTimeRange.finish.CompareTo(nextTimeRange.finish) < 0)
          {
            newTimeRange.finish = nextTimeRange.finish;
          }
        }

        if (TimeRangesOverlap)
        {
          timeRanges[i] = newTimeRange;
          timeRanges.RemoveAt(i + 1);
        }
        else
        {
          i += 1;
        }
      }
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

    public void addTimeRange(TimeRange timeRange)
    {
      timeRanges.Add(timeRange);
    }

    public void DEBUG_displayTimeRanges()
    {
      foreach (var tr in timeRanges)
      {
        Console.WriteLine(tr.ToString());
      }
    }
  }
}
