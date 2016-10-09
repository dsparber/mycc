using System;
using System.Collections.Generic;
using System.Linq;
using data.settings;
using enums;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace helpers
{
	public static class SortHelper
	{
		public static IEnumerable<T> SortCells<T>(IEnumerable<T> cells) where T : SortableViewCell
		{
			Func<T, object> sortLambda;

			switch (ApplicationSettings.SortOrder)
			{
				case SortOrder.BY_VALUE: sortLambda = c => c.Value; break;
				case SortOrder.BY_UNITS: sortLambda = c => c.Units; break;
				default: sortLambda = c => c.Name; break;
			}

			switch (ApplicationSettings.SortDirection)
			{
				case SortDirection.DESCENDING: cells = cells.OrderByDescending(sortLambda); break;
				default: cells = cells.OrderBy(sortLambda); break;
			}

			return cells;
		}

		public static void ApplySortOrder<T>(IEnumerable<T> cells, TableSection section) where T : SortableViewCell
		{
			cells = SortCells(cells);

			section.Clear();
			foreach (var c in cells)
			{
				section.Add(c);
			}
		}

		public static void ApplySortOrder<T>(IEnumerable<Tuple<TableSection, List<T>>> elements, TableView tableView) where T : SortableViewCell
		{
			tableView.Root.Clear();

			elements = elements.OrderBy(e => e.Item1.Title);
			foreach (var e in elements)
			{
				ApplySortOrder(e.Item2, e.Item1);
				if (e.Item1.Count > 0)
				{
					tableView.Root.Add(e.Item1);
				}
			}
		}
	}
}

