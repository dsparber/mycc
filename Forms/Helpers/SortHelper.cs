using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Types;
using MyCC.Forms.view.components.cells;
using Xamarin.Forms;

namespace MyCC.Forms.helpers
{
    public static class SortHelper
    {
        private static IEnumerable<T> SortCells<T>(IEnumerable<T> cells, SortOrder order, SortDirection direction) where T : SortableViewCell
        {
            Func<T, object> sortLambda;

            switch (order)
            {
                case SortOrder.ByValue: sortLambda = c => c.Value; break;
                case SortOrder.ByUnits: sortLambda = c => c.Units; break;
                case SortOrder.Alphabetical: sortLambda = c => c.Name; break;
                case SortOrder.None: sortLambda = c => null; break;
                default: sortLambda = c => null; break;
            }

            switch (direction)
            {
                case SortDirection.Descending: cells = cells.OrderByDescending(sortLambda); break;
                case SortDirection.Ascending: cells = cells.OrderBy(sortLambda); break;
                default: cells = cells.OrderBy(sortLambda); break;
            }

            return cells.ToList();
        }

        public static void ApplySortOrder<T>(IEnumerable<T> cells, TableSection section, SortOrder order, SortDirection direction) where T : SortableViewCell
        {
            cells = SortCells(cells, order, direction);

            section.Clear();
            foreach (var c in cells)
            {
                section.Add(c);
            }
        }
    }
}

