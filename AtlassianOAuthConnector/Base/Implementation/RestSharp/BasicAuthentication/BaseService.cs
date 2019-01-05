using AtlassianConnector.Model;
using AtlassianConnector.Model.Exceptions;
using DevDefined.OAuth.Framework;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.RestSharp
{

    //TODO implement error logging
    public class BaseService : RestClient, IBaseService
    {
        private static JiraService _jiraInstance = null;
        private static ConfluenceService _confluenceInstance = null;

        private const int TIMEOUT = 5000;
        private const int EXTENDED_TIMEOUT = 50000;

        private string _baseUrl;
        private string _username;
        private string _password;

        protected BaseService()
        {
        }

        public void InitializeBasicAuthenticationAuthenticator(string baseUrl, string username, string password)
        {
            this._baseUrl = baseUrl;
            this.BaseUrl = new Uri(baseUrl);

            this._username = username;
            this._password = password;

            this.Authenticator = new HttpBasicAuthenticator(_username, _password);
        }

        public void DeleteAuthenticationCredentials()
        {
            this._username = null;
            this._password = null;
        }

        /*
         * No reinitalization method
         * Username and Password are not stored for reinitalization
         * Every VS restart needs new authentication with user input
         */

        public T Get<T>(string resource, string resourceContext) where T : new()
        {
            var request = new RestRequest(new Uri(this._baseUrl + resourceContext + resource), Method.GET);
            request.Timeout = TIMEOUT;

            var response = Execute<T>(request);

            if (response.ErrorException != null)
            {
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Access is denied due to invalid credentials.");
                }

                ErrorResponse er = new ErrorResponse();
                er.ErrorMessages = new string[1];
                er.ErrorMessages[0] = response.ErrorException.Message;

                throw new JiraException(er);
            }

            return response.Data;
        }

        public void Put(string resource, string resourceContext, string content)
        {
            var request = new RestRequest(new Uri(this._baseUrl + resourceContext + resource), Method.PUT);
            request.Timeout = TIMEOUT;
            request.AddParameter("application/json", content, ParameterType.RequestBody);

            var response = Execute(request);

            if (response.ErrorException != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Access is denied due to invalid credentials.");
                }

                ErrorResponse er = new ErrorResponse();
                er.ErrorMessages = new string[1];
                er.ErrorMessages[0] = response.ErrorException.Message;

                throw new JiraException(er);
            }
        }

        public void Post(string resource, string resourceContext, FileInfo file, string content, string contentType = "application/json")
        {
            var request = new RestRequest(new Uri(this._baseUrl + resourceContext + resource), Method.POST);

            if (contentType.Equals("multipart/form-data"))
            {
                request.AddHeader("X-Atlassian-Token", "no-check");
                request.Timeout = EXTENDED_TIMEOUT;
                request.AlwaysMultipartFormData = true;
                request.AddFile("file", file.FullName, "multipart/form-data");
            }
            else
            {
                request.Timeout = TIMEOUT;
                request.AddParameter("application/json", content, ParameterType.RequestBody);
            }

            var response = Execute(request);

            if (response.ErrorException != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Access is denied due to invalid credentials.");
                }

                ErrorResponse er = new ErrorResponse();
                er.ErrorMessages = new string[1];
                er.ErrorMessages[0] = response.ErrorException.Message;

                throw new JiraException(er);
            }
        }

        public void Delete(string resource, string resourceContext)
        {
            var request = new RestRequest(new Uri(this._baseUrl + resourceContext + resource), Method.DELETE);
            request.Timeout = TIMEOUT;

            var response = Execute(request);

            if (response.ErrorException != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Access is denied due to invalid credentials.");
                }

                ErrorResponse er = new ErrorResponse();
                er.ErrorMessages = new string[1];
                er.ErrorMessages[0] = response.ErrorException.Message;

                throw new JiraException(er);
            }
        }

        public K PostWithResponse<K>(string resource, string resourceContext, string content) where K : new()
        {
            var request = new RestRequest(new Uri(this._baseUrl + resourceContext + resource), Method.POST);
            request.Timeout = TIMEOUT;
            request.AddParameter("application/json", content, ParameterType.RequestBody);

            var response = Execute<K>(request);

            if (response.ErrorException != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Access is denied due to invalid credentials.");
                }

                ErrorResponse er = new ErrorResponse();
                er.ErrorMessages = new string[1];
                er.ErrorMessages[0] = response.ErrorException.Message;

                throw new JiraException(er);
            }

            return response.Data;
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
