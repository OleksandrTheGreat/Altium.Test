using Altium.Test.Api;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.Test
{
  public abstract class AFileReader: IFileReader
  {
    public StreamReader StreamReader { get; protected set; }

    public IEnumerable<string> ReadLines(int count)
    {
      for (var i = 0; i < count && !StreamReader.EndOfStream; i++)
        yield return StreamReader.ReadLine();
    }

    public IEnumerable<IEnumerable<string>> ReadBlock(int blockSize)
    {
      if (blockSize <= 0)
        yield break;

      while (!StreamReader.EndOfStream)
        yield return ReadLines(blockSize).ToArray();
    }

    public abstract IFileReader CreateInstance();
    public abstract void BeginRead(string path, int bufferSize);
    public abstract void EndRead();
  }
}
