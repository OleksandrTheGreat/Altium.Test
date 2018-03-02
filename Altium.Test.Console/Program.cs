using Altium.Test.Providers;
using Altium.Test.Generator;
using Altium.Test.Scenarios;
using Altium.Test.Scenarios.Api;
using Altium.Test.Sorter;
using Altium.Test.Sorter.Api;
using System;
using System.Collections.Generic;

namespace Altium.Test
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
            new FileAdapter(),
            new FileWriter(),
            new LineGenerator(),
            new ConsolePercentageProgress("Generated: ")))
      },
      {
        "2",
        new SortScenario(
          "Sort file",
          new SortFileScenarioConsoleProvider(),
          new FileSorter<TestLine>(
            new TestLineParser(),
            new LinqSortStrategy(
              new TestLineComparer()
            ),
            //new MergeSortStrategy<TestLine>(
            //  new TestLineComparer()
            //),
            new TestLineComparer(),
            new FileAdapter(),
            new FileReader(),
            new FileWriter(),
            new ConsoleSortProgress()))
      }
    };
  }
}
