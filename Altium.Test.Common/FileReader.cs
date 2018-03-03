using Altium.Test.Api;
using System.IO;

namespace Altium.Test
{
  public class FileReader : AFileReader, IFileReader
  {
    private FileStream _fileStream;

    public override IFileReader CreateInstance() => new FileReader();

    public override void BeginRead(
      string path, 
      int bufferSize
    )
    {
      _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true);
      StreamReader = new StreamReader(_fileStream);
    }

    public override void EndRead()
    {
      if (StreamReader != null)
      {
        StreamReader.Dispose();
        StreamReader = null;
      }

      if (_fileStream != null)
      {
        _fileStream.Dispose();
        _fileStream = null;
      }
    }
    
    ~FileReader()
    {
      EndRead();
    }
  }
}
