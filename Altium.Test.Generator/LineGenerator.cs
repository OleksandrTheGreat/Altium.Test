using Altium.Test.Generator.Api;
using System;

namespace Altium.Test.Generator
{
  public class LineGenerator : ILineGenerator
  {
    private static string _string = GenerateString();
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
        _string = GenerateString();

        return GenerateLine();
      }

      if (_timesHit > requiredTimes)
      {
        _string = GenerateString();
        _timesHit = 0;
      }

      _timesHit++;

      return GenerateLine();
    }

    private static double CalculateTimes(int percentOfAppearance, long totalSize)
    {
      var size = _string.Length;
      double posibleTimes = totalSize / size;
     
      return posibleTimes * percentOfAppearance / 100;
    }

    private static int GenerateNumber()
    {
      var random = new Random();

      return random.Next(1, 100000);
    }

    private static string GenerateString()
    {
      return Guid.NewGuid().ToString();
    }

    private static string GenerateLine()
    {
      return $"{GenerateNumber()}. {_string}{Environment.NewLine}";
    }
  }
}
