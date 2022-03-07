﻿using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public record OrigoOrderSim
    {
        /// <summary>
        /// The recipient name of the sim card.
        /// </summary>
        public string SendToName { get; set; }
        /// <summary>
        /// Send to either private or business address
        /// </summary>
        public Address Address { get; set; }

        public string OperatorName { get; set; }
        public int Quantity { get; set; }
    }
}
