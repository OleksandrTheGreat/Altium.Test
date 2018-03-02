using Altium.Test.Api;
using System.Collections.Generic;
using System.IO;

namespace Altium.Test
{
  public class FileWriter : IFileWriter
  {
    public IFileWriter CreateInstance()
    {
      return new FileWriter();
    }

    private FileStream _fileStream;
    public StreamWriter StreamWriter { get; private set; }
    
    public void BeginWrite(
      string path, 
      int bufferSize
    )
    {
      EndWrite();

      _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize);
      StreamWriter = new StreamWriter(_fileStream);
    }

    public void Write(IEnumerable<string> lines)
    {
      foreach (var line in lines)
        StreamWriter.WriteLine(line);
    }

    public void WriteLine(string text)
    {
      StreamWriter.WriteLine(text);
    }

    public void CopyFrom(Stream stream)
    {
      stream.CopyTo(_fileStream);
    }

    public void SetLength(long length)
    {
      _fileStream.SetLength(length);
    }

    public void EndWrite()
    {
      if (StreamWriter != null)
      {
        StreamWriter.Flush();
        StreamWriter.Dispose();
        StreamWriter = null;
      }

      if (_fileStream != null)
      {
        _fileStream.Dispose();
        _fileStream = null;
      }
    }
    
    ~FileWriter()
    {
      EndWrite();
    }
  }
}
