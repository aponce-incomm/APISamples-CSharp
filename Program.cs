using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Net;
using System.Web.Script.Serialization;

namespace TSG_API_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Settings
                string apiUrl = "https://sandbox.thesecuregateway.com/rest/v1/transactions";
                string apiKey = "a20effd6dc1d4512888e6b06d870248a";
                int timeout = 15000; //Milliseconds
                string lang_type = "json"; //"xml" or "json"

                //Populate Transaction Request Info
                transaction_request transaction_req = new transaction_request();
                transaction_req.api_key = apiKey;
                transaction_req.type = "SALE";
                transaction_req.card = "4111111111111111";
                transaction_req.csc = "123";
                transaction_req.exp_date = "1121";
                transaction_req.amount = "10.00";
                transaction_req.avs_address = "112 N. Orion Court";
                transaction_req.avs_zip = "20210";
                transaction_req.purchase_order = "10";
                transaction_req.invoice = "100";
                transaction_req.email = "email@tsg.com";
                transaction_req.customer_id = "25";
                transaction_req.order_number = "1000";
                transaction_req.client_ip = "";
                transaction_req.description = "Cel Phone";
                transaction_req.comments = "Electronic Product";

                billing bl = new billing();
                bl.first_name = "Joe";
                bl.last_name = "Smith";
                bl.company = "Company Inc.";
                bl.street = "Street 1";
                bl.street2 = "Street 2";
                bl.city = "Jersey City";
                bl.state = "NJ";
                bl.zip = "07097";
                bl.country = "USA";
                bl.phone = "123456789";
                transaction_req.billing = bl;

                shipping sh = new shipping();
                sh.first_name = "Joe";
                sh.last_name = "Smith";
                sh.company = "Company 2 Inc.";
                sh.street = "Street 1 2";
                sh.street2 = "Street 2 2";
                sh.city = "Colorado City";
                sh.state = "TX";
                sh.zip = "79512";
                sh.country = "USA";
                sh.phone = "123456789";
                transaction_req.shipping = sh;

                string request = "";
                if ("xml".Equals(lang_type))
                {
                    //Serialize transaction object to XML representation
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(transaction_request));
                    MemoryStream ms = new MemoryStream();
                    Encoding Utf8 = new UTF8Encoding(false);
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(ms, Utf8);
                    xmlTextWriter.Formatting = Formatting.Indented;
                    serializer.Serialize(xmlTextWriter, transaction_req, ns);
                    request = Utf8.GetString(ms.ToArray());
                }
                else
                {
                    //Serialize transaction object to JSON representation
                    transaction_req.avs_address = null;
                    transaction_req.avs_zip = null;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string tmp = serializer.Serialize(transaction_req);
                    request=tmp.Replace("\"avs_address\":null,\"avs_zip\":null,",""); //avoid avs_address and avs_zip fields
                }

                //Execute request to gateway
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("REQUEST TO URL: " + apiUrl);
                Console.WriteLine("POST DATA: " +Environment.NewLine + request);
                HttpWebRequest req = WebRequest.Create(new Uri(apiUrl)) as HttpWebRequest;
                req.Method = "POST";
                req.ContentType = "application/"+lang_type;
                req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                byte[] dataByteLen = UTF8Encoding.UTF8.GetBytes(request);
                req.ContentLength = dataByteLen.Length;
                req.Timeout = timeout;
                Stream post = req.GetRequestStream();
                post.Write(dataByteLen, 0, dataByteLen.Length);
                post.Close();

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(resp.GetResponseStream());

                //handle response
                string response = reader.ReadToEnd();

                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("RESPONSE DATA: " + Environment.NewLine + response);

                if ((response.Contains("<transaction>")) || (response.Contains("\"transaction\"")))
                {
                    transaction_response transaction_res;
                    if ("xml".Equals(lang_type))
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(transaction_response));
                        transaction_res = (transaction_response)serializer.Deserialize(new StringReader(response));
                    }
                    else
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        //remove 'transaction' element from the string response
                        response = response.Replace("{\"transaction\":", "");
                        response = response.Remove(response.Length-1);
                        transaction_res = serializer.Deserialize<transaction_response>(response);
                    }
                    

                    if (transaction_res.result_code != null && transaction_res.result_code == "0000")
                    {
                        Console.WriteLine("-----------------------------------------------------");
                        Console.WriteLine("TRANSACTION APPROVED: " + transaction_res.authorization_code);
                    }
                    else
                    {
                        String code = "";
                        if (transaction_res.error_code != null)
                            code = transaction_res.error_code;
                        if (transaction_res.result_code != null)
                            code = transaction_res.result_code;
                        Console.WriteLine("-----------------------------------------------------");
                        Console.WriteLine("TRANSACTION ERROR: Code=" + code + " Message=" + transaction_res.display_message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("EXCEPTION: " + e.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
