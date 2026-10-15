using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Exceptions
{
    public class UnauthorizedException:Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
