using Altium.Test.Api;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Altium.Test
{
  public class MemoryMappedFileReader : AFileReader, IFileReader
  {
    private MemoryMappedFile _memoryMappedFile;
    private MemoryMappedViewStream _viewStream;

    public override IFileReader CreateInstance() => new MemoryMappedFileReader();

    public override void BeginRead(
      string path, 
      int bufferSize
    )
    {
      EndRead();

      _memoryMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
      _viewStream = _memoryMappedFile.CreateViewStream();

      StreamReader = new StreamReader(_viewStream);
    }

    public override void EndRead()
    {
      if (StreamReader != null)
      {
        StreamReader.Dispose();
        StreamReader = null;
      }

      if (_viewStream!=null)
      {
        _viewStream.Dispose();
        _viewStream = null;
      }

      if(_memoryMappedFile!=null)
      {
        _memoryMappedFile.Dispose();
        _memoryMappedFile = null;
      }
    }

    ~MemoryMappedFileReader()
    {
      EndRead();
    }
  }
}
