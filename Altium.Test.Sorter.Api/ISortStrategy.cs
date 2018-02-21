using System.Collections.Generic;

namespace Altium.Test.Sorter.Api
{
  public interface ISortStrategy<T>
  {
    bool IsSorted(T[] target);
    T[] Sort(IEnumerable<T> target);
    T[] Merge(T[] left, T[] right);
  }
}