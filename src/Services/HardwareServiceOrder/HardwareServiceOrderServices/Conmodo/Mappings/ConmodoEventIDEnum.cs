namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    /// <summary>
    ///     An enum that uses Conmodo's event description, and maps it to their corresponding event-ID. 
    ///     The names reflects the name used in Conmodo's system. This means most enum names will be in either Norwegian or Swedish.
    /// </summary>
    /// <remarks>
    ///     <b>NB:</b> This only contains events that is used in several locations. Please see <see cref="EventMapper.GetStatusFromEventId(int)"/> for a complete list.
    /// </remarks>
    /// <see cref="EventMapper.GetStatusFromEventId(int)"/>
    internal enum ConmodoEventIdEnum
    {
        /*
         * Start statuses:
         * 
         * These are provided during the creation of the service-order.
         */

        // N/A


        /*
         * Payment statuses:
         * 
         * These reflects the users/customers payment status for the service-order.
         */

        /// <summary> 
        ///     Conmodo's description: "<c>Garanti</c>". <br/>
        ///     Type: Payment status.
        /// </summary>
        /// <remarks>
        ///     Indicates that the service is completely, or partially covered by a warranty
        /// </remarks>
        PaymentStatus_Garanti = 8,

        /// <summary> 
        ///     Conmodo's description: "<c>Betalbar</c>". <br/>
        ///     Type: Payment status. </summary>
        /// <remarks>
        ///     Indicates that the owner has to pay for the service-order (not covered by warranty).
        /// </remarks>
        PaymentStatus_Betalbar = 25009,


        /*
         * Service events:
         * 
         * This is actual service-events that represents that something is happening with the actual repair.
         */


        /// <summary> Conmodo's description: "<c>Mottatt Servicested</c>". </summary>
        /// <remarks>
        ///     This indicates that the package has been received by the repair-shop.
        /// </remarks>
        Mottatt_Servicested = 3,

        /// <summary> Conmodo's description: "<c>Feilregistrering</c>". </summary>
        /// <remarks>
        ///     Indicates that the service-order was registered by mistake, and has now been closed
        /// </remarks>
        Feilregistrering = 22,

        /// <summary> 
        ///     Conmodo's description: "<c>Mottatt Forhandler</c>". <para>
        ///     
        ///     Additional details received from e-mail communication: <br/>
        ///     "<c>Når en ordre har 'mottatt forhandler' er ikke ordren hos Conmodo. Den er da sendt og ordren mottatt, 
        ///     via pakkefunksjon i portal. Mulig denne aldri vil bli bruk av Techstep</c>". </para>
        /// </summary>
        Mottatt_Forhandler = 30,

        /// <summary> Conmodo's description: "<c>Levert Kunde</c>". </summary>
        /// <remarks>
        ///     Indicates that the package has been delivered to the customer.
        /// </remarks>
        Levert_Kunde = 31,

        /// <summary> Conmodo's description: "<c>Returneres ureparert</c>". </summary>
        Returneres_ureparert = 42,

        /// <summary> 
        ///     Conmodo's description: "<c>Pakke utlevert fra posten</c>". <para>
        ///     
        ///     Additional details received from e-mail communication: <br/>
        ///     "<c>Kommer kun dersom transportør har skannet utlevering. Ellers vil de ha annen siste status (e.g. 'Plukket opp av transportør' eller 'Sendt direkte til kunde')</c>". </para>
        /// </summary>
        /// <remarks>
        ///     Indicates that the package has handed over to the receiver. This status will only happen once the shipping provider has updated/scanned the delivery.
        /// </remarks>
        Pakke_utlevert_fra_posten = 25433,

    }
}
