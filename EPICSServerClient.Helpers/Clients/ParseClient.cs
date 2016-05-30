using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using EPICSServerClient.Helpers.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Windows.Controls;

namespace EPICSServerClient.Helpers.Clients
{
    public class ParseClient : IParseService
    {
        public List<TabItem> GetClasses()
        {
            List<TabItem> Classes = new List<TabItem>();
            var results = ParseData.RequestJArray("Classes");
            foreach (JObject result in results.Children<JObject>())
            {
                foreach (JProperty data in result.Properties())
                {
                    foreach (JObject dbs in JArray.Parse(data.Value.ToString()).Children<JObject>())
                    {
                        dynamic dB = JObject.Parse(dbs.ToString());
                        string name = dB.name;
                        Classes.Add(new TabItem { Header = dB.name });
                    }
                }
            }
            return Classes;
        }

        public List<ParseObject> GetClassObjects(string Class)
        {
            List<ParseObject> objects = new List<ParseObject>();
            var results = ParseData.RequestJArray(Class);
            foreach (JObject result in results.Children<JObject>())
            {
                foreach (JProperty data in result.Properties())
                {
                    foreach (JObject obj in data.Values())
                    {
                        ParseObject parseObject = new ParseObject();
                        foreach (JProperty prop in obj.Properties())
                            parseObject.AddProperty(prop.Name.ToString(), prop.Value.ToString());
                        objects.Add(parseObject);
                    }
                }
            }
            return objects;
        }

    }
}
