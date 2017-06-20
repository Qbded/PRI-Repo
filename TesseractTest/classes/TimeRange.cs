using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractTest.classes
{

  public class TimeRange:IComparable<TimeRange>
  {
    public TimeSpan start { get; set; }
    public TimeSpan finish { get; set; }

    public TimeRange()
    {
      start = new TimeSpan(0);
      finish = new TimeSpan(0);
    }
    public TimeRange(TimeSpan start, TimeSpan finish)
    {
      if (finish > start)
      {
        this.start = start;
        this.finish = finish;
      }
      else
      {
        this.start = finish;
        this.finish = start;
      }
    }

    
    public TimeSpan getTimeDifference()
    {
      return finish - start;
    }


    public override string ToString()
    {
      return "(" + start.ToString() + "," + finish.ToString() + ")";
    }

    public int CompareTo(TimeRange other)
    {
      return this.start.CompareTo(other.start) * 10 + this.finish.CompareTo(other.finish);
    }
  }
}
