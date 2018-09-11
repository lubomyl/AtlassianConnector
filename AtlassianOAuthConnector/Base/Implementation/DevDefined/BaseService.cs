﻿using AtlassianConnector.Service;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var response = _session.Request().Get().ForUrl(_baseUrl + resourceContext + resource).ReadBody();

            if (response != null)
            {
                return JsonConvert.DeserializeObject<K>(response);
            }
            else
            {
                return default(K);
            }
        }

        /// <summary>
        /// <see cref="IBaseService{T}.Put{K}(string, string, string)"/>
        /// </summary>
        public void Put(string resource, string resourceContext, byte[] content)
        {
            var response = _session.Request().ForMethod("PUT").WithRawContentType("application/json").WithRawContent(content).ForUri(new Uri(_baseUrl + resourceContext + resource)).ToWebResponse();

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
