using System.Collections.Concurrent;

namespace SIS.HTTP.Sessions
{
    public class HttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private static readonly ConcurrentDictionary<string, HttpSession> sessions =
            new ConcurrentDictionary<string, HttpSession>();

        public static IHttpSession GetSession(string id)
            => sessions.GetOrAdd(id, _ => new HttpSession(id));
    }
}