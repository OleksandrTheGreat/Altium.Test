using Altium.Test.Api;
using System.IO;
using System.Linq;

namespace Altium.Test
{
  public class FileAdapter : IFileAdapter
  {
    public IFileAdapter CreateInstance()
    {
      return new FileAdapter();
    }

    public FileInfo GetFileInfo(string path)
    {
      return new FileInfo(path);
    }

    public DriveInfo GetDriveInfo(string path)
    {
      return new DriveInfo(Path.GetPathRoot(path));
    }
    
    public void Delete(string path)
    {
      File.Delete(path);
    }

    public void CleanSortTrash(
      string inputPath,
      string outputPath
    )
    {
      var files = Directory
        .GetFiles(Path.GetDirectoryName(inputPath))
        .Where(x => x.StartsWith(inputPath) && x != inputPath && x != outputPath);

      foreach (var file in files)
        File.Delete(file);
    }
  }
}
