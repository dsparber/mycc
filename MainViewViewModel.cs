using System;
using System.ComponentModel;

namespace MyCryptos
{
	public class MainViewViewModel : INotifyPropertyChanged
	{
		private bool isLoading;

		private AccountsCollection accountsCollection;

		public MainViewViewModel ()
		{
			IsLoading = true;
			accountsCollection = AccountsCollection.Instance;
		}

		public bool IsLoading {
			get {
				return this.isLoading;
			}

			set {
				this.isLoading = value;
				RaisePropertyChanged ("IsLoading");
			}
		}

		public AccountsCollection AccountsCollection {
			get { 
				return this.accountsCollection;
			}
			set {
				this.accountsCollection = value;
				RaisePropertyChanged ("AccountsCollection");
			}
		}



		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged (string propName)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (propName));
			}
		}
	}
}

