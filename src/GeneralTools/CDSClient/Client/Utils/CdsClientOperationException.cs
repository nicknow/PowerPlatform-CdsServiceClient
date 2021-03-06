﻿using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerPlatform.Cds.Client.Utils
{
    /// <summary>
    /// Used to encompass a ServiceClient Operation Exception 
    /// </summary>
    [Serializable]
    public class CdsClientOperationException : Exception
    {
        /// <summary>
        /// Creates a CdsService Client Exception
        /// </summary>
        /// <param name="message">Error Message</param>
        public CdsClientOperationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Creates a CdsService Client Exception
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="errorCode">Error code</param>
        /// <param name="data">Data Properties</param>
        /// <param name="helpLink">Help Link</param>
        public CdsClientOperationException(string message, int errorCode , string helpLink , IDictionary<string,string> data)
            : base(message)
        {
            HResult = errorCode;
            HelpLink = helpLink;
            Source = "Cds Server API";
            foreach (var itm in data)
            {
                this.Data.Add(itm.Key, itm.Value);
            }
        }

        /// <summary>
        /// Creates a CdsService Client Exception from a httpOperationResult. 
        /// </summary>
        /// <param name="httpOperationException"></param>
        public static CdsClientOperationException GenerateCdsClientOperationException(HttpOperationException httpOperationException )
        {
            string errorDetailPrefixString = "@Microsoft.PowerApps.CDS.ErrorDetails.";
            Dictionary<string, string> cdsErrorData = new Dictionary<string, string>();

            JObject contentBody = JObject.Parse(httpOperationException.Response.Content);
            var ErrorBlock = contentBody["error"];
            var errorMessage = CdsTraceLogger.GetFirstLineFromString(ErrorBlock["message"].ToString()).Trim();
            int HResult = Convert.ToInt32(ErrorBlock["code"].ToString(), 16);
            string HelpLink = ErrorBlock["@Microsoft.PowerApps.CDS.HelpLink"].ToString();

            foreach (var node in ErrorBlock.ToArray())
            {
                if ( node.Path.Contains(errorDetailPrefixString))
                {
                    cdsErrorData.Add(node.Value<JProperty>().Name.ToString().Replace(errorDetailPrefixString, string.Empty), node.HasValues? node.Value<JProperty>().Value.ToString() : string.Empty);
                }
            }
            return new CdsClientOperationException(errorMessage, HResult, HelpLink, cdsErrorData);
        }

        /// <summary>
        /// Creates a CdsService Client Exception
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Supporting Exception</param>
        public CdsClientOperationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a CdsService Client Exception
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected CdsClientOperationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
             : base(serializationInfo, streamingContext)
        {
        }
    }
}
