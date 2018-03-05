using Altium.Test.Generator.Api;
using System;

namespace Altium.Test.Generator
{
  public class LineGenerator : ILineGenerator
  {
    private static int MAX_NUMBER = 100_000;
    private static Random _random = new Random();
    private static string _line = GenerateLine();

    private int _timesHit = 0;

    public string Generate(
      int percentOfAppearance,
      long totalSize
    )
    {
      double requiredTimes = CalculateTimes(percentOfAppearance, totalSize);

      if (requiredTimes < 1)
      {
        _line = GenerateLine();

        return _line;
      }

      if (_timesHit > requiredTimes)
      {
        _line = GenerateLine();
        _timesHit = 0;
      }

      _timesHit++;

      return _line;
    }

    private static double CalculateTimes(int percentOfAppearance, long totalSize)
    {
      var size = _line.Length;
      double posibleTimes = totalSize / size;
     
      return posibleTimes * percentOfAppearance / 100;
    }

    private static string GenerateLine()
    {
      return $"{_random.Next(1, MAX_NUMBER)}. {Guid.NewGuid().ToString()}{Environment.NewLine}";
    }
  }
}
