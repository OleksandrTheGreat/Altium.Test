using Altium.Test.Sorter.Api;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Test.Sorter
{
  public class LinqSortStrategy : ASortStrategy<GroupedItem<TestLine>>, ISortStrategy<GroupedItem<TestLine>>
  {
    public LinqSortStrategy(IComparer<GroupedItem<TestLine>> comparer) : base(comparer)
    {
    }

    public GroupedItem<TestLine>[] Sort(IEnumerable<GroupedItem<TestLine>> target)
    {
      return 
        target
        .AsParallel()
        .OrderBy(x => x.Item.String)
        .ThenBy(x => x.Item.Number)
        .ToArray();
    }
  }
}
