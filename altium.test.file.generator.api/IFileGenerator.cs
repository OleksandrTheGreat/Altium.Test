namespace altium.test.file.generator.api
{
  public interface IFileGenerator
  {
    ///<summary>
    /// Generates file with rows in format "Number. String".
    /// Where "Number" is a random value from 0 to <paramref name="maxNumber"/> and "String"
    /// is a random value from <paramref name="strings"/>
    ///</summary>
    ///<param name="path">File path</param>
    ///<param name="size">File size in bytes</param>
    ///<param name="maxNumber">Max template "Number" value</param>
    ///<param name="strings">List of template "String" values </param>
    ///<param name="bufferSize">Write buffer size</param>
    void Generate(
      string path,
      long size,
      int maxNumber,
      string[] values,
      int bufferSize
    );
  }
}