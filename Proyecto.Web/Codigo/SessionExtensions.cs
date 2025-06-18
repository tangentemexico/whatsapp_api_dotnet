using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mgk.Base.WebCore.Codigo
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            try
            {
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {

            }
            return default(T);
        }

        public static T GetObjectFromJson<T>(string session)
        {
            var value = session;
            try
            {
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {

            }
            return default(T);
        }
    }
}
