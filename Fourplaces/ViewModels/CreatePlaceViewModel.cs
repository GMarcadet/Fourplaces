using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Resources;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD.Api.Dtos;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Fourplaces.ViewModels
{
    class CreatePlaceViewModel : ViewModelBase
    {

        private string _title;
        private string _description;
        private string _longitude;
        private string _latitude;

        private ImageItem _imageItem;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public ICommand SubmitPlaceCommand { protected set; get; }
        public ICommand FillCoordinatesCommand { protected set; get; }
        public ICommand PickPhotoCommand { protected set; get; }

      

        public CreatePlaceViewModel()
        {
            this.SubmitPlaceCommand = new Command(OnSubmitPlace);
            this.FillCoordinatesCommand = new Command(OnFillCoordinates);
            this.PickPhotoCommand = new Command(OnPickPhoto);
        }

        public async void OnPickPhoto()
        {
            bool permissionGranted = await AskPermissions();
            if (permissionGranted)
            {
                // ask at user to choose between pick an image from gallery and take a picture
                PictureSelectionMode selectionMode = await AskToPickSelectionMode();
                MediaFile file = await ImageManagerService.PickImage(selectionMode);
                if ( file != null )
                {
                    ApiClient client = new ApiClient();
                    _imageItem = await client.PublishMediaFile(file);
                    Console.WriteLine("Received image id: " + _imageItem.Id);
                } else
                {
                    Console.WriteLine("Returned file is null");
                }
            } else {
                await DisplayAlert("Missing Permissions", "Some permissions are missing");
            }

        }

        private async Task<bool> AskPermissions()
        {
            return await PermissionManager.AskPermissions(new Permission[] { Permission.Camera, Permission.Storage });
        }

        private async Task DisplayAlert( string title, string message )
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            await dialog.DisplayAlertAsync( title, message, "Close" );
        }

        private async Task<PictureSelectionMode> AskToPickSelectionMode()
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            bool selection =  await dialog.DisplayAlertAsync("Source", "Which source do you want for the picture ?", "Camera", "Gallery");
            // if true, bind to camera, else Gallery
            if ( selection )
            {
                return PictureSelectionMode.PICK_FROM_CAMERA;
            } else
            {
                return PictureSelectionMode.PICK_FROM_GALLERY;
            }
        }

        public async void OnFillCoordinates()
        {
            Console.WriteLine("Fill coordinates");
            await FillCoordinates();
        }

        public async void OnSubmitPlace()
        {
            Console.WriteLine("OnSubmitPlace");

            // Cannot publish a place without all elements
            if ( Title == "" || Description == "" || _imageItem == null || _imageItem.Id == 0 || Double.Parse( Longitude ) == 0 || Double.Parse( Latitude ) == 0 )
            {
                await DisplayAlert("Missing fields", "Some fields are not filled !");
                return;
            }
           

            // cannot publish a place without image
            CreatePlaceRequest request = new CreatePlaceRequest() {
                Title = Title,
                Description = Description,
                ImageId = _imageItem.Id,
                Latitude = Double.Parse(Latitude),
                Longitude = Double.Parse(Longitude),
            };

            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Post, APIResources.buildPlacePublicationURI(), request, token:accessToken);

            // Parse response only if status code is OK
            if (httpResponse.IsSuccessStatusCode)
            {
                // parse response to return a list of place
                Response placesResponse = await client.ReadFromResponse<Response>(httpResponse);
                if (placesResponse.IsSuccess)
                {
                    await DisplayAlert("Place uploaded !", "The place has been created successfully");
                    await NavigationService.PopAsync();
                    return;
                } else
                {
                    Console.WriteLine("Place response failure");   
                }
            } else
            {
                Console.WriteLine("Uploading failure: Invalid response status code: " + httpResponse.StatusCode);
            }
            await DisplayAlert("Uploading Failure", "The place you have created cannot be updated :(");
        }

        
        public override async Task OnResume()
        {
            Console.WriteLine("CreatePlaceViewModel: OnResume");

            // request missing permissions
            await CrossMedia.Current.Initialize();

            await FillCoordinates();
        }

        private async Task FillCoordinates()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Lowest);
            var location = await Geolocation.GetLocationAsync( request );
            if (location != null)
            {
                Console.WriteLine("Localisation: " + location.Latitude + " " + location.Longitude);
                Longitude = location.Longitude.ToString();
                Latitude = location.Latitude.ToString();
            } else
            {
                Console.WriteLine("Warning: Localisation missing");
                Longitude = "0";
                Latitude = "0";
            }
        }
    }
}
