using System;
using System.IO;
using Altium.Test.Generator.Api;
using Xunit;

namespace Altium.Test.Generator.Tests
{
  public class FileGeneratorTests
  {
    private readonly IFileGenerator _generator = new FileGenerator();

    //[Fact]
    public void Generator_should_generate_file_of_required_size()
    {
      var path = "Altium.Test.generated.txt";
      long expectedSize = 1 * 1024 * 1024;

      _generator.Generate(
        path,
        expectedSize,
        1024,
        1
      );

      var actualSize = new FileInfo(path).Length;

      Assert.Equal(expectedSize, actualSize);
    }
  }
}
