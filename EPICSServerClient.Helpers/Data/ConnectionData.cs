using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Helpers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EPICSServerClient.Helpers.Data
{
    public class ConnectionData
    {
        public static List<Connection> Connections = new List<Connection>();

        public static void ConvertConnectionsToXml()
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Connection>));
            var textWriter = new XmlTextWriter(FileConstants.EpicsServerClientConnectionsFile, Encoding.UTF8);
            xmlSerializer.Serialize(textWriter, Connections);
            textWriter.Close();
        }

        public static List<Connection> ConvertXmlToConnections()
        {
            if (!File.Exists(FileConstants.EpicsServerClientConnectionsFile))
                return new List<Connection>();
            using (var reader = new StreamReader(File.OpenRead(FileConstants.EpicsServerClientConnectionsFile)))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Connection>));
                Connections = xmlSerializer.Deserialize(reader) as List<Connection>;
                reader.Close();
                return Connections;
            }
        }

        public static void InitiateConnectionsXml()
        {
            if (!File.Exists(FileConstants.EpicsServerClientConnectionsFile))
                ConvertConnectionsToXml();
        }


    }
}
