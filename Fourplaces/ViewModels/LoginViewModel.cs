using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Views;
using Newtonsoft.Json;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
        private string _loginStatus = "";

        private string _email;
        private string _password;

        public ICommand LoginCommand { set; get; }
        public ICommand RegisterCommand { set; get; }

        public string Email
        {
            set => SetProperty( ref _email,  value);
            get => _email;
        }

        public String Password
        {
            set => SetProperty(ref _password, value);
            get => _password;
        }

        public String LoginStatus
        {
            set => SetProperty(ref _loginStatus, value);
            get => _loginStatus;
        }

        public LoginViewModel( )
        {
            this.LoginCommand = new Command(OnSubmitLogin);
            this.RegisterCommand = new Command(OnRegister);
        }


        public async void OnRegister()
        {
            await NavigationService.PushAsync(new RegisterPage());
        }

        public async void OnSubmitLogin()
        {
            // creates a  login request
            LoginRequest request = new LoginRequest();
            request.Email = _email;
            request.Password = _password;

            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse  = await client.Execute(HttpMethod.Post, APIResources.API_URI_ROOT + APIResources.API_URI_AUTH_LOGIN, request);
            if ( httpResponse.IsSuccessStatusCode ) {
                // LoginResult loginResult = await client.ReadFromResponse<LoginResult>(httpResponse);
                Response< LoginResult > loginResponse = await client.ReadFromResponse<Response<LoginResult>>(httpResponse);

                if ( loginResponse.IsSuccess ) {
                    LoginResult loginResult = loginResponse.Data;

                    // put data in session storage
                    SessionStorage storage = SessionStorage.GetStorage();
                    storage.Add(ApiClient.ACCESS_TOKEN, loginResult.AccessToken);
                    storage.Add(ApiClient.REFRESH_TOKEN, loginResult.RefreshToken);
                    
                    // move to next page
                    Dictionary<string, object> navigationParameter = new Dictionary<string, object>();
                    navigationParameter.Add( ApiClient.ACCESS_TOKEN, loginResult.AccessToken);
                    navigationParameter.Add( ApiClient.REFRESH_TOKEN, loginResult.RefreshToken);
                    await NavigationService.PushAsync( new PlacesListPage(), navigationParameter );
                }
            }
        }

    }
}
