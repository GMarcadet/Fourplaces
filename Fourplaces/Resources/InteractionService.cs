using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Fourplaces.Resources
{
    
    class InteractionService 
    {
        public static async Task DisplayAlert(string title, string message)
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            await dialog.DisplayAlertAsync(title, message, "Close");
        }

        public static async Task DisplayMissingFields()
        {
            await DisplayAlert("Missing Field", "Some fields are missing, please fill them and retry");
        }
    }
}
