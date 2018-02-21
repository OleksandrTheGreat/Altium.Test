namespace Altium.Test.Generator.Api
{
  public interface IFileGenerator
  {
    void Generate(
      string path,
      long size,
      int bufferSize,
      int percentOfAppearance
    );
  }
}