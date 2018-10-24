using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Model.Exceptions
{
    public class MissingParameterException : Exception
    {

        public ErrorResponse ErrorResponse { get; set; }

        public MissingParameterException(ErrorResponse er) : base()
        {
            this.ErrorResponse = er;
        }

    }
}