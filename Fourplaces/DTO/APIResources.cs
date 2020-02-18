using System;
using System.Collections.Generic;
using System.Text;
using TD.Api.Dtos;

namespace Fourplaces.DTO
{
    class APIResources
    {
        public static string API_URI_ROOT = "https://td-api.julienmialon.com";
        public static string API_URI_AUTH_LOGIN = "/auth/login";
        public static string API_URI_PLACES = "/places";
        public static string API_URI_IMG = "/images";

        public static string buildURI( string URI )
        {
            return API_URI_ROOT + URI;
        }

        internal static object buildImageURI(int idImagePlace)
        {
            return API_URI_ROOT + API_URI_IMG + "/" + idImagePlace;
        }

        internal static string buildUserProfilURI()
        {
            return API_URI_ROOT + "/me";
        }

        internal static string buildUpdatePasswordURI()
        {
            return API_URI_ROOT + "/me/password"; 
        }

        internal static string buildPlaceItemURI(PlaceItemSummary summary)
        {
            return API_URI_ROOT + API_URI_PLACES + "/" + summary.Id; 
        }

        internal static string buildRegisterURI()
        {
            return API_URI_ROOT + "/auth/register";
        }

        internal static string buildUpdateProfilURI()
        {
            return API_URI_ROOT + "/me";
        }

        internal static string buildCreateCommentURI( int placeId )
        {
            return API_URI_ROOT + "/places/" + placeId + "/comments"; 
        }

        internal static string buildImagePublicationURI()
        {
            return API_URI_ROOT + "/images";
        }

        internal static string buildPlacePublicationURI()
        {
            return API_URI_ROOT + "/places";
        }
    }
}
