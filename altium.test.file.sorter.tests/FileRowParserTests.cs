using System;
using System.IO;
using altium.test.file.sorter.api;
using Xunit;

namespace altium.test.file.sorter.tests
{
  public class FileRowParserTests
  {
    private IFileRowParser _parser = new FileRowParser();

    [Theory]
    [InlineData("1")]
    [InlineData("1. ")]
    [InlineData("1.")]
    [InlineData(".")]
    [InlineData(".1")]
    public void FileRowParser_should_skip_wrong_format_rows(string row)
    {
      var actual = _parser.Parse(row);

      Assert.Null(actual);
    }

    [Theory]
    [InlineData("1. String", 1, "String")]
    [InlineData("1 .String", 1, "String")]
    [InlineData("1.String", 1, "String")]
    [InlineData("1.String ", 1, "String")]
    [InlineData(" 1.String ", 1, "String")]
    [InlineData(" 1. String ", 1, "String")]
    [InlineData(" 1. Some String ", 1, "Some String")]
    public void FileRowParser_should_parse_corrent_format_rows(string row, int Number, string String)
    {
      var actual = _parser.Parse(row);

      Assert.Equal(Number, actual.Number);
      Assert.Equal(String, actual.String);
    }
  }
}