using System.Collections.Generic;

namespace Altium.Test.Sorter
{
  public abstract class ASortStrategy<T>
  {
    protected readonly IComparer<T> _comparer;

    public ASortStrategy(IComparer<T> comparer)
    {
      _comparer = comparer;
    }

    public bool IsSorted(T[] target)
    {
      var length = target.Length;
      var middle = length / 2 + 1;

      for (var i = 1; i < middle; i++)
        if (
          _comparer.Compare(target[i - 1], target[i]) > 0
          || _comparer.Compare(target[length - i], target[length - i - 1]) < 0
        )
          return false;

      return true;
    }
    
    public T[] Merge(T[] left, T[] right)
    {
      var merge = new T[left.Length + right.Length];

      int
        m = 0,
        l = 0,
        r = 0;

      for (; m < merge.Length; m++)
      {
        if (r >= right.Length)
        {
          merge[m] = left[l];
          l++;
        }
        else if (l < left.Length && _comparer.Compare(left[l], right[r]) < 0)
        {
          merge[m] = left[l];
          l++;
        }
        else
        {
          merge[m] = right[r];
          r++;
        }
      }

      return merge;
    }
  }
}
