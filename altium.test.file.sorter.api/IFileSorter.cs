namespace altium.test.file.sorter.api {
  public interface IFileSorter {
    ///<summary>
    /// Sorts file rows in format "Number. String"; first by String then by Number.
    ///</summary>
    ///<param name="targetFilePath">Input file path</param>
    ///<param name="sortedFilePath">Sorted file path</param>
    void Sort (
      string targetFilePath,
      string sortedFilePath,
      int bufferSize
    );
  }
}