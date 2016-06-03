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

        public static string HttpTestClass(string Class)
        {
            try
            {
                WebRequest request = WebRequest.Create(Url + Class + "?limit=1");
                request.Method = "GET";
                request.Headers["X-Parse-Application-Id"] = AppId;
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    var results = ParseData.HttpGet(Class);
                    if (results == null)
                        return HttpStatusCode.NotImplemented.ToString();
                        foreach (JObject result in results.Children<JObject>())
                        {
                            foreach (JProperty data in result.Properties())
                            {
                                if(JArray.Parse(data.Value.ToString()).Children<JObject>().Count() < 1)
                                    return HttpStatusCode.NotImplemented.ToString();
                            }
                        }
                }
                return response.StatusCode.ToString();
            } catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string HttpTestObject(string Class)
        {
            try
            {
                PFObject obj = new PFObject();
                obj.AddProperty("objectId", String.Empty);
                obj.AddProperty("name", "Class");
                return HttpPost(Class, obj);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string HttpPost(string Class, PFObject obj)
        {
            try
            {
                PFObject jsonObj = new PFObject();
                List<Property> props = obj.Properties.Where(p =>
                {
                    if (p.Name == "objectId" || p.Name == "createdAt" || p.Name == "updatedAt")
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
                var response = (HttpWebResponse)request.GetResponse();
                request.Abort();
                string objectId = String.Empty;
                if (objId == String.Empty)
                {
                    try
                    {
                        string response_location = response.Headers["Location"].ToString();
                        char[] response_char = response_location.ToCharArray();
                        for (int i = 0; i < 10; i++)
                        {
                            objectId += response_char[response_char.Length - (10 - i)];
                        }
                    } catch(Exception e)
                    {
                        Debug.WriteLine(e.Message + " : Could not get the location header.");
                        return "0000000000";
                    }
                }
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

        public static string HttpUpdateOnDelete(string Class, PFObject obj)
        {
            HttpDelete(Class, obj);
            obj.SetPropertyValue("objectId", String.Empty);
            return HttpPost(Class, obj);
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
            var objects = dataGrid.ItemsSource.Cast<PFObject>();
            if (objects == null) return;
            var props = new List<Property>();
            foreach(var obj in objects)
            {
                foreach(var prop in obj.Properties)
                {
                    if (!props.Contains(prop))
                        props.Add(prop);
                }
            }
            foreach (var prop in props)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = prop.Name,
                    Binding = new Binding(prop.Name),
                    IsReadOnly = (prop.Name == "objectId" || prop.Name == "createdAt" || prop.Name == "updatedAt")
                });
            }
        }

        public static void AddDataGridColumn(string ColumnName)
        {
            if (dataGrid == null)
                return;
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = ColumnName,
                Binding = new Binding(ColumnName)
            });
        }

        public static bool DeleteDataGridColumn(string ColumnName)
        {
            if (dataGrid == null)
                return false;
            DataGridColumn column;
            try
            {
                column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == ColumnName);
            }
            catch (Exception)
            {
                column = null;
            }
            if (column == null)
                return false;
            dataGrid.Columns.Remove(column);
            return true;
        }
    }
}
