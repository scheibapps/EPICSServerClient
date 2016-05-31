using EPICSServerClient.Helpers.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static string Url = "";
        public static string AppId = "";
        public static string CurrentClass = "";
        public static DataGrid dataGrid;

        public static JArray RequestJArray(string Class)
        {
            try
            {
                WebRequest request = WebRequest.Create(Url + Class);
                request.Method = "GET";
                request.Headers["X-Parse-Application-Id"] = "hfjeuiry7oj23hlwer4";
                request.ContentType = "application/json";
                WebResponse wr = request.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                var json = "[" + content + "]"; // change this to array
                var results = JArray.Parse(json); // parse as array
                request.Abort();
                return results;
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return new JArray();
        }

        public static void PopulateDataGrid(DataGrid dg = null)
        {
            if (dg != null)
                dataGrid = dg;
            dataGrid.Columns.Clear();
            var first = dataGrid.ItemsSource.Cast<object>().FirstOrDefault() as ParseObject;
            if (first == null) return;
            var props = first.Properties;
            foreach (var prop in props)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = prop.Name,
                    Binding = new Binding(prop.Name),
                    IsReadOnly = true
                });
            }
        }
    }
}
