using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EPICSServerClient.Helpers.Models
{
    [XmlRoot("Connection")]
    public class Connection : IEquatable<Connection>, IEqualityComparer<Connection>
    {
        public Connection() { }

        public string DatabaseName { get; set; }
        public string Url { get; set; }
        public string AppId { get; set; }

        public bool Equals(Connection x, Connection y)
        {
            if (x == null || y == null)
                return false;
            if (x.DatabaseName == y.DatabaseName)
                return true;
            return false;
        }

        public int GetHashCode(Connection obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(Connection other)
        {
            if (other == null)
                return false;
            if (this.DatabaseName == other.DatabaseName)
                return true;
            return false;
        }
    }
}
