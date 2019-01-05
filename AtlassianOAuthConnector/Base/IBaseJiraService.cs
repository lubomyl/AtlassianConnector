using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base
{
    public interface IBaseJiraService
    {

        K GetResource<K>(string resource) where K : new();

        K Get10Resource<K>(string resource) where K : new();

        K GetAgileResource<K>(string resource) where K : new();

        void PutResource(string resource, string content);

        void PostResourceContent(string resource, string content);

        void PostAgileResourceContent(string resource, string content);

        K PostResourceContentWithResponse<K>(string resource, string content) where K : new();

        void PostResourceFile(string resource, FileInfo file);

        void DeleteResource(string resource);

    }
}
