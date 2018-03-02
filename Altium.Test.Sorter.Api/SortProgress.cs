using System;

namespace Altium.Test.Sorter.Api
{
  public class SortProgress
  {
    public int PassesMade { get; set; }
    public long RowsRed { get; set; }
    public int BlocksSorted { get; set; }
    public int BlocksMerged { get; set; }
    public TimeSpan Elapsed { get; set; }
    public Exception Exception { get; set; }
  }
}
