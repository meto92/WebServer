using System.Collections.Concurrent;

using SIS.HTTP.Sessions;

namespace SIS.MvcFramework.Sessions
{
    public class HttpSessionStorage : IHttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private readonly ConcurrentDictionary<string, HttpSession> sessions;

        public HttpSessionStorage()
        {
            this.sessions = new ConcurrentDictionary<string, HttpSession>();
        }        

        public IHttpSession GetSession(string sessionId)
            => sessions.GetOrAdd(sessionId, _ => new HttpSession(sessionId));

        public bool ContainsSession(string sessionId)
            => this.sessions.ContainsKey(sessionId);
    }
}