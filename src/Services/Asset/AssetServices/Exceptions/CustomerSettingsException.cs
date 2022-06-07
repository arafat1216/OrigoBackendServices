using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class CustomerSettingsException : Exception
    {
        public CustomerSettingsException(string? message) : base(message)
        {
        }
    }
}
