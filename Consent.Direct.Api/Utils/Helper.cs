using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Consent.Direct.Api.Utils
{
    class Helper
    {

        public static bool TrySerialise(object inObj, out string output)
        {
            try
            {
                JToken parsedJSON = JToken.FromObject(inObj);
                output = parsedJSON.ToString();
                return true;
            }
            catch (Exception ex)
            {
                output = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Validates JSON string
        /// </summary>
        /// <param name="inJSON">JSON to be validatedd</param>
        /// <param name="output">Valid JSON or Error Message</param>
        /// <returns>True if valid</returns>
        public static bool TryParseJSON(string inJSON, out string output)
        {
            try
            {
                JToken parsedJSON = JToken.Parse(inJSON);
                output = parsedJSON.ToString();
                return true;
            }
            catch (Exception ex)
            {
                output = ex.Message;
                return false;
            }
        }
    }
}