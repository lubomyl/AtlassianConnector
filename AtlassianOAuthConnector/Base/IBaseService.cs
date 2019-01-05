using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage.Basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Base
{

    /// <summary>
    /// Base interface for any http library implementation. 
    /// </summary>
    public interface IBaseService
    {

        /// <summary>
        /// HTTP GET operation.
        /// </summary>
        /// <typeparam name="K">Result of Get operation. Might be some type of Response object or deserialized model class object.</typeparam>
        /// <param name="resource">Resource identifier. (e. g. spaces, content, user?id=1)</param>
        /// <param name="resourceContext">Url context specifying path to resource. (e. g. /api/agile/latest)</param>
        /// <returns></returns>
        K Get<K>(string resource, string resourceContext) where K : new();

        /// <summary>
        /// HTTP POST operation.
        /// </summary>
        /// <param name="resource">Resource identifier. (e. g. spaces, content, user?id=1)</param>
        /// <param name="resourceContext">Url context specifying path to resource. (e. g. /api/agile/latest)</param>
        /// <param name="file">FileInfo object to send. Default null.</param>
        /// <param name="content">Content to send. Default null.</param>
        /// <param name="contentType">ContentType to send. Default application/json. Supporting multipart/form-data.</param>
        /// <returns></returns>
        void Post(string resource, string resourceContext, FileInfo file, string content, string contentType);

        /// <summary>
        /// HTTP POST with response operation.
        /// </summary>
        /// <param name="resource">Resource identifier. (e. g. spaces, content, user?id=1)</param>
        /// <param name="resourceContext">Url context specifying path to resource. (e. g. /api/agile/latest)</param>
        /// <param name="content">Content to send.</param>
        /// <returns></returns>
        K PostWithResponse<K>(string resource, string resourceContext, string content) where K : new();

        /// <summary>
        /// HTTP PUT operation.
        /// </summary>
        /// <param name="resource">Resource identifier. (e. g. spaces, content, user?id=1)</param>
        /// <param name="resourceContext">Url context specifying path to resource. (e. g. /api/agile/latest)</param>
        /// <param name="content">Content to send.</param>
        /// <returns></returns>
        void Put(string resource, string resourceContext, string content);

        /// <summary>
        /// HTTP DELETE operation.
        /// </summary>
        /// <param name="resource">Resource identifier. (e. g. spaces, content, user?id=1)</param>
        /// <param name="resourceContext">Url context specifying path to resource. (e. g. /api/agile/latest)</param>
        /// <returns></returns>
        void Delete(string resource, string resourceContext);
    }
}
