using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base.Implementation.RestSharp
{
    public class JiraService : BaseService, IBaseJiraService
    {

        private const string RESOURCE_CONTEXT = "/rest/api/latest/";
        private const string RESOURCE_CONTEXT_10 = "/rest/api/1.0/";
        private const string RESOURCE_CONTEXT_AGILE = "/rest/agile/latest/";

        public JiraService() : base()
        {
        }

        public void DeleteResource(string resource)
        {
            base.Delete(resource, RESOURCE_CONTEXT);
        }

        public K Get10Resource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT_10);
        }

        public K GetAgileResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT_AGILE);
        }

        public K GetResource<K>(string resource) where K : new()
        {
            return base.Get<K>(resource, RESOURCE_CONTEXT);
        }

        public void PostAgileResourceContent(string resource, string content)
        {
            base.Post(resource, RESOURCE_CONTEXT_AGILE, null, content);
        }

        public void PostResourceContent(string resource, string content)
        {
            base.Post(resource, RESOURCE_CONTEXT, null, content);
        }

        public K PostResourceContentWithResponse<K>(string resource, string content) where K : new()
        {
            return base.PostWithResponse<K>(resource, RESOURCE_CONTEXT, content);
        }

        public void PostResourceFile(string resource, FileInfo file)
        {
            base.Post(resource, RESOURCE_CONTEXT, file, null, "multipart/form-data");
        }

        public void PutResource(string resource, string content)
        {
            base.Put(resource, RESOURCE_CONTEXT, content);
        }
    }
}
