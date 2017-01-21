using System.Collections.Generic;
using MyCC.Forms.Resources;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.addsource
{

    public abstract class AddSourceSubview
    {
        public abstract List<TableSection> InputSections { get; }
        public abstract bool Enabled { set; }
        public abstract string Description { get; }
        public string DefaultName => I18N.Unnamed;
        public abstract void Unfocus();
    }
}
