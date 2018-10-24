using AtlassianConnector.Model;
using AtlassianConnector.Model.Exceptions;
using AtlassianConnector.Service;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.DevDefined
{


    /// <summary>
    /// DevDefined.OAuth library concrete implementation of IBaseService.
    /// Singleton class pattern used to not to reinitialize OAuth session on every reference of this class.
    /// </summary>
    public class BaseService : IBaseService<IToken>
    {
        private static OAuthSession _session;

        private static JiraService _jiraInstance = null;
        private static ConfluenceService _confluenceInstance = null;

        private string _baseUrl;

        private string _requestTokenUrlContext, _userAuthorizeTokenUrlContext, _accessTokenUrlContext;

        protected BaseService()
        {
        }

        protected void InitializeUris(string requestTokenUrl, string userAuthorizeTokenUrlContext, string accessTokenUrlContext, string resourceContext)
        {
            this._requestTokenUrlContext = requestTokenUrl;
            this._userAuthorizeTokenUrlContext = userAuthorizeTokenUrlContext;
            this._accessTokenUrlContext = accessTokenUrlContext;
        }

        /// <summary>
        /// Initializes OAtuh session object with parameters needed like requestTokenUrl, userAuthorieUrl, accessTokenUrl, consumerKey, privateKey, signatureMethod or consumerSecret 
        /// </summary>
        public void InitializeOAuthSession(string baseUrl)
        {
            this._baseUrl = baseUrl;

            X509Certificate2 certificate = new X509Certificate2(Properties.Settings.Default.CertificatePath, Properties.Settings.Default.CertificateSecret);

            string requestTokenUrl = this._baseUrl + this._requestTokenUrlContext;
            string userAuthorizeTokenUrl = this._baseUrl + this._userAuthorizeTokenUrlContext;
            string accessTokenUrl = this._baseUrl + this._accessTokenUrlContext;

            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = Properties.Settings.Default.ConsumerKey,
                ConsumerSecret = Properties.Settings.Default.ConsumerSecret,
                SignatureMethod = SignatureMethod.RsaSha1,
                Key = certificate.PrivateKey,
                UseHeaderForOAuthParameters = true
            };

            _session = new OAuthSession(consumerContext, requestTokenUrl, userAuthorizeTokenUrl, accessTokenUrl);
        }

        /// <summary>
        /// Reinitializes OAuth session object with provided access token properties. Used on restart of application. Important for remember me like function.
        /// </summary>
        /// <param name="token">Access token string.</param>
        /// <param name="tokenSecret">Access token secret string.</param>
        public void ReinitializeOAuthSessionAccessToken(string token, string tokenSecret, string baseUrl)
        {
            this.InitializeOAuthSession(baseUrl);

            IToken accessToken = new TokenBase();
            accessToken.Token = token;
            accessToken.TokenSecret = tokenSecret;

            _session.AccessToken = accessToken;
        }


        /// <summary>
        /// <see cref="IBaseService{T}.Get{K}(string, string)"/>
        /// </summary>
        public K Get<K>(string resource, string resourceContext) where K : new()
        {
            try
            {
                var response = _session.Request().Get().ForUrl(_baseUrl + resourceContext + resource).ReadBody();

                if (response != null)
                {
                    return JsonConvert.DeserializeObject<K>(response);
                }
                else
                {
                    return default(K);
                }
            } catch(Exception ex)
            {
                return default(K);
            }
        }

        /// <summary>
        /// <see cref="IBaseService{T}.Put(string, string, byte)"/>
        /// </summary>
        public void Put(string resource, string resourceContext, byte[] content)
        {
            var response = _session.Request().ForMethod("PUT").WithRawContentType("application/json").WithRawContent(content).ForUri(new Uri(_baseUrl + resourceContext + resource)).ToWebResponse();

            response.Close();
        }

        /// <summary>
        /// <see cref="IBaseService{T}.Post(string, string, byte, string)"/>
        /// </summary>
        public void Post(string resource, string resourceContext, FileInfo file = null, byte[] content = null, string contentType = "application/json")
        {
            var request = _session.Request();
            request.ForMethod("POST");
            request.ForUri(new Uri(_baseUrl + resourceContext + resource));
            request.WithTimeout(5000);

            if (contentType.Equals("multipart/form-data")) {
                request.WithHeaders(new Dictionary<string, string> { { "X-Atlassian-Token", "no-check" } });
                PostMultiPart(request, file);
            }
            else
            {
                request.WithRawContentType(contentType);
                request.WithRawContent(content);

                var response = request.ToWebResponse();

                response.Close();
            }    
        }

        /// <summary>
        /// <see cref="IBaseService{T}.PostWithResponse(string, string, byte)"/>
        /// </summary>
        public K PostWithResponse<K>(string resource, string resourceContext, byte[] content) where K : new()
        {
            var request = _session.Request();
            request.ForMethod("POST");
            request.ForUri(new Uri(_baseUrl + resourceContext + resource));
            request.WithTimeout(5000);

            request.WithRawContentType("application/json");
            request.WithRawContent(content);

            HttpWebRequest webRequest = request.ToWebRequest();

            try
            {
                using (var response = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            return JsonConvert.DeserializeObject<K>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = wex.Response as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            ErrorResponse er = JsonConvert.DeserializeObject<ErrorResponse>(reader.ReadToEnd());

                            throw new MissingParameterException(er);
                        }
                    }
                }
            }

            return default(K);
        }

        public void Delete(string resource, string resourceContext)
        {
            var response = _session.Request().ForMethod("DELETE").ForUri(new Uri(_baseUrl + resourceContext + resource)).ToWebResponse();

            response.Close();
        }

        /// <summary>
        /// <see cref="IBaseService{T}.GetRequestToken"/>
        /// </summary>
        public IToken GetRequestToken()
        {
            IToken ret = _session.GetRequestToken("POST");

            return ret;
        }

        /// <summary>
        /// <see cref="IBaseService{T}.GetUserAuthorizationUrlForToken(T)"/>
        /// </summary>
        public string GetUserAuthorizationUrlForToken(IToken requestToken)
        {
            string ret = _session.GetUserAuthorizationUrlForToken(requestToken);

            return ret;
        }

        /// <summary>
        /// <see cref="IBaseService{T}.ExchangeRequestTokenForAccessToken(T, string)"/>
        /// </summary>
        public IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string verificationCode)
        {

            IToken ret = _session.ExchangeRequestTokenForAccessToken(requestToken, "POST", verificationCode);

            return ret;
        }

        //DevDefined.OAuth library doesn't support multipart/form-data contentType
        //This method takes 
        private bool PostMultiPart(IConsumerRequest devDefinedRequest, FileInfo filePath)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = devDefinedRequest.ToWebRequest();

            try
            {
                var boundary = string.Format("----------{0:N}", Guid.NewGuid());
                var content = new MemoryStream();
                var writer = new StreamWriter(content);

                var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                writer.WriteLine("--{0}", boundary);
                writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", filePath.Name);
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();

                content.Write(data, 0, data.Length);

                writer.WriteLine();

                writer.WriteLine("--" + boundary + "--");
                writer.Flush();
                content.Seek(0, SeekOrigin.Begin);

                if (request == null)
                {
                    return false;
                }

                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                request.ContentLength = content.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    content.WriteTo(requestStream);
                    requestStream.Close();
                }

                using (response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var reader = new StreamReader(response.GetResponseStream());
                        return false;
                    }

                    return true;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        var reader = new StreamReader(errorResponse.GetResponseStream());
                    }
                }

                if (request != null)
                {
                    request.Abort();
                }

                return false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public static ConfluenceService ConfluenceInstance
        {
            get
            {
                if (_confluenceInstance == null)
                {
                    _confluenceInstance= new ConfluenceService();
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
