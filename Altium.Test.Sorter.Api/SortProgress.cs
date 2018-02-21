using System;

namespace Altium.Test.Sorter.Api
{
  public class SortProgress
  {
    public int PassesMade { get; set; }
    public int RowsRed { get; set; }
    public int BlocksSorted { get; set; }
    public int BlocksMerged { get; set; }
    public TimeSpan Elapsed { get; set; }
  }
}
