using System;

namespace Altium.Test.Sorter.Api
{
  public enum SortStatus
  {
    StandBy,
    Reading,
    Grouping,
    Sorting,
    Merging,
    Writing,
    Cleaning
  }

  public class SortProgress
  {
    public int PassesMade { get; set; }
    public long RowsRed { get; set; }
    public int RedPercent { get; set; }
    public int BlocksSorted { get; set; }
    public int BlocksMerged { get; set; }
    public SortStatus Status { get; set; }
    public TimeSpan Elapsed { get; set; }
    public Exception Exception { get; set; }
  }
}
