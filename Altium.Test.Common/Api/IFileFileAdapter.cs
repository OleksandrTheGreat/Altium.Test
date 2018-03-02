using System.IO;

namespace Altium.Test.Api
{
  public interface IFileAdapter
  {
    FileInfo GetFileInfo(string path);
    DriveInfo GetDriveInfo(string disk);

    void Delete(string path);
    void CleanSortTrash(string inputPath, string outputPath);
  }
}
