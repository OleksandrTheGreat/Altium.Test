using System;

namespace altium.test.file.console
{
  internal class ConsolePercentageProgress : IProgress<int?>
  {
    private int? _lastValue;
    private string _title;

    public ConsolePercentageProgress(string title = null)
    {
      _title = title;
    }

    public void Report(int? value)
    {
      if (_lastValue == value)
        return;

      _lastValue = value;

      Console.SetCursorPosition(0, Console.CursorTop);
      Console.Write($"{_title}{value:##0}%");

      if (value == 100)
        Console.WriteLine();
    }
  }
}