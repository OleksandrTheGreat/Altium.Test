namespace Altium.Test.Sorter.Api {

  public interface IFileSorter {
    void Sort (
      string inputPath,
      string outputPath,
      int bufferSize,
      long blockSize
    );
  }

}