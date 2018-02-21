namespace Altium.Test.Sorter.Api
{
  public interface ILineParser<T>
  {
    T Parse(string data);
    string Unparse(T obj);
  }
}
