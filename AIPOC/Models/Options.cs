using CommandLine.Attributes;
using System;

namespace AIPOC.Models
{
    class Options
    {
        [RequiredArgument(0, "endpoint", "The olsa endpoint")]
        public string endpoint { get; set; }

        [RequiredArgument(1, "customerid", "The olsa customerid")]
        public string customerid { get; set; }

        [RequiredArgument(2, "sharedsecret", "The olsa sharedsecret")]
        public string sharedsecret { get; set; }

        [OptionalArgument(1, "interval", "The interval in minutes between polls")]
        public int interval { get; set; }

        [OptionalArgument(10, "retries", "The maximum number of retries for polling")]
        public int retries { get; set; }

        [OptionalArgument("ALL_NOT_RESET", "mode", "The Asset Integration mode")]
        public string modeString { get; set; }

        public Olsa.assetInitiationMode mode
        {
            get
            {
                Olsa.assetInitiationMode result;
                if (Enum.TryParse(modeString, out result))
                {
                    return result;
                }
                else
                {
                    //If the string cannot be parsed use ALL
                    return Olsa.assetInitiationMode.all;
                }
            }
        }

    }
}
