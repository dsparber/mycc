using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Addsource;
using MyCC.Ui;
using MyCC.Ui.DataItems;

namespace MyCC.Forms.View.Overlays
{
    public partial class AddSourceOverlay
    {
        private AddSourceSubview _specificAddView;

        public AddSourceOverlay(bool local = false)
        {
            InitializeComponent();
            Header.Data = new HeaderItem(I18N.NewSource, I18N.Testing);

            var addViews = new List<AddSourceSubview>
            {
                new AddAddressSubview(Navigation, NameEntryCell.Entry),
                new AddBittrexSubview(),
                new AddPoloniexSubview(),
                new AddLocalAccountSubview(Navigation)
            };

            _specificAddView = addViews[local ? 2 : 0];
            TableViewComponent.Root.Clear();
            foreach (var s in _specificAddView.InputSections)
            {
                TableViewComponent.Root.Add(s);
            }
            TableViewComponent.Root.Add(NameSection);

            SegmentedControl.BackgroundColor = AppConstants.TableBackgroundColor;
            SegmentedControl.Tabs = addViews.Select(v => v.Description).ToList();
            SegmentedControl.SelectedIndex = local ? 2 : 0;
            SegmentedControl.SelectionChanged = index =>
            {
                _specificAddView = addViews[index];
                NameEntryCell.Placeholder = _specificAddView.DefaultName;
                var txt = NameEntryCell.Text?.Trim();
                Header.Info = string.Empty.Equals(txt) || txt == null ? _specificAddView.DefaultName : txt;

                TableViewComponent.Root.Clear();
                foreach (var s in _specificAddView.InputSections)
                {
                    TableViewComponent.Root.Add(s);
                }
                TableViewComponent.Root.Add(NameSection);
            };

            Header.Info = _specificAddView.DefaultName;
            NameEntryCell.Placeholder = _specificAddView.DefaultName;
            NameEntryCell.Entry.TextChanged += (sender, e) => Header.Info = e.NewTextValue.Length != 0 ? e.NewTextValue : _specificAddView.DefaultName;
        }

        private void Cancel(object sender, EventArgs e)
        {
            UnfocusAll();
            Navigation.PopOrPopModal();
        }

        private async void Save(object sender, EventArgs e)
        {
            ViewsEnabled = false;

            var nameText = (NameEntryCell.Text ?? string.Empty).Trim();
            var name = nameText.Equals(string.Empty) ? _specificAddView.DefaultName : nameText;

            var repositoryView = _specificAddView as AddRepositorySubview;
            var accountView = _specificAddView as AddAccountSubview;

            var value = accountView != null ? accountView.GetAccount(name) : repositoryView?.GetRepository(name) as dynamic;

            await UiUtils.Edit.Add(value);

            ViewsEnabled = true;
        }

        private void UnfocusAll()
        {
            NameEntryCell.Entry.Unfocus();
            _specificAddView.Unfocus();
        }

        private bool ViewsEnabled
        {
            set
            {
                if (!value)
                {
                    UnfocusAll();
                }
                Header.IsLoading = !value;
                NameEntryCell.IsEditable = value;
                _specificAddView.Enabled = value;
            }
        }
    }
}
