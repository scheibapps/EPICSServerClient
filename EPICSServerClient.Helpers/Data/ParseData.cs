using EPICSServerClient.Helpers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace EPICSServerClient.Helpers.Data
{
    public static class ParseData
    {
        public static string Url = String.Empty;
        public static string AppId = String.Empty;
        public static string CurrentClass = String.Empty;
        public static string ErrorMessage = String.Empty;
        public static DataGrid dataGrid;

        public static string HttpPost(string Class, PFObject obj)
        {
            try
            {
                PFObject jsonObj = new PFObject();
                List<Property> props = obj.Properties.Where(p =>
                {
                    if (p.Name == "objectId")
                        return false;
                    return true;
                }).ToList();
                foreach(var prop in props)
                {
                    jsonObj.AddProperty(prop);
                }
                string stream = JsonConvert.SerializeObject(jsonObj);
                byte[] bytes = Encoding.UTF8.GetBytes(stream);
                var objId = obj.Properties.FirstOrDefault(o => o.Name == "objectId").Value.ToString();
                var Parms = (objId == String.Empty) ? String.Empty : ("/" + objId);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url + Class + Parms);
                request.Method = (objId == String.Empty) ? "POST" : "PUT";
                request.Headers["X-Parse-Application-Id"] = AppId;
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                string objectId = String.Empty;
                if (objId == String.Empty)
                {
                    string response_location = response.Headers["Location"].ToString();
                    char[] response_char = response_location.ToCharArray();
                    for (int i = 0; i < 10; i++)
                    {
                        objectId += response_char[response_char.Length - (10 - i)];
                    }
                }
                request.Abort();
                return objectId;
            }catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return String.Empty;
        }

        public static JArray HttpGet(string Class)
        {
            try
            {
                WebRequest request = WebRequest.Create(Url + Class);
                request.Method = "GET";
                request.Headers["X-Parse-Application-Id"] = AppId;
                request.ContentType = "application/json";
                WebResponse wr = request.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                var json = "[" + content + "]"; // change this to array
                var results = JArray.Parse(json); // parse as array
                request.Abort();
                ErrorMessage = String.Empty;
                return results;
            } catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return null;
        }

        public static void HttpDelete(string Class, PFObject obj)
        {
            var objId = obj.Properties.FirstOrDefault(o => o.Name == "objectId").Value.ToString();
            if(objId != null)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url + Class + "/" + objId);
                    request.Method = "DELETE";
                    request.Headers["X-Parse-Application-Id"] = AppId;
                    request.ContentType = "application/json";
                    request.GetResponse();
                    request.Abort();
                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public static void PopulateDataGrid(DataGrid dg = null)
        {
            if (dg != null)
                dataGrid = dg;
            dataGrid.Columns.Clear();
            var first = dataGrid.ItemsSource.Cast<object>().FirstOrDefault() as PFObject;
            if (first == null) return;
            var props = first.Properties;
            foreach (var prop in props)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = prop.Name,
                    Binding = new Binding(prop.Name),
                    IsReadOnly = (prop.Name == "objectId")
                });
            }
        }
    }
}
