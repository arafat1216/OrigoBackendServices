using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public class AssetLabel : Entity
    {
        public Guid AssetId { get; protected set; }
        public Guid LabelId { get; protected set; }

        // Set to protected as DDD best practice
        protected AssetLabel()
        { }

        public AssetLabel(Guid assetId, Guid labelId)
        {
            AssetId = assetId;
            LabelId = labelId;
        }
    }
}
