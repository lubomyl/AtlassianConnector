using DevDefined.OAuth.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.DevDefined
{
    public class JiraService : BaseOAuthService
    {

        private const string REQUEST_TOKEN_URL_CONTEXT = "/plugins/servlet/oauth/request-token";
        private const string USER_AUTHORIZE_TOKEN_URL_CONTEXT = "/plugins/servlet/oauth/authorize";
        private const string ACCESS_TOKEN_URL_CONTEXT = "/plugins/servlet/oauth/access-token";
        private const string RESOURCE_CONTEXT = "/rest/api/latest/";
        private const string RESOURCE_CONTEXT_10 = "/rest/api/1.0/";
        private const string RESOURCE_CONTEXT_AGILE = "/rest/agile/latest/";

        public JiraService() : base() {
            base.InitializeUris(REQUEST_TOKEN_URL_CONTEXT, 
                USER_AUTHORIZE_TOKEN_URL_CONTEXT, 
                ACCESS_TOKEN_URL_CONTEXT, 
                RESOURCE_CONTEXT);
        }

        public K GetResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT);
        }

        public K Get10Resource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT_10);
        }

        public K GetAgileResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT_AGILE);
        }

        public void PutResource(string resource, byte[] content)
        {
            base.Put(resource, RESOURCE_CONTEXT, content);
        }

        public void PostResourceContent(string resource, byte[] content)
        {
            base.Post(resource, RESOURCE_CONTEXT, null, content);
        }

        public void PostAgileResourceContent(string resource, byte[] content)
        {
            base.Post(resource, RESOURCE_CONTEXT_AGILE, null, content);
        }

        public K PostResourceContentWithResponse<K>(string resource, byte[] content) where K : new()
        {
            return base.PostWithResponse<K>(resource, RESOURCE_CONTEXT, content);
        }

        public void PostResourceFile(string resource, FileInfo file)
        {
            base.Post(resource, RESOURCE_CONTEXT, file, null, "multipart/form-data");
        }

        public void DeleteResource(string resource)
        {
            base.Delete(resource, RESOURCE_CONTEXT);
        }
    }
}
