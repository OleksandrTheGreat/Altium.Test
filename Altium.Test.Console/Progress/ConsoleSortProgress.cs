using Altium.Test.Sorter.Api;
using System;

namespace Altium.Test
{
  public class ConsoleSortProgress : IProgress<SortProgress>
  {
    private int _passesMade  = 0;

    public void Report(SortProgress progress)
    {
      if (progress.Exception != null)
      {
        xConsole.WriteError(progress.Exception.Message);
        return;
      }

      if (_passesMade != progress.PassesMade)
      {
        Console.WriteLine();
        _passesMade = progress.PassesMade;
      }

      Console.SetCursorPosition(0, Console.CursorTop);
      Console.Write(
        $"Pass: {progress.PassesMade:#,###,##0}; " +
        $"Rows red: {progress.RowsRed:#,###,##0}; " +
        $"Blocks sorted: {progress.BlocksSorted:#,###,##0}; " +
        $"Blocks merged: {progress.BlocksMerged:#,###,##0}; " +
        $"Elapsed: {progress.Elapsed.Hours:00}:{progress.Elapsed.Minutes:00}:{progress.Elapsed.Seconds:00}");
    }
  }
}
