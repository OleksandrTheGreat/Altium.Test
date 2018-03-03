using Altium.Test.Sorter.Api;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Test.Sorter
{
  public sealed class MergeSortStrategy<T> : ASortStrategy<T>, ISortStrategy<T>
  {
    public MergeSortStrategy(IComparer<T> comparer) : base(comparer)
    {
    }

    public override T[] Sort(IEnumerable<T> target)
    {
      return Sort(target.ToArray());
    }
    
    private T[] Sort(T[] target)
    {
      var count = target.Count();

      if (count <= 1)
      {
        return target;
      }

      var middle = count / 2;

      var left = Sort(target.Take(middle).ToArray());
      var right = Sort(target.Skip(middle).ToArray());

      return Merge(left, right);
    }
  }
}
