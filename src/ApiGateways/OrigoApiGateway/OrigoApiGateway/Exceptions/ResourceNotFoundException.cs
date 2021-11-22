﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public ResourceNotFoundException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
