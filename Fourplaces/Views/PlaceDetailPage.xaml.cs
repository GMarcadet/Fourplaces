using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fourplaces.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceDetailPage : BaseContentPage
    {
        public PlaceDetailPage()
        {
            InitializeComponent();

            BindingContext = new PlaceDetailViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            PlaceDetailViewModel viewModel = BindingContext as PlaceDetailViewModel;
            return viewModel.OnBackButtonPressed();
        }
    }
}