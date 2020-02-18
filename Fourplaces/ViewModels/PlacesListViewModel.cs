using Common.Api.Dtos;
using Fourplaces.DTO;
using Fourplaces.Views;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    class PlacesListViewModel : ViewModelBase
    {
        private List<PlaceItemSummary> _places;

        
        public List<PlaceItemSummary> Places
        {
            get => _places;
            private set => SetProperty( ref _places, value );
        }


        public ICommand RefreshCommand { protected set; get; }
        public ICommand GoToProfilCommand { protected set; get; }
        public ICommand CreatePlaceCommand { protected set; get; }

        private PlaceItemSummary _selectedPlace;
        public PlaceItemSummary SelectedPlace
        {

            set {
                if (SetProperty(ref _selectedPlace, value))
                {
                    DisplayPlaceItemSummaryDetail( value );
                }
            }
        }

        public PlacesListViewModel()
        {
            this.RefreshCommand = new Command( RefreshPlaces );
            this.GoToProfilCommand = new Command(GoToProfil);
            this.CreatePlaceCommand = new Command(OnCreatePlace);
        }


        public async void GoToProfil()
        {
            await NavigationService.PushAsync(new ProfilPage());
        }

        public async void RefreshPlaces()
        {
            Places = await GetAllPlaces();
        }

        public async void OnCreatePlace()
        {
            await NavigationService.PushAsync(new CreatePlacePage());
        }

        
        public override async Task OnResume()
        {
            Places = await GetAllPlaces();
        }

        public async Task<List<PlaceItemSummary>> GetAllPlaces()
        {
            // creates a request to the API to load all places
            string listPlacesURI = APIResources.buildURI(APIResources.API_URI_PLACES);

            // sends request to the api and returns list of place
            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Get, listPlacesURI, null); 

            // Parse response only if status code is OK
            if (httpResponse.IsSuccessStatusCode)
            {   
                // parse response to return a list of place
                Response< List<PlaceItemSummary> > placesResponse = await client.ReadFromResponse<Response< List<PlaceItemSummary> >>(httpResponse);
                if (placesResponse.IsSuccess)
                {
                    Console.WriteLine("Get All placed successed !");
                    return placesResponse.Data;
                }
            }

            Console.WriteLine("An error occured: Please review the code !");
            return new List<PlaceItemSummary>();

        }

        
        public void DisplayPlaceItemSummaryDetail( PlaceItemSummary summary )
        {
            Console.WriteLine("Go to the next interface with " + summary.Title);

            SessionStorage storage = SessionStorage.GetStorage();
            storage.Add("selected_place", summary);


            NavigationService.PushAsync(new PlaceDetailPage() );
        }
    }
}
