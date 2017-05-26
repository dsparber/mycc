using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Types;

namespace MyCC.Ui.Helpers
{
    public static class SortDirectionHelper
    {
        public static SortDirection GetNewSortDirection(SortOrder sortOrder, SortDirection sortDirection, SortOrder fieldSortOrder)
        {
            return sortOrder == fieldSortOrder && sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
        }

        public static SortDirection? GetSortDirection(SortOrder sortOrder, SortDirection sortDirection, SortOrder fieldSortOrder)
        {
            return sortOrder == fieldSortOrder ? sortDirection as SortDirection? : null;
        }

        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending)
        {
            return ascending ? source.OrderBy(keySelector)
                              : source.OrderByDescending(keySelector);
        }
    }
}