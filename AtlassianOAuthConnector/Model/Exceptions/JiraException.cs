using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Model.Exceptions
{
    public class JiraException : Exception
    {

        public ErrorResponse ErrorResponse { get; set; }

        public JiraException(ErrorResponse er) : base()
        {
            this.ErrorResponse = er;
        }

    }
}