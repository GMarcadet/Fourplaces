using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.Resources
{

    public enum PictureSelectionMode
    {
        PICK_FROM_GALLERY,
        PICK_FROM_CAMERA,
    }
    

    class ImageManagerService
    {

  
        public static async Task<MediaFile> PickImage( PictureSelectionMode selectionMode )
        {
            bool permissionGranted = await AskPermissions();
            if (!permissionGranted)
            {
                await DisplayAlert("Missing Permissions", "Some permissions are missing");
                return null;
            }


            await CrossMedia.Current.Initialize();

            

            if ( selectionMode == PictureSelectionMode.PICK_FROM_GALLERY )
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Error", "No media available");
                    return null;
                }

                return await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Small,
                });
            }

            if ( selectionMode == PictureSelectionMode.PICK_FROM_CAMERA )
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("Error", "No media available");
                    return null;
                }

                return await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Small,
                });
            }

            return null;
            

        }


        private static async Task<bool> AskPermissions()
        {
            return await PermissionManager.AskPermissions(new Permission[] { Permission.Camera, Permission.Storage });
        }

        private static async Task DisplayAlert(string title, string message)
        {
            IDialogService dialog = DependencyService.Get<IDialogService>();
            await dialog.DisplayAlertAsync(title, message, "Close");
        }
    }
}
