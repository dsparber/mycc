using data.repositories.account;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCryptos.view.addrepositoryviews
{

    public abstract class AbstractAddRepositoryView : ContentView
    {
        protected TableView View;

        public AbstractAddRepositoryView() {
            View = new TableView();
            Content = View;
        }

        public abstract bool Enabled { set; }
        public abstract string DefaultName { get; }
        public abstract AccountRepository GetRepository(string name);
    }
}
