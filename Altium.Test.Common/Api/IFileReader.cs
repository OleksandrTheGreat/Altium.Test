using System.Collections.Generic;
using System.IO;

namespace Altium.Test.Api
{
  public interface IFileReader
  {
    IFileReader CreateInstance();

    StreamReader StreamReader { get; }

    void BeginRead(string path, int bufferSize);
    IEnumerable<string> ReadLines(int count);
    IEnumerable<IEnumerable<string>> ReadBlock(int blockSize);
    void EndRead();
  }
}
