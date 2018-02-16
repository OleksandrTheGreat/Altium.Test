using altium.test.file.sorter.api;

namespace altium.test.file.sorter
{
  public class FileRowParser : IFileRowParser
  {
    public FileRow Parse(string row)
    {
      var values = row.Split('.');

      if(values.Length < 2)
        return null;

      var Number = ParseNumber(values[0]);
      var String = values[1];

      if(Number == null || String == null)
        return null;

      return new FileRow
      {
        Number = (int)Number,
        String = String
      };
    }

    private static int? ParseNumber(string value)
    {
      try
      {
        return int.Parse(value);
      }
      catch 
      {
        return null;
      }
    }
  }
}