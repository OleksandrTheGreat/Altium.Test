using Altium.Test.Api;
using Altium.Test.Sorter.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altium.Test.Common.Tests
{
  public class FileAdapterTests
  {
    private readonly IFileAdapter _fileAdapter = new FileAdapter();

    [Theory]
    [InlineData("line 1\r\nline 2", 20, 1, "")]
    [InlineData("line 1\r\nline 2\r\nline 3", 0, 5, "line 1\r\n")]
    [InlineData("line 1\r\nline 2\r\nline 3", 5, 5, "line 2\r\n")]
    [InlineData("line 1\r\nline 2\r\nline 3", 10, 5, "line 3")]
    [InlineData("line 1\r\nline 2\r\nline 3", 5, 10, "line 2\r\nline 3")]
    [InlineData("line 1\r\nline 2\r\nline 3\r\nline 4\r\nline 5", 5, 10, "line 2\r\nline 3\r\n")]
    public void FileAdapter_should_read_next_line_from_offset(
      string content,
      int offset,
      int length,
      string expected
    )
    {
      var path = "FileAdapter_should_read_next_line_from_start.txt";
      var bufferSize = 65 * 1024 * 1024;

      File.Delete(path);
      File.AppendAllText(path, content);

      var actual = _fileAdapter.ReadLines(path, bufferSize, offset, length);

      Assert.Equal(expected, actual);
    }

    [Fact]
    public void FileAdapter_should_read()
    {
      var path = "FileAdapter_should_read_next_line_from_start.txt";
      var bufferSize = 65 * 1024 * 1024;

      var content = @"-1657404145. 4ab35b52-572f-4ee9-bb8b-aa5dc982c6c4
-612036330. 74da16ae-7c65-4cc0-9f46-34aa423dab49
-426808103. d47eae07-5dd7-4095-b0a7-f362b9389710
-811998373. d20637b3-b469-46fc-a646-892e2726ea75
-1479701862. 0f85b4f5-6f58-4d9b-8479-966db3284588
-1097228659. 3d552983-a8a9-4d71-aa8d-594a0d92e484
1818033781. df1acccd-bb57-407b-a231-57cc4db86a3f
-1084921681. d93d0925-129b-4e48-ad5c-a672bc2f865a
-365318409. 37867280-70b1-407d-96e2-c0b9504e0224
-2025353667. c825e633-6775-4941-b852-b794c35d9492
935125773. 6b33ee98-9d2e-47e7-8682-fbc03d1293db
338237810. 1655bab7-34e7-42e7-80f3-854ea2641e0e
-444711226. 1a0200c6-6520-49bd-99d9-cd82b9820c34
359001541. 850340c3-b18b-4b97-bd12-e87e300e19a5
-715445919. ac80f698-e35c-4397-8281-bbcb27bdf7f1
617233623. 8672dd55-5158-4b16-845c-12a95eecbc40
-1593864635. e8603a18-15fe-40ba-9d44-d7433ee5f24b
1102573579. 0f17c83b-0fcd-4971-b54c-8701487b5606
953907245. af740d36-459d-4ecd-a606-392820365bf1
-2064380383. 1166de8d-d6d1-4c04-b36a-3690ce6ca049
-675533405. 9b6179c8-4b33-430c-bbb6-090de3a9d802
-1770555703. 8ecb705d-f8ff-4c84-b95c-f3f5d2a2cba1
2006349465. 1ee35cca-570a-47d5-ac42-e7bff53f4791
-105068484. 8a703935-15b0-4e55-bec9-c6c6072d5ffb
-489438324. 2a4061a4-7d08-4121-9b12-bb53bbcd09da
144676728. 3efbfddf-fba8-4196-b070-b205bfe14072
2000743392. a4a7cce1-2192-485c-998d-72710a8bc9ea
";

      //File.Delete(path);
      //File.AppendAllText(path, content);

      //var length = 128;

      //var offset = 0;
      //var block1 = _fileAdapter.ReadLines(path, bufferSize, offset, length).ToArray();

      //offset += block1.Sum(x => x.Length + 2) - 2;
      //var block2 = _fileAdapter.ReadLines(path, bufferSize, offset, length).ToArray();

      //offset += block2.Sum(x => x.Length + 2) - 2;
      //var block3 = _fileAdapter.ReadLines(path, bufferSize, offset, length).ToArray();

      //offset += block3.Sum(x => x.Length + 2) - 2;
      //var block4 = _fileAdapter.ReadLines(path, bufferSize, offset, length).ToArray();

      //offset += block4.Sum(x => x.Length + 2) - 2;
      //var block5 = _fileAdapter.ReadLines(path, bufferSize, offset, length).ToArray();
    }

    [Fact]
    public void FileAdapter_should_not_loose_or_add_lines_1G()
    {
      var targetPath = "d:/temp/1G";
      var testPath = $"{targetPath}.test";
      var bufferSize = 65 * 1024 * 1024;

      var target = new FileInfo(targetPath);

      var offset = 0;

      var read = new List<string>();

      using (var fs = new FileStream($"{testPath}", FileMode.Create, FileAccess.Write))
      using (var sw = new StreamWriter(fs))
      {
        while (offset < target.Length)
        {
          var block = _fileAdapter.ReadLines(targetPath, bufferSize, offset, bufferSize);

          var lines = block.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
          var intersect = lines.Intersect(read);
          if (intersect.Any())
          {
          }
          read.AddRange(lines);

          sw.Write(block);

          offset += block.Length + 1;
        }
      }

      var actual = new FileInfo(testPath);

      Assert.Equal(target.Length, actual.Length);

      //for (var i = 0; i < chunks; i++)
      //{
      //  using (var fs = new FileStream($"{testPath}.{i}", FileMode.Create, FileAccess.Write))
      //  using (var sw = new StreamWriter(fs))
      //  {
      //    foreach (var line in _fileAdapter.ReadLines(targetPath, bufferSize, offset, bufferSize))
      //    {
      //      sw.WriteLine(line);
      //      offset += line.Length + 2;
      //    }

      //    offset -= 2;
      //  }
      //}

      //var blocks = new List<string[]>();

      //for(var i=0; i< chunks; i++)
      //{
      //  var lines = File.ReadAllLines($"{testPath}.{i}");

      //  blocks.Add(
      //    new[] {
      //      lines.First(),
      //      lines.Last()
      //    }
      //  );
      //}

      //var intersections = new List<string[]>();
      //string[] prevBlock = null;

      //for (var i = 1; i < chunks; i++)
      //{
      //  var lines0 = File.ReadAllLines($"{testPath}.{i - 1}");
      //  var lines1 = File.ReadAllLines($"{testPath}.{i}");

      //  intersections.Add(lines1.Intersect(lines0).ToArray());
      //}
    }

    [Fact]
    public void Pararllel_read_test()
    {
      var path = "d:/temp/1G.1";
      var bufferSize = 65 * 1024 * 1024;

      var blockSize = 2;

      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, bufferSize))
      using (var sw = new StreamWriter(fs))
      {
        foreach (var pair in ReadPairOfBlocks(path, bufferSize, blockSize))
        {
          foreach (var row in pair[1])
            sw.WriteLine(row);

          foreach (var row in pair[0])
            sw.WriteLine(row);
        }
      }
    }

    private static IEnumerable<string[][]> ReadPairOfBlocks(
      string path,
      int bufferSize,
      int blockSize
    )
    {
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        while (!sr.EndOfStream)
        {
          var block1 = ReadLines(sr, blockSize).ToArray();
          var block2 = ReadLines(sr, blockSize).ToArray();

          yield return new[] { block1, block2 };
        }
      }
    }

    private static IEnumerable<string> ReadLines(
      StreamReader sr,
      int count
    )
    {
      var readCount = 0;

      while(!sr.EndOfStream && readCount < count)
      {
        yield return sr.ReadLine();
        readCount++;
      }
    }
  }
}
