using System;
using System.IO;
using System.Linq;

namespace Altium.Test
{
  internal static class xConsole
  {
    public static void WriteColored(string message, ConsoleColor color)
    {
      Console.ForegroundColor = color;
      Console.Write(message);
      Console.ForegroundColor = ConsoleColor.White;
    }

    public static void WriteHeader(string message)
    {
      WriteColorLine(message, ConsoleColor.Green);
    }

    public static void WriteQuestion(string message)
    {
      WriteColored(message, ConsoleColor.Yellow);
    }

    public static void WriteInfo(string message)
    {
      WriteColorLine(message, ConsoleColor.Yellow);
    }

    public static void WriteError(string message)
    {
      WriteColorLine(message, ConsoleColor.Red);
    }

    public static void WriteColorLine(string message, ConsoleColor color)
    {
      Console.ForegroundColor = color;
      Console.WriteLine(message);
      Console.ForegroundColor = ConsoleColor.White;
    }

    public static int ReadInt(int? @default = null)
    {
      while (true)
      {
        try
        {
          return int.Parse(Console.ReadLine());
        }
        catch
        {
          if (@default.HasValue)
            return @default.Value;

          WriteError("Wrong number");
        }
      }
    }

    public static long ReadLong()
    {
      while (true)
      {
        try
        {
          return long.Parse(Console.ReadLine());
        }
        catch
        {
          WriteError("Wrong number");
        }
      }
    }

    public static string[] ReadAnyStrings(char separator, string @default = null)
    {
      while (true)
      {
        var value = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value) && @default !=null)
          value = @default;

        var strings = value
            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToArray();

        if (strings.Length > 0)
          return strings;
      }
    }

    public static string ReadString(string @default = null)
    {
      while (true)
      {
        var value = Console.ReadLine().Trim();

        if (string.IsNullOrWhiteSpace(value) && @default != null)
          value = @default;

        if (!string.IsNullOrWhiteSpace(value))
          return value;
      }
    }

    public static bool Ask(string question)
    {
      Console.WriteLine();
      WriteQuestion(question);

      while (true)
      {
        var answer = Console.ReadLine().ToLower();

        if (answer == "y")
          return true;

        if (answer == "n")
          return false;
      }
    }
  }
}