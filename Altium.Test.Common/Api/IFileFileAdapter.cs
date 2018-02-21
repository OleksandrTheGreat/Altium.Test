using System.Collections.Generic;
using System.IO;

namespace Altium.Test.Api
{
  public interface IFileAdapter
  {
    IFileAdapter CreateInstance();

    FileInfo GetFileInfo(string path);
    DriveInfo GetDriveInfo(string disk);

    StreamReader StreamReader { get; }
    StreamWriter StreamWriter { get; }

    void BeginRead(string path, int bufferSize);
    IEnumerable<string> ReadLines(int count);
    IEnumerable<IEnumerable<string>> ReadBlock(int blockSize);
    void EndRead();

    void BeginWrite(string path,int bufferSize);
    void Write(IEnumerable<string> lines);
    void WriteLine(string line);
    void CopyFrom(Stream stream);
    void SetLength(long length);    
    void EndWrite();

    void Delete(string path);
  }
}
