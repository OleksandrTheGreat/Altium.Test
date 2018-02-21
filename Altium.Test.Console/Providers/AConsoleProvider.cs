using System;

namespace Altium.Test.Providers
{
  public abstract class AConsoleProvider
  {
    public void Cancel()
    {
      Program.Start();
    }

    public void NotifyError(Exception ex)
    {
      xConsole.WriteError(ex.Message);
      //xConsole.WriteError(ex.StackTrace);
      Console.WriteLine("\nPress Enter...");
      Console.ReadLine();

      Program.Start();
    }

    public void NotifyFinished(TimeSpan time)
    {
      Console.WriteLine("\r\nFinished\r\n");
      xConsole.WriteInfo($"Execution Time: {time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:000}");
      Console.WriteLine("\nPress Enter...");
      Console.ReadLine();

      Program.Start();
    }

    public void NotifyStarted()
    {
      Console.WriteLine($"\nStarted at {DateTime.Now:HH:mm:ss}\r\n");
    }
  }
}
