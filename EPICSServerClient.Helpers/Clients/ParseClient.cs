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
            var classUrl = "/classes/Class";
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
                    if(objects.Count < 1)
                    {
                        PFObject obj = new PFObject();
                        obj.AddProperty("objectId", String.Empty);
                        obj.AddProperty("sampleColumn", "sampleData");
                        var objectId = PostClassObject(ParseData.CurrentClass, obj);
                        if (objectId != String.Empty)
                            obj.SetPropertyValue("objectId", objectId);
                        objects.Add(obj);
                    }
                }
            }
            return objects;
        }

        public string CheckForClass(string Class)
        {
            var classUrl = "/classes/"+Class;
            return ParseData.HttpTestClass(classUrl);
        }

        public string CreateClass(string Class)
        {
            var classUrl = "/classes/" + Class;
            return ParseData.HttpTestObject(classUrl);
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

        public string UpdateOnDelete(string Class, PFObject obj)
        {
            var classUrl = "/classes/" + Class;
            return ParseData.HttpUpdateOnDelete(classUrl, obj);
        }

    }
}
