using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianConnector.Model.Exceptions
{
    public class UnauthorizedException : Exception
    {

        public ErrorResponse ErrorResponse { get; set; }

        public UnauthorizedException(string errorMessage) : base()
        {
            this.ErrorResponse = new ErrorResponse();
            this.ErrorResponse.ErrorMessages = new string[1];
            this.ErrorResponse.ErrorMessages[0] = errorMessage;
        }

    }
}
