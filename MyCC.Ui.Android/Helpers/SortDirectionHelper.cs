using MyCC.Core.Types;

namespace MyCC.Ui.Android.Helpers
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
    }
}