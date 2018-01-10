using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
  public class VideoFile
  {
    private string language;
    private string filePath;
    private HashSet<string> tags;

    private double fps;
    private TimeSpan time;

    // Getters
    public double getFps()
    {
      return fps;
    }

    public string getLanguage()
    {
      return language;
    }

    public string getFilePath()
    {
      return filePath;
    }

    public HashSet<string> getTags()
    {
      return tags;
    }

    // Constructors
    public VideoFile(string filePath, double fps, TimeSpan time, string language = "eng")
    {
      this.filePath = filePath;
      this.fps = fps;
      this.time = time;
      this.language = language;
      tags = new HashSet<string>();
    }


    public void setLanguage(string language)
    {
      this.language = language;
    }


    public void addTags(HashSet<string> newTags)
    {
      tags.UnionWith(newTags);
    }


    public long getFramesInTimeSpan()
    {
      return (long)(fps * time.TotalSeconds);
    }
    public long getFramesInTimeSpan(TimeSpan timeSpan)
    {
      return (long)fps * timeSpan.Seconds;
    }
  }
}
