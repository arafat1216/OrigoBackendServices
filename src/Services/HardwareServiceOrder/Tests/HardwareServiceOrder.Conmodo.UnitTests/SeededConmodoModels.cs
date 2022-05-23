namespace HardwareServiceOrder.Conmodo.UnitTests
{
    /// <summary>
    ///     Static class for creating/obtaining new entity instances that can be used during testing.
    /// </summary>
    internal static class SeededConmodoModels
    {
        /// <summary>
        ///     Returns a new <see cref="UpdatedOrdersResponse"/> instance.
        /// </summary>
        public static UpdatedOrdersResponse UpdatedOrdersResponse { get { return GetUpdatedOrdersResponse(); } }

        /// <summary>
        ///     Returns a new <see cref="OrderResponse"/> instance.
        /// </summary>
        public static OrderResponse OrderResponse1 { get { return GetOrderResponse1(); } }

        /// <summary>
        ///     Returns a new <see cref="OrderResponse"/> instance.
        /// </summary>
        public static OrderResponse OrderResponse2 { get { return GetOrderResponse2(); } }

        /// <summary>
        ///     Returns a new <see cref="OrderResponse"/> instance.
        /// </summary>
        public static OrderResponse OrderResponse3 { get { return GetOrderResponse3(); } }


        private static UpdatedOrdersResponse GetUpdatedOrdersResponse()
        {
            UpdatedOrdersResponse updatedOrdersResponse = new()
            {
                Order = new List<Order>
                {
                    new Order
                    {
                        CommId = "082c9ae0-ab03-4175-92a7-27d5a791cedc",
                        OrderNo = 10039296
                    },
                    new Order
                    {
                        CommId = "NOLF693-115",
                        OrderNo = 10003260
                    },
                    new Order
                    {
                        CommId = "1e45c7b6-8000-4ca4-a6c5-c5dc41ac4f63",
                        OrderNo = 10115186
                    },
                }
            };

            return updatedOrdersResponse;
        }

        private static OrderResponse GetOrderResponse1()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            OrderResponse orderResponse = new()
            {
                Reference = "Techstep AS Conmodo reperasjon.",
                DeltaOrderNumber = 10039296,
                WorkDescription = "",
                WorkshopID = 31,
                ProductInfoIn = new()
                {
                    Category = "Mobiltelefon",
                    Brand = "Samsung",
                    Model = "SM-G965F",
                    Imei = "352419093089189",
                    Accessories = new List<string>()
                },
                Events = new()
                {
                    new Event()
                    {
                        EventName = "Registrert",
                        EventId = 21,
                        EventDateTime = DateTimeOffset.Parse("2021-03-09T10:12:50.546086+01:00")
                    },
                    new Event()
                    {
                        EventName = "Feilregistrering",
                        EventId = 2,
                        EventDateTime = DateTimeOffset.Parse("2021-03-09T11:54:13.004505+01:00")
                    },
                    new Event()
                    {
                        EventName = "Garanti?",
                        EventId = 25007,
                        EventDateTime = DateTimeOffset.Parse("2021-03-09T10:12:50.546086+01:00")
                    }
                },
                OrderPrintURL = "http://localhost:8080/serviceportal/orderprint?orderid=10039296&pw=5413",
                RegisteredImeiIn = "352419093089189"
            };
#pragma warning restore CS0618 // Type or member is obsolete

            return orderResponse;
        }

        private static OrderResponse GetOrderResponse2()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            OrderResponse orderResponse = new()
            {
                Reference = "NOLF693-115",
                DeltaOrderNumber = 10003260,
                WorkDescription = "",
                WorkshopID = 31,
                ProductInfoIn = new()
                {
                    Category = "Annet",
                    Brand = "Apple",
                    Model = "AirPods Pro",
                },
                Events = new()
                {
                    new()
                    {
                        EventName = "Sendt Serviceverkstedet",
                        EventId = 4,
                        EventDateTime = DateTimeOffset.Parse("2020-11-07T16:45:13.44+01:00")
                    },
                    new()
                    {
                        EventName = "Registrert",
                        EventId = 21,
                        EventDateTime = DateTimeOffset.Parse("2020-11-07T16:45:13.379+01:00")
                    }
                },
                Packages = new()
                {
                    new Package()
                    {
                        Type = "Post",
                        Sender = "Dealer",
                        Packagenumber = "00707221504754994466",
                        Url = "https://sporing.bring.no/sporing/00707221504754994466"
                    }
                },
                OrderPrintURL = "http://localhost:8080/serviceportal/orderprint?orderid=10003260&pw=3340",
            };
#pragma warning restore CS0618 // Type or member is obsolete

            return orderResponse;
        }

        private static OrderResponse GetOrderResponse3()
        {

#pragma warning disable CS0618 // Type or member is obsolete
            OrderResponse orderResponse = new()
            {
                Reference = "Techstep AS Conmodo reperasjon.",
                DeltaOrderNumber = 10115186,
                WorkDescription = "",
                WorkshopID = 31,
                ProductInfoIn = new()
                {
                    Category = "Mobiltelefon",
                    Brand = "Apple",
                    Model = "iPhone 11 Pro",
                    Imei = "352819111586217",
                    Accessories = new List<string>()
                },
                Events = new()
                {
                    new Event()
                    {
                        EventName = "Garanti?",
                        EventId = 25007,
                        EventDateTime = DateTimeOffset.Parse("2021-06-14T12:57:46.880563+02:00")
                    },
                    new Event()
                    {
                        EventName = "Registrert",
                        EventId = 21,
                        EventDateTime = DateTimeOffset.Parse("2021-06-14T12:57:46.880563+02:00")
                    },
                    new Event()
                    {
                        EventName = "Feilregistrering",
                        EventId = 22,
                        EventDateTime = DateTimeOffset.Parse("2021-06-28T10:48:04.829667+02:00")
                    }
                },
                Messages = new()
                {
                    new Message()
                    {
                        MessageID = 13153516,
                        message = "Denne enheten har funksjonen Find My iPhone aktiv og kan ikke behandles av Conmodo før dette er deaktivert. ",
                        CreatedDateTime = DateTimeOffset.Parse("2021-06-14T12:58:38.283+02:00"),
                        Read = false,
                        Author = "Service"
                    },
                    new Message()
                    {
                        MessageID = 13153699,
                        message = "SAK:10115186\r\nDitt registrerte Apple-produkt i servicesystemet har \"Finn iPhone(FMiP)\" aktivert. \r\nDeaktiver \"Finn iPhone\" på følgende vis: \r\nFjern enheten fra aktuelle konto på icloud.com \r\n- Be brukeren/eieren logge inn med sin Apple-ID på www.icloud.com/find\r\n- Dersom det er flere produkter koblet til kontoen; Velg enheten som skal fjernes fra kontoen ved å klikke på \"Alle enheter\" øverst på siden.\r\n- Dersom enheten er offline/frakoblet (grå prikk til venstre for enheten), trykk på \"Fjern fra konto\" - IKKE \"Slett iPhone/iPad\".\r\n- Dersom enheten er online/tilkoblet (grønn prikk til venstre for enheten)(ta backup først), trykk så på \"Slett iPhone/iPad\". Når enheten er slettet, trykk da på \"Fjern fra konto\". Send oss en kommentar tilbake med at dette er utført.",
                        CreatedDateTime = DateTimeOffset.Parse("2021-06-14T13:50:56.508+02:00"),
                        Read = false,
                        Author = "Nikolai",
                    },
                    new Message()
                    {
                        MessageID = 13172627,
                        message = "Kjære kunde, din ordre 10115186 er kansellert. Det er viktig at du ikke benytter eventuelle fraktdokumenter du har mottatt til denne ordren.",
                        CreatedDateTime = DateTimeOffset.Parse("2021-06-28T10:48:04.837+02:00"),
                        Read = false,
                        Author = "Nikolai"
                    }
                },
                OrderPrintURL = "http://localhost:8080/serviceportal/orderprint?orderid=10115186&pw=6987",
                RegisteredImeiIn = "352819111586217"
            };
#pragma warning restore CS0618 // Type or member is obsolete

            return orderResponse;
        }
    }
}
