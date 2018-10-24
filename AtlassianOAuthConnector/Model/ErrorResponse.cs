using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Model
{
    public class ErrorResponse
    {

        public string[] ErrorMessages { get; set; }
        public Dictionary<string, string> Errors { get; set; }

    }
}
