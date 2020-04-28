/*
	Project Name: Airport Class DLL
	Written By: Thomas Ruigrok
    
    Copyright 2019-2020 By Thomas Ruigrok.
    
	This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.

    This Source Code Form is "Incompatible With Secondary Licenses", as
    defined by the Mozilla Public License, v. 2.0.
*/

using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Airports
{
    public class AirportInfo
    {
        //Variables
        public string API_Key { get; private set; }

        //Constructor
        /// <summary>
        /// Create an Instance of the Airports Class for looking up airport data
        /// </summary>
        /// <param name="pAPI_Key">Your API Key</param>
        public AirportInfo(string pAPI_Key)
        {
            API_Key = pAPI_Key;
        }


        //Methods
        /// <summary>
        /// Find airport using IATA code
        /// </summary>
        /// <exception cref="System.IO.InvalidDataException">Thrown When IATA Code is Invalid</exception>
        /// <param name="rLID">Requested IATA Code (3 Chars)</param>
        /// <returns>JSON String of Airport Data</returns>
        public string Find_By_IATA(string rIATA)
        {
            string apiUrl = "https://example.com";
            //TODO: Implement Method
            throw new NotImplementedException();
        }


        /// <summary>
        /// Find airport using ICAO code (4 Chars)
        /// </summary>
        /// <exception cref="System.IO.InvalidDataException">Thrown When ICAO Code is Invalid</exception>
        /// <param name="rICAO"></param>
        /// <returns>JSON String of Airport Data</returns>
        public string Find_By_ICAO(string rICAO)
        {
            //Variables
            string apiUrl = "https://v4p4sz5ijk.execute-api.us-east-1.amazonaws.com/anbdata/airports/locations/doc7910";
            Uri apiRequest;

            //check if code is posibly valid
            bool validCode = ValidateCode("ICAO", rICAO);
            
            //Throw Exception if code is invalid
            if(validCode == false)
            {
                throw new InvalidDataException("Invalid ICAO Code");
            }

            //Get the Api request url
            apiRequest = BuildAPICall(rICAO.ToUpper(), "ICAO", apiUrl);

            //Return JSON Object string
            return ConvertToJSONString(GetWebResponse(apiRequest));
        }


        /// <summary>
        /// Find airport using LID (Variable Chars)
        /// </summary>
        /// <param name="rLID"></param>
        /// <param name="pAuthority">Local Authority (FAA or TC)</param>
        /// <returns>JSON String of Airport Data</returns>
        public string Find_By_Lid(string rLID, string pAuthority)
        {
            //TODO: Implememt Method
            throw new NotImplementedException();
        }


        /// <summary>
        /// Builds the Uri to access api using passed data
        /// </summary>
        /// <exception cref="System.IO.InvalidDataException">Thrown when codeType is not ICAO, IATA, or LID</exception>
        /// <param name="code">ICAO, IATA, LID Code</param>
        /// <param name="codeType">ICAO, IATA or LID</param>
        /// <param name="apiUrl">Url Api</param>
        /// <returns>Built Uri with embeded params</returns>
        private Uri BuildAPICall(string code, string codeType, string apiUrl)
        {
            //Convert code and type to uppercase
            codeType = codeType.ToUpper();
            code = code.ToUpper();

            //Start the builder
            StringBuilder builder = new StringBuilder(apiUrl);

            //Determine params
            switch(codeType)
            {
                case "ICAO":
                    {
                        string paramList = "?api_key=" + API_Key + "&airports=" + code + "&format=json";
                        builder.Append(paramList);
                        break;
                    }
                case "IATA":
                    {
                        string paramList = "";
                        builder.Append(paramList);
                        break;
                    }
                case "LID":
                    {
                        string paramList = "";
                        builder.Append(paramList);
                        break;
                    }
                default:
                    {
                        throw new InvalidDataException("Invalid Code Type!");
                    }
            }

            //Returns the Uri
            return new Uri(builder.ToString());
        }


        /// <summary>
        /// Validates the passed code against codeType
        /// </summary>
        /// <param name="codeType">Permitted Values (ICAO, IATA & LID)</param>
        /// <param name="code">The actual code to test (Ex. CYGK, YGK, CNL3)</param>
        /// <returns>True if passed code is valid</returns>
        private bool ValidateCode(string codeType, string code)
        {
            bool isValid = false;

            //Convert to upper case
            code = code.ToUpper();
            codeType = codeType.ToUpper();

            switch (codeType)
            {
                case "ICAO":
                    {
                        if(code.Length == 4)
                        {
                            isValid = true;
                        }
                        break;
                    }
                case "IATA":
                    {
                        if(code.Length == 3)
                        {
                            isValid = true;
                        }
                        break;
                    }
                case "TC_LID":
                    {
                        if (Regex.IsMatch(code, "C[A-Z0-9]{3}") && code.Length == 4)
                        {
                            isValid = true;
                        }
                        break;
                    }
                case "FAA_LID":
                    {
                        if (Regex.IsMatch(code, "[A-Z0-9]{3}") && code.Length <= 4 && code.Length > 2)
                        {
                            isValid = true;
                        }
                        break;
                    }
                default:
                    {
                        isValid = false;
                        break;
                    }
            }

            return isValid;
                    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.Web.HttpException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <param name="pUri"></param>
        /// <returns></returns>
        private string GetWebResponse(Uri pUri)
        {
            string serverData = "";
            WebRequest request = WebRequest.Create(pUri);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            CheckResponse(response);

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            serverData = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            return serverData;
        }


        private void CheckResponse(HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {

            }
            else
            {
                string message = "HTTP Error: " + response.StatusCode.ToString() + response.StatusDescription.ToString();
                int statusCode = Convert.ToInt32(response.StatusCode);
                throw new HttpException(statusCode, response.StatusDescription.ToString());
            }
        }


        /// <summary>
        /// Convert from JSON Array to JSON Object
        /// </summary>
        /// <param name="jsonData">JSON Array to convert</param>
        /// <returns>JSON Object String</returns>
        private string ConvertToJSONString(string jsonData)
        {
            jsonData = jsonData.Replace("[", String.Empty);
            jsonData = jsonData.Replace("]", string.Empty);

            return jsonData;
        }

        /// <summary>
        /// Returns System.Drawing.Image of Airport from web
        /// </summary>
        /// <param name="code">ICAO, IATA, LID Code</param>
        /// <param name="codeType">ICAO, IATA, LID</param>
        /// <exception cref="System.NotImplementedException">Not Yet Implemented</exception>
        /// <returns>System.Drawing.Image</returns>
        public Image Get_Airport_Image(string code, string codeType)
        {

            throw new NotImplementedException();
        }
    }
}
