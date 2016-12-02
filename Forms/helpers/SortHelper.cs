﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Settings;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.helpers
{
    public static class SortHelper
    {
        public static IEnumerable<T> SortCells<T>(IEnumerable<T> cells, SortOrder? order = null, SortDirection? direction = null) where T : SortableViewCell
        {
            Func<T, object> sortLambda;

            var usedOrder = order ?? ApplicationSettings.SortOrder;
            var usedDirection = direction ?? ApplicationSettings.SortDirection;

            switch (usedOrder)
            {
                case SortOrder.BY_VALUE: sortLambda = c => c.Value; break;
                case SortOrder.BY_UNITS: sortLambda = c => c.Units; break;
                case SortOrder.ALPHABETICAL: sortLambda = c => c.Name; break;
                default: sortLambda = c => null; break;
            }

            switch (usedDirection)
            {
                case SortDirection.DESCENDING: cells = cells.OrderByDescending(sortLambda); break;
                default: cells = cells.OrderBy(sortLambda); break;
            }

            return cells.ToList();
        }

        public static void ApplySortOrder<T>(IEnumerable<T> cells, TableSection section, SortOrder? order = null, SortDirection? direction = null) where T : SortableViewCell
        {
            cells = SortCells(cells, order, direction);

            section.Clear();
            foreach (var c in cells)
            {
                section.Add(c);
            }
        }

        public static void ApplySortOrder<T>(IEnumerable<Tuple<TableSection, List<T>>> elements, TableView tableView, SortOrder? order = null, SortDirection? direction = null) where T : SortableViewCell
        {
            tableView.Root.Clear();

            elements = elements.OrderBy(e => e.Item1.Title);
            foreach (var e in elements)
            {
                ApplySortOrder(e.Item2, e.Item1, order, direction);
                if (e.Item1.Count > 0)
                {
                    tableView.Root.Add(e.Item1);
                }
            }
        }
    }
}
