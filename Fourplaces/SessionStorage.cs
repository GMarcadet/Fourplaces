using System;
using System.Collections.Generic;
using System.Text;

namespace Fourplaces
{
    class SessionStorage
    {
        private static SessionStorage instance;

        public static SessionStorage GetStorage()
        {
            if ( instance == null )
            {
                instance = new SessionStorage(); 
            }
            return instance;
        }

        private Dictionary<string, object> data;

        public SessionStorage()
        {
            this.data = new Dictionary<string, object>();
        }

        public void Add( string key, object data )
        {
            if ( this.data.ContainsKey( key ) )
            {
                this.data.Remove(key);
            }
            this.data.Add(key, data);
        }

        public object Get( string key  )
        {
            return this.data[key];
        }
    }
}
