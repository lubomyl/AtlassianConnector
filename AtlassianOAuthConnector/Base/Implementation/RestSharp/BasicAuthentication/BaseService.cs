using DevDefined.OAuth.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.RestSharp
{

    //TODO implement error logging
    public class BaseService : RestClient, IBaseService
    {
        private static JiraService _jiraInstance = null;
        private static ConfluenceService _confluenceInstance = null;

        private string _baseUrl;
        private string _username;
        private string _password;

        protected BaseService()
        {
        }

        public void InitializeBasicAuthenticationAuthenticator(string baseUrl, string username, string password)
        {
            this._baseUrl = baseUrl;
            this._username = username;
            this._password = password;

            this.Authenticator = new HttpBasicAuthenticator(_username, _password);
        }

        /*
         * No reinitalization method
         * Username and Password are not stored for reinitalization
         * Every VS restart needs new authentication with user input
         */

        public T Get<T>(string resource, string resourceContext) where T : new()
        {
            var request = new RestRequest(resource);

            var response = Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                return default(T);
            }
        }

        public void Put(string resource, string resourceContext, byte[] content)
        {
            throw new NotImplementedException();
        }

        public void Post(string resource, string resourceContext, FileInfo file, byte[] content, string contentType)
        {
            throw new NotImplementedException();
        }

        public void Delete(string resource, string resourceContext)
        {
            throw new NotImplementedException();
        }

        public K PostWithResponse<K>(string resource, string resourceContext, byte[] content) where K : new()
        {
            throw new NotImplementedException();
        }

        public static ConfluenceService ConfluenceInstance
        {
            get
            {
                if (_confluenceInstance == null)
                {
                    _confluenceInstance = new ConfluenceService();
                }

                return _confluenceInstance;
            }
        }

        public static JiraService JiraInstance
        {
            get
            {
                if (_jiraInstance == null)
                {
                    _jiraInstance = new JiraService();
                }

                return _jiraInstance;
            }
        }

    }
}
