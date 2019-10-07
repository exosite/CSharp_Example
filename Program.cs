using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Json;

namespace python_env
{
    class Program
    {

        public static void Main(string[] args)
        {
            string productID = GetProductID();
            string deviceID = "csharpdevice3";

            string tokenFile = GetTokenFile();
            if (!File.Exists(tokenFile))
                provision(productID, deviceID);

            string token = GetToken();

            JsonObject data_in = new JsonObject();
            Random rand = new Random();
            int rand_value = rand.Next(1,100);
            data_in.Add("rand_val", rand_value);

            WriteResource(productID, "data_in", data_in.ToString());
        }

        // Sends a POST request to provision the device
        //     If the device is provisioned successfully, a token is returned
        //     We save that token to token.txt for future use
        private static void provision(string productID, string deviceID)
        {
            string url = "https://" + productID+ ".m2.exosite.io/";

            string provisionEndpoint = "provision/activate";

            url = url + provisionEndpoint;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Host = productID + ".m2.exosite.io";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;

            /// Body
            string body = "id="+deviceID;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] payloadBytes = encoding.GetBytes(body);
            request.ContentLength = payloadBytes.Length;
            Stream newStream = request.GetRequestStream ();
            newStream.Write (payloadBytes, 0, payloadBytes.Length);
            /// End Body

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                string MSG = reader.ReadLine();
                File.WriteAllText(GetTokenFile(),MSG);
            }
            catch(Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

        // Gets the full filepath/name of the token.txt file
        private static string GetTokenFile()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string tokenFile = CurrentDirectory + Path.DirectorySeparatorChar + "token.txt";
            return tokenFile;
        }
        
        // Reads the contents of token.txt which contains the authentication token
        private static string GetToken()
        {
            return System.IO.File.ReadAllText(GetTokenFile());
        }

        // Gets the productID from product_id.txt
        //    Can be changed/overriden if needed
        private static string GetProductID()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string productIDFile = CurrentDirectory + Path.DirectorySeparatorChar + "product_id.txt";
            
            if (!File.Exists(productIDFile))
            {
                throw new System.InvalidOperationException("Need to have product_id.txt with the productID in the contents!");
            }

            string productId =  System.IO.File.ReadAllText(productIDFile);
            // Remove Whitespaces and newlines that may accidentally be in the file
            productId = Regex.Replace(productId, @"\s+", String.Empty);
            return productId;
        }

        // Writes a value to a resource in Murano
        //    The most common resources to write to are config_io and data_in
        //    For more info on resources, see the industrial IoT Schema: https://github.com/exosite/industrial_iot_schema/blob/master/channel-signal_io_schema.md
        //    More documentation is also available here: https://exosense.readme.io/docs/device-interface-overview
        private static void WriteResource(string productID, string resource, string resource_value)
        {
            Console.WriteLine("Writing Data...");
            string url = "https://" + productID+ ".m2.exosite.io/";

            string provisionEndpoint = "onep:v1/stack/alias";

            url = url + provisionEndpoint;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Host = productID + ".m2.exosite.io";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.Method = "POST";
            string token = GetToken();
            request.Headers["X-Exosite-CIK"] = token;
            request.ProtocolVersion = HttpVersion.Version11;

            /// Body
            string body = resource + "=" + resource_value;;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] payloadBytes = encoding.GetBytes(body);
            request.ContentLength = payloadBytes.Length;
            Stream newStream = request.GetRequestStream ();
            newStream.Write (payloadBytes, 0, payloadBytes.Length);
            /// End Body

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                string MSG = reader.ReadLine();
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(MSG);
            }
            catch(Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }
    }
}
