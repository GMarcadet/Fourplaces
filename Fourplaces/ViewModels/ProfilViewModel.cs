using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Resources;
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
    class ProfilViewModel : ViewModelBase
    {
        private string _email;
        private string _firstName;
        private string _lastName;
        private int _imageId;
        private UserItem _user;

        private string _oldPassword;
        private string _newPassword;


        public string OldPassword
        {
            get => _oldPassword;
            set => SetProperty(ref _oldPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public UserItem User 
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public int ImageId
        {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        public ICommand SubmitProfilUpdate { protected set; get; }
        public ICommand SubmitChangePassword { protected set; get; }

        public ProfilViewModel()
        {
            this.SubmitProfilUpdate = new Command(OnSubmitProfil);
            this.SubmitChangePassword = new Command(OnChangePassword);
        }

        public async void OnSubmitProfil()
        {

            // cannnot update if one is missing
            if (FirstName == "" || LastName == "" ||ImageId == 0)
            {

                await InteractionService.DisplayMissingFields();
                return;
            }
                // build password update request
                UpdateProfileRequest request = new UpdateProfileRequest();
            request.FirstName = FirstName;
            request.LastName = LastName;
            request.ImageId = ImageId;

            

            // get access token
            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(new HttpMethod("PATCH"), APIResources.buildUpdateProfilURI(), request, token: accessToken);
            if (httpResponse.IsSuccessStatusCode)
            {
                // LoginResult loginResult = await client.ReadFromResponse<LoginResult>(httpResponse);
                Response<UserItem> userResponse = await client.ReadFromResponse<Response<UserItem>>(httpResponse);

                if (userResponse.IsSuccess)
                {
                    await InteractionService.DisplayAlert("Profil Updated", "Your profil has been updated.");
                }
                else
                {
                    await InteractionService.DisplayAlert("Password Not Updated", "The update has been rejected by the server.");
                }

            }
        }

        public async void OnChangePassword()
        {
            if ( OldPassword == "" || NewPassword == "" )
            {
                await InteractionService.DisplayMissingFields();
                return;
            }

            // build password update request
            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.OldPassword = OldPassword;
            request.NewPassword = NewPassword;

            // get access token
            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(new HttpMethod( "PATCH" ), APIResources.buildUpdatePasswordURI(), request, token: accessToken);
            if (httpResponse.IsSuccessStatusCode)
            {
                // LoginResult loginResult = await client.ReadFromResponse<LoginResult>(httpResponse);
                Response userResponse = await client.ReadFromResponse<Response>(httpResponse);

                if (userResponse.IsSuccess)
                {
                    await InteractionService.DisplayAlert("Password Updated", "Your password has been updated.");
                }
                else
                {
                    await InteractionService.DisplayAlert("Password Not Updated", "Your password has been rejected by server.");
                }
            } else
            {
                await InteractionService.DisplayAlert("Password Not Updated", "Your password is invalid.");
            }
        }

        public override async Task OnResume()
        {
            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            Console.WriteLine("OnResume(): Search the connected user: " + accessToken);
            User = await GetConnectedUserItem(accessToken);
            
            Console.WriteLine("User returned: " + _user.FirstName);
            FirstName = _user.FirstName;
            LastName = _user.LastName;
            Email = _user.Email;
            ImageId = (int) _user.ImageId;
        }

        public async Task<UserItem> GetConnectedUserItem( string accessToken )
        {
            
            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Get, APIResources.buildUserProfilURI(), null, token:accessToken);
            if (httpResponse.IsSuccessStatusCode)
            {
                // LoginResult loginResult = await client.ReadFromResponse<LoginResult>(httpResponse);
                Response<UserItem> userResponse = await client.ReadFromResponse<Response<UserItem>>(httpResponse);

                if (userResponse.IsSuccess)
                {
                    UserItem user = userResponse.Data;
                    Console.WriteLine("User founded: " + user.FirstName);
                    return user;
                }
                else
                {
                    Console.WriteLine("Login response failure: ");
                }

            }
            else
            {
                Console.WriteLine("Invalid response status code: " + httpResponse.StatusCode);
            }
            return null;
        }

       
    }
}
