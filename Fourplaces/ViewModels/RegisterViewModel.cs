using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Resources;
using Fourplaces.Views;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
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
            // checks all fields for avoid missing one
            if ( Email == "" || Password == "" || FirstName == "" || LastName == "" )
            {
                await DisplayAlert("Missing Fields", "Please complete your profil before to register.");
                return;
            }
           

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
                    SessionStorage storage = SessionStorage.GetStorage();
                    storage.Add(ApiClient.ACCESS_TOKEN, loginResult.AccessToken);
                    storage.Add(ApiClient.REFRESH_TOKEN, loginResult.RefreshToken);
                    await NavigationService.PushAsync(new PlacesListPage());
                } else
                {
                    Console.WriteLine("Login response failure: ");
                }
            
            } else
            {
                Console.WriteLine("Invalid response status code: " + httpResponse.StatusCode);
            }
        }

        private async Task<PictureSelectionMode> AskToPickSelectionMode()
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            bool selection = await dialog.DisplayAlertAsync("Source", "Which source do you want for the picture ?", "Camera", "Gallery");
            // if true, bind to camera, else Gallery
            if (selection)
            {
                return PictureSelectionMode.PICK_FROM_CAMERA;
            }
            else
            {
                return PictureSelectionMode.PICK_FROM_GALLERY;
            }
        }

        public bool OnBackButtonPressed()
        {
            return true;
        }

        private async Task DisplayAlert(string title, string message)
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            await dialog.DisplayAlertAsync(title, message, "Close");
        }


    }
}
