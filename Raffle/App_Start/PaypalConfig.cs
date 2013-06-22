using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Raffle.App_Start
{
    public static class PaypalConfig
    {
        public static string BusinessEmail { get; private set; }

        public static void Initilize(NameValueCollection settings)
        {
            PayPal.Profile.Initialize(settings["PaypalApiUsername"], settings["PaypalApiPassword"], settings["PaypalApiSignature"], settings["PaypalEnvironment"]);
            BusinessEmail = settings["PaypalBusinessEmail"];
        }
    }
}