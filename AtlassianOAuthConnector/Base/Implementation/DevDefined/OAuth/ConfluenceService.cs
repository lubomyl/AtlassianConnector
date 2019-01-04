using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.DevDefined
{
    public class ConfluenceService : BaseOAuthService
    {

        private const string REQUEST_TOKEN_URL_CONTEXT = "/wiki/plugins/servlet/oauth/request-token";
        private const string USER_AUTHORIZE_TOKEN_URL_CONTEXT = "/wiki/plugins/servlet/oauth/authorize";
        private const string ACCESS_TOKEN_URL_CONTEXT = "/wiki/plugins/servlet/oauth/access-token";
        private const string RESOURCE_CONTEXT = "/wiki/rest/api/latest/";

        public ConfluenceService() : base() {
            base.InitializeUris(REQUEST_TOKEN_URL_CONTEXT,
                USER_AUTHORIZE_TOKEN_URL_CONTEXT,
                ACCESS_TOKEN_URL_CONTEXT);
        }

        public K GetResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT);
        }
    }
}
