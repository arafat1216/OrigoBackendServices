using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    //Cam be deleted
    public class CustomerSettingsException : Exception
    {
        public CustomerSettingsException(string? message) : base(message)
        {
        }
    }
}
