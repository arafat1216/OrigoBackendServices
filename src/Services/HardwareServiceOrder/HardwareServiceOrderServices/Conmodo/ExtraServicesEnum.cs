namespace HardwareServiceOrderServices.Conmodo
{
    internal enum ExtraServicesEnum
    {
        /*
         * Norway only:
         */

        /// <summary>
        ///     Indicates that the item was sent in by a customer or a user, and not by the partner.
        /// </summary>
        /// <remarks> This service can only be used in Norway. </remarks>
        SentInByCustomerOrUser_NO = 304,

        /// <summary>
        ///     Indicates that the item should be returned to a customer or a user, and not to the partner.
        /// </summary>
        /// <remarks> This service can only be used in Norway. </remarks>
        ReturnToCustomerOrUser_NO = 310,

        /// <summary>
        ///     Unknown: Conmodo has not provided any details about this additional service. It is also (currently) not used or support 
        ///     by our solution, and is only added to this list to ensure the record of additional services is complete.
        /// </summary>
        /// <remarks> This service can only be used in Norway. </remarks>
        [Obsolete("Not supported.", true)]
        ReturnToRetailerOrAccountAddress_NO = 312,

        /// <summary>
        ///     When added
        /// </summary>
        /// <remarks> This service can only be used in Norway. </remarks>
        EmailShippingLabelToCustomerOrUser_NO = 219,

        /// <summary>
        ///     Add preswap to the service-order.
        /// </summary>
        /// <remarks> This service can only be used in Norway. </remarks>
        Preswap_NO = 308,


        /*
         * Sweden only:
         */

        /// <remarks> This service can only be used in Sweden. </remarks>
        /// <inheritdoc cref="SentInByCustomerOrUser_NO"/>
        SentInByCustomerOrUser_SE = 272,

        /// <remarks> This service can only be used in Sweden. </remarks>
        /// <inheritdoc cref="ReturnToCustomerOrUser_NO"/>
        ReturnToCustomerOrUser_SE = 279,

        /// <remarks> This service can only be used in Norway. </remarks>
        /// <inheritdoc cref=" ReturnToRetailerOrAccountAddress_NO"/>
        [Obsolete("Not supported.", true)]
        ReturnToRetailerOrAccountAddress_SE = 271,

        /// <remarks> This service can only be used in Sweden. </remarks>
        /// <inheritdoc cref="EmailShippingLabelToCustomerOrUser_NO"/>
        EmailShippingLabelToCustomerOrUser_SE = 320,

        /// <remarks> This service can only be used in Sweden. </remarks>
        /// <inheritdoc cref="Preswap_NO"/>
        Preswap_SE = 278
    }
}
