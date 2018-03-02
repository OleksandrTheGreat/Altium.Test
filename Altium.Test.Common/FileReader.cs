using Altium.Test.Api;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.Test
{
  public class FileReader : IFileReader
  {
    public IFileReader CreateInstance()
    {
      return new FileReader();
    }

    private FileStream _fileStream;
    public StreamReader StreamReader { get; private set; }

    public void BeginRead(
      string path, 
      int bufferSize
    )
    {
      _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true);
      StreamReader = new StreamReader(_fileStream);
    }

    public IEnumerable<string> ReadLines(int count)
    {
      if (count <= 0)
        yield break;

      var read = 0;

      while (!StreamReader.EndOfStream && read < count)
      {
        yield return StreamReader.ReadLine();
        read++;
      }
    }

    public IEnumerable<IEnumerable<string>> ReadBlock(int blockSize)
    {
      if (blockSize <= 0)
        yield break;

      while (!StreamReader.EndOfStream)
        yield return ReadLines(blockSize).ToArray();
    }

    public void EndRead()
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
