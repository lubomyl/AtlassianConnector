using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.RestSharp
{
    public class JiraService : BaseService
    {

        private const string RESOURCE_CONTEXT = "/rest/api/latest/";

        public JiraService()
        {
        }

        public K GetResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT);
        }
    }
}
