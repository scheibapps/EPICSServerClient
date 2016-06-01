using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using EPICSServerClient.Helpers.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net;
using System.Windows.Controls;

namespace EPICSServerClient.Helpers.Clients
{
    public class ParseClient : IParseService
    {
        public List<TabItem> GetClasses()
        {
            List<TabItem> Classes = new List<TabItem>();
            var classUrl = "/classes/Classes";
            var results = ParseData.HttpGet(classUrl);
            if (results == null)
                return null;
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

        public List<PFObject> GetClassObjects(string Class)
        {
            List<PFObject> objects = new List<PFObject>();
            var classUrl = "/classes/" + Class;
            var results = ParseData.HttpGet(classUrl);
            if (results == null)
                return null;
            foreach (JObject result in results.Children<JObject>())
            {
                foreach (JProperty data in result.Properties())
                {
                    foreach (JObject obj in data.Values())
                    {
                        PFObject parseObject = new PFObject();
                        foreach (JProperty prop in obj.Properties())
                            parseObject.AddProperty(prop.Name.ToString(), prop.Value.ToString());
                        objects.Add(parseObject);
                    }
                }
            }
            return objects;
        }

        public string PostClassObject(string Class,PFObject obj)
        {
            var classUrl = "/classes/" + Class;
            return ParseData.HttpPost(classUrl, obj);
        }

        public void DeleteClassObject(string Class, PFObject obj)
        {
            var classUrl = "/classes/" + Class;
            ParseData.HttpDelete(classUrl, obj);
        }

    }
}
