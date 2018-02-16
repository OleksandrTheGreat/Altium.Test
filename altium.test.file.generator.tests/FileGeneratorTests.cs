using System;
using System.IO;
using altium.test.file.generator.api;
using Xunit;

namespace altium.test.file.generator.tests
{
  public class FileGeneratorTests
  {
    private readonly IFileGenerator _generator = new FileGenerator();

    [Fact]
    public void Generator_should_generate_file_of_required_size()
    {
      var path = "altium.test.file.generated.txt";
      long expectedSize = 1 * 1024 * 1024;

      _generator.Generate(
        path,
        expectedSize,
        100000,
        new[] {
          "Apple",
          "Something something something",
          "Apple",
          "Cherry is the best",
          "Banana is yellow"
        },
        1024,
        10
      );

      var actualSize = new FileInfo(path).Length;

      Assert.Equal(expectedSize, actualSize);
    }
  }
}
