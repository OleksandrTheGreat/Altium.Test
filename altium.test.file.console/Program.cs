using altium.test.file.console.Providers;
using altium.test.file.generator;
using altium.test.file.scenarios;
using altium.test.file.scenarios.api;
using altium.test.file.sorter;
using System;
using System.Collections.Generic;

namespace altium.test.file.console
{
  class Program
  {
    static void Main(string[] args)
    {
      Start();
    }

    public static void Start()
    {
      PrintMenu();

      var choise = Console.ReadLine();

      if (choise == "0")
        return;

      if (Menu.ContainsKey(choise))
        Menu[choise].Run();
      else
        Start();
    }

    private static void PrintMenu()
    {
      Console.Clear();
      xConsole.WriteHeader("Select action");
      Console.WriteLine();

      const string template = "  {0}. {1}";

      foreach (var item in Menu)
      {
        if (item.Value == null)
          continue;

        Console.WriteLine(template, item.Key, item.Value.Description);
      }

      Console.WriteLine(template, "0", "Exit");

      Console.WriteLine();
    }

    static Dictionary<string, IScenario> Menu = new Dictionary<string, IScenario>
    {
      { "0", null },
      {
        "1",
        new GenerateFileScenario(
          "Generate File",
          new GenerateFileScenarioConsoleProvider(),
          new FileGenerator(
            new ConsolePercentageProgress("Generated: ")
          )
        )
      },
      {
        "2",
        new SortScenario(
          "Sort file",
          new SortFileScenarioConsoleProvider(),
          new CompressedFileSorter(
            new FileRowParser(),
            new ConsolePercentageProgress("Parsed: "),
            new ConsolePercentageProgress("Written: ")
          )
        )
      }
    };
  }
}
