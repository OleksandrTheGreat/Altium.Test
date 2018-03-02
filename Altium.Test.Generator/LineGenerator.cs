using Altium.Test.Generator.Api;
using System;

namespace Altium.Test.Generator
{
  public class LineGenerator : ILineGenerator
  {
    private static string _line = GenerateLine();

    private int _timesHit = 0;

    public string Generate(
      int percentOfAppearance,
      long totalSize
    )
    {
      var lineSize = _line.Length;
      var posibleTimes = totalSize / lineSize;
      var requiredTimes = posibleTimes * percentOfAppearance / 100;

      if (percentOfAppearance < 2)
        return GenerateLineUnique();

      if (_timesHit > requiredTimes)
      {
        _line = GenerateLine();
        _timesHit = 0;
      }

      _timesHit++;

      return _line;
    }

    private static string GenerateLine()
    {
      var String = Guid.NewGuid();
      return $"{String.GetHashCode()}. {String}{Environment.NewLine}";
    }

    //TODO: duplicates on big volumes 
    private string GenerateLineUnique()
    {
      var String = Guid.NewGuid();
      return $"{String.GetHashCode()}. {String}{Environment.NewLine}";
    }
  }
}
