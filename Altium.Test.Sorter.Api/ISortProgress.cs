using System;

namespace Altium.Test.Sorter.Api
{
  public interface ISortProgress
  {
    void NotifyProgress(
      int pass,
      int lines,
      TimeSpan elapsed
    );
  }
}
