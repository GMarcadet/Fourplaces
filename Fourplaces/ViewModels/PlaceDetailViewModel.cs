using Common.Api.Dtos;
using Fourplaces.DTO;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD.Api.Dtos;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Fourplaces.ViewModels
{
    class PlaceDetailViewModel : ViewModelBase
    {
        private PlaceItem _selectedPlace;

        private string _title;
        private string _description;
        private int _imageId;
        private List<CommentItem> _comments;

        private string _userComment;

        private Position _myPosition;
        private ObservableCollection<Pin> _pinCollection = new ObservableCollection<Pin>();


        public Position MyPosition 
        {
            get => _myPosition;
            set => SetProperty(ref _myPosition, value);
        }
        public ObservableCollection<Pin> PinCollection 
        {
            get => _pinCollection;
            set => SetProperty(ref _pinCollection, value); 
        }

        public string UserComment
        {
            set => SetProperty(ref _userComment, value);
            get => _userComment;
        }

        public string Title
        {
            set => SetProperty(ref _title, value);
            get => _title;
        }

        public string Description
        {
            set => SetProperty(ref _description, value);
            get => _description;
        }

        public int ImageId
        {
            set => SetProperty(ref _imageId, value);
            get => _imageId;
        }

        public List<CommentItem> Comments
        {
            set => SetProperty(ref _comments, value);
            get => _comments;
        }


        public ICommand SendCommentCommand { protected set; get; }


        public PlaceDetailViewModel()
        {
            this.SendCommentCommand = new Command(OnSendComment);
        }

        public async void OnSendComment()
        {
            // block empty comment 
            if ( UserComment == "" )
            {
                return;
            }

            // access to access token
            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            CreateCommentRequest request = new CreateCommentRequest() { Text = UserComment };
            // sends request to the api and returns list of place
            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Post, APIResources.buildCreateCommentURI( this._selectedPlace.Id ), request, token:accessToken);

            // Parse response only if status code is OK
            if (httpResponse.IsSuccessStatusCode)
            {
                // clear user comment input
                this.UserComment = "";

            }

        }

   

        public override async void Initialize( Dictionary< string, object> parameters )
        {
            Console.WriteLine("THIS MESSAGE MUST BE PRINTED IN DEBUGGED, OTHERWISE IS A FUCKING BAD NEWS !");
            // access to item
            SessionStorage storage = SessionStorage.GetStorage();
            PlaceItemSummary summary = storage.Get("selected_place") as PlaceItemSummary;
            Console.WriteLine("Display detail for " + summary.Title);

            PlaceItem place = await GetPlaceItem(summary);
            this.Title = place.Title;
            this.Description = place.Description;
            this.ImageId = place.ImageId;
            this.Comments = place.Comments;
            this._selectedPlace = place;

            // set map position to the latitude and longitude
            MyPosition = new Position( place.Latitude, place.Longitude );
            PinCollection.Clear();
            PinCollection.Add(new Pin()
            {
                Position = MyPosition,
                Type = PinType.Generic,
                Label = Title
            });
        }

        public async Task<PlaceItem> GetPlaceItem( PlaceItemSummary summary )
        {
            // creates a request to the API to load all places
            string placeItemURI = APIResources.buildPlaceItemURI( summary );

            // sends request to the api and returns list of place
            ApiClient client = new ApiClient();
            HttpResponseMessage httpResponse = await client.Execute(HttpMethod.Get, placeItemURI, null);

            // Parse response only if status code is OK
            if (httpResponse.IsSuccessStatusCode)
            {
                // parse response to return a list of place
                Response<PlaceItem> placesResponse = await client.ReadFromResponse<Response<PlaceItem>>(httpResponse);
                if (placesResponse.IsSuccess)
                {
                    Console.WriteLine("Get place item successed !");
                    return placesResponse.Data;
                }
            }
            Console.WriteLine("An error occured: Please review the code !");
            return null;

        }

        

        internal bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
