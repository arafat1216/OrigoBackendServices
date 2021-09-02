using System;

namespace AssetServices.Exceptions
{
    public class InvalidAssetDataException : Exception
    {
        public InvalidAssetDataException(string msg) : base(msg)
        {

        }
    }
}
