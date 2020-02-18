using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Fourplaces.Resources
{
    class PermissionManager
    {
        public static async Task<bool> AskPermissions( Permission[] requestedPermissions )
        {
            // checks all permissions
            int permissionsNumber = requestedPermissions.Length;
            bool allPermissionsGranted = true;
            for ( int index = 0; index < permissionsNumber; index++ ) 
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(  requestedPermissions[ index ] );
                if ( status != PermissionStatus.Granted )
                {
                    allPermissionsGranted = false;
                }
            }

            if ( !allPermissionsGranted )
            {
                 await CrossPermissions.Current.RequestPermissionsAsync( requestedPermissions );
            }

            /*
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await DisplayAlert("Camera Permission", "Allow SavR to access your camera");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                status = results[Permission.Camera];
            }*/

            allPermissionsGranted = true;
            for (int index = 0; index < permissionsNumber; index++)
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(requestedPermissions[index]);
                if (status != PermissionStatus.Granted)
                {
                    allPermissionsGranted = false;
                }
            }

            return allPermissionsGranted;
        }
    }
}
