using AppFirebaseDemo.Engine.Model;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace AppFirebaseDemo.Engine.ViewModel
{
	public class BaseViewModel : BaseModel
	{
		public BaseViewModel()
		{
		}

		public static bool PossuiAcessoAInternet()
		{
			try
			{
				if (Connectivity.NetworkAccess == NetworkAccess.Internet)
				{
					return true;
				}

				if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
				{
					return true;
				}

				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
