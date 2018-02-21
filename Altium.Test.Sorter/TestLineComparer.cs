using System.Collections.Generic;

namespace Altium.Test.Sorter.Api
{
  public class TestLineComparer : IComparer<GroupedItem<TestLine>>
  {
    public int Compare(GroupedItem<TestLine> x, GroupedItem<TestLine> y)
    {
      var result = string.Compare(x.Item.String, y.Item.String);

      if (result < 0)
        return -1;

      if (result == 0)
      {
        if (x.Item.Number < y.Item.Number)
          return -1;

        if (x.Item.Number == y.Item.Number)
          return 0;

        return 1;
      }

      return 1;
    }
  }
}
