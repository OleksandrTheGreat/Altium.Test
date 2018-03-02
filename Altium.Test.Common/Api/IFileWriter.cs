using System.Collections.Generic;
using System.IO;

namespace Altium.Test.Api
{
  public interface IFileWriter
  {
    IFileWriter CreateInstance();

    StreamWriter StreamWriter { get; }

    void BeginWrite(string path, int bufferSize);
    void Write(IEnumerable<string> lines);
    void WriteLine(string line);
    void CopyFrom(Stream stream);
    void SetLength(long length);
    void EndWrite();
  }
}
