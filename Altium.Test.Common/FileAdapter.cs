using Altium.Test.Api;
using System.Collections.Generic;
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

    public StreamReader StreamReader { get; private set; }
    public StreamWriter StreamWriter { get; private set; }

    private FileStream _readingFileStream;
    private FileStream _writingStream;

    public void BeginRead(
      string path, 
      int bufferSize
    )
    {
      _readingFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true);
      StreamReader = new StreamReader(_readingFileStream);
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

      if (_readingFileStream != null)
      {
        _readingFileStream.Dispose();
        _readingFileStream = null;
      }
    }

    public void BeginWrite(
      string path, 
      int bufferSize
    )
    {
      EndWrite();

      _writingStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize);
      StreamWriter = new StreamWriter(_writingStream);
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
      stream.CopyTo(_writingStream);
    }

    public void SetLength(long length)
    {
      _writingStream.SetLength(length);
    }

    public void EndWrite()
    {
      if (StreamWriter != null)
      {
        StreamWriter.Flush();
        StreamWriter.Dispose();
        StreamWriter = null;
      }

      if (_writingStream != null)
      {
        _writingStream.Dispose();
        _writingStream = null;
      }
    }

    public void Delete(string path)
    {
      File.Delete(path);
    }

    ~FileAdapter()
    {
      EndRead();
      EndWrite();
    }
  }
}
