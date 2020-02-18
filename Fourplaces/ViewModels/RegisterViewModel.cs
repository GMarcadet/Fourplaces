using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Views;
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
    class RegisterViewModel : ViewModelBase
    {

        private string _email;
        private string _password;
        private string _firstName;
        private string _lastLame;

        public string Email
        {
            set => SetProperty(ref _email, value);
            get => _email;
        }

        public string Password
        {
            set => SetProperty(ref _password, value);
            get => _password;
        }

        public string FirstName
        {
            set => SetProperty(ref _firstName, value);
            get => _firstName;
        }

        public string LastName
        {
            set => SetProperty(ref _lastLame, value);
            get => _lastLame;
        }

        public ICommand SubmitRegisterCommand { protected set; get;  }

        public RegisterViewModel()
        {
            this.SubmitRegisterCommand = new Command( OnSubmitRegister );
        }

        public async void OnSubmitRegister()
        {
            // prepare request
            RegisterRequest request = new RegisterRequest() { Email = Email, Password = Password, FirstName = FirstName, LastName = LastName };
            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Post, APIResources.buildRegisterURI(), request);
            if (httpResponse.IsSuccessStatusCode)
            {
                // LoginResult loginResult = await client.ReadFromResponse<LoginResult>(httpResponse);
                Response<LoginResult> loginResponse = await client.ReadFromResponse<Response<LoginResult>>(httpResponse);

                if (loginResponse.IsSuccess)
                {
                    LoginResult loginResult = loginResponse.Data;

                    // move to next page
                    Dictionary<string, object> navigationParameter = new Dictionary<string, object>();
                    navigationParameter.Add(ApiClient.ACCESS_TOKEN, loginResult.AccessToken);
                    navigationParameter.Add(ApiClient.REFRESH_TOKEN, loginResult.RefreshToken);
                    await NavigationService.PushAsync(new PlacesListPage(), navigationParameter);
                } else
                {
                    Console.WriteLine("Login response failure: ");
                }
            
            } else
            {
                Console.WriteLine("Invalid response status code: " + httpResponse.StatusCode);
            }
        }
        
        public bool OnBackButtonPressed()
        {
            return true;
        }

    }
}
