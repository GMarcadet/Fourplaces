using Fourplaces.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Storm.Mvvm.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fourplaces.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePlacePage : BaseContentPage
    {

        public Image SelectedImage;
        public CreatePlacePage()
        {
            InitializeComponent();

            BindingContext = new CreatePlaceViewModel();
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            MediaFile selectedMedia = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            this.SelectedImage.Source = ImageSource.FromStream(() => selectedMedia.GetStream());
        }
    }
}