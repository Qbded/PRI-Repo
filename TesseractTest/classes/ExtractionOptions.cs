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

    public ExtractionOptions()
    {
      samplingFrequency = 0.5m;
      language = "eng";
    }
    public ExtractionOptions(decimal samplingFrequency, string language)
    {
      this.samplingFrequency = samplingFrequency;
      this.language = language;
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
  }
}
