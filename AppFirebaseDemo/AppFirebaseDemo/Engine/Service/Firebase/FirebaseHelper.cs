using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;

namespace AppFirebaseDemo.Engine.Service.Firebase
{
	public static class FirebaseHelper
	{
		private static string firebaseUrl;
		public static string FirebaseUrl { get => firebaseUrl; set => firebaseUrl = value; }

		private static string firebaseApiKey;
		public static string FirebaseApiKey { get => firebaseApiKey; set => firebaseApiKey = value; }

		private static string firebaseSignInEmail;
		public static string FirebaseSignInEmail { get => firebaseSignInEmail; set => firebaseSignInEmail = value; }

		private static string firebaseSignInPassword;
		public static string FirebaseSignInPassword { get => firebaseSignInPassword; set => firebaseSignInPassword = value; }

		private static DateTime CurrentDateTime = DateTime.UtcNow;
		public static FirebaseAuthLink MyFirebaseAuth = null;

		#region Conector do firebase

		private static FirebaseClient _Client;

		public static FirebaseClient Client
		{
			get
			{
				if (_Client == null)
				{
					// CatalogarErroHelper.Mensagem("FireBaseHelper.Client.before");
					if ((string.IsNullOrEmpty(FirebaseApiKey)) || (string.IsNullOrEmpty(FirebaseSignInEmail)) || (string.IsNullOrEmpty(FirebaseSignInPassword)))
					{
						_Client = new FirebaseClient(
							baseUrl: FirebaseUrl,
							options: new FirebaseOptions());
					}
					else
					{
						_Client = new FirebaseClient(
							baseUrl: FirebaseUrl,
							options: new FirebaseOptions()
							{
								AuthTokenAsyncFactory = () => GetAuth()
							});
					}
					// CatalogarErroHelper.Mensagem("FireBaseHelper.Client.after");
				}
				return _Client;
			}
		}

		#endregion

		public static async Task<string> GetAuth(string SignInEmail = null)
		{
			try
			{
				// Se foi informado o e-mail, significa que eh outro login conectado
				if (string.IsNullOrEmpty(SignInEmail) == false)
				{
					// Substituo o e-mail vinculado ao Firebase
					firebaseSignInEmail = SignInEmail;
					// Limpo os dados da sessao atual, para criar uma nova sessao de acesso ao Firebase
					MyFirebaseAuth = null;
				}

				if (MyFirebaseAuth == null)
				{
					MyFirebaseAuth = new FirebaseAuthLink(null, new FirebaseAuth() { ExpiresIn = 0, FirebaseToken = string.Empty });
					MyFirebaseAuth.FirebaseAuthRefreshed += Auth_FirebaseAuthRefreshed;
				}

				//  CatalogarErroHelper.Mensagem("FireBaseHelper.GetAuth().before");
				if (DateTime.UtcNow.Subtract(CurrentDateTime).TotalSeconds > MyFirebaseAuth.ExpiresIn)
				{
					if (string.IsNullOrEmpty(MyFirebaseAuth.FirebaseToken))
					{
						if (await GenerateToken().ConfigureAwait(true))
						{
							CurrentDateTime = DateTime.UtcNow;
						}
					}
					else
					// Faz uma tentativa de refresh..
					if (await RefreshToken().ConfigureAwait(true) == false)
					{
						if (await GenerateToken().ConfigureAwait(true))
						{
							CurrentDateTime = DateTime.UtcNow;
						}
					}
					else
					{
						CurrentDateTime = DateTime.UtcNow;
					}
				}

				return MyFirebaseAuth.FirebaseToken;
			}
			catch (Exception ex)
			{
				App.Log(ex.Message);

				return "";
			}
		}

		private static void Auth_FirebaseAuthRefreshed(object sender, FirebaseAuthEventArgs e)
		{
			Console.WriteLine($"Auth_FirebaseAuthRefreshed >>>>>>>>>> {e.FirebaseAuth.FirebaseToken}");
			MyFirebaseAuth.FirebaseToken = e.FirebaseAuth.FirebaseToken;
			MyFirebaseAuth.ExpiresIn = e.FirebaseAuth.ExpiresIn - 30; //Subtraio 30 segundos para renovar antes de expirar
		}

		private static async Task<bool> CreateAuth()
		{
			try
			{
				FirebaseAuthProvider fap = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));

				FirebaseAuthLink register = await fap.CreateUserWithEmailAndPasswordAsync(firebaseSignInEmail, FirebaseSignInPassword);
				fap.Dispose();
				if (register != null && register.Created > DateTime.MinValue)
				{
					MyFirebaseAuth.ExpiresIn = register.ExpiresIn - 30; //Subtraio 30 segundos para renovar antes de expirar
					MyFirebaseAuth.FirebaseToken = register.FirebaseToken;

					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static async Task<bool> RefreshToken()
		{
			try
			{
				bool retorno = false;
				FirebaseAuthProvider fap = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
				try
				{
					await fap.RefreshAuthAsync(MyFirebaseAuth).ConfigureAwait(true);
					retorno = true;
				}
				finally
				{
					fap.Dispose();
				}
				return retorno;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static async Task<bool> GenerateToken(bool tryCreateAuth = true)
		{
			try
			{
				bool retorno = false;
				FirebaseAuthProvider fap = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
				try
				{
					FirebaseAuthLink generated = await fap.SignInWithEmailAndPasswordAsync(FirebaseSignInEmail, FirebaseSignInPassword).ConfigureAwait(true);
					if (generated != null && generated.Created > DateTime.MinValue && string.IsNullOrEmpty(generated.FirebaseToken) == false)
					{
						MyFirebaseAuth.FirebaseToken = generated.FirebaseToken;
						MyFirebaseAuth.ExpiresIn = generated.ExpiresIn - 30; //Subtraio 30 segundos para renovar antes de expirar
					}
					retorno = true;
				}
				finally
				{
					fap.Dispose();
				}
				return retorno;
			}
			catch (FirebaseAuthException fae)
			{
				// se o e-mail nao foi cadastrado, tentamos registrar o e-mail e solicitar um novo token
				if (fae.Reason == AuthErrorReason.UnknownEmailAddress)
				{
					try
					{
						// somente tento, se foi permitido a partir deste codigo, para evitar recursividade 
						if (tryCreateAuth)
							// Caso, tenha conseguido criar o usuario, tento cria-lo
							if (await CreateAuth().ConfigureAwait(true))
								// se foi criado o usuario, entao, obtenho o token
								return await GenerateToken(false).ConfigureAwait(true);
						return false;
					}
					catch (Exception ex1)
					{
						Console.Write(ex1.Message);
						return false;
					}
				}
				Console.Write(fae.Message);
				return false;
			}
			catch (Exception ex2)
			{
				Console.Write(ex2.Message);
				return false;
			}
		}
	}
}
