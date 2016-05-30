using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Helpers.Models
{
    public class ParseObject : DynamicObject
    {
        private readonly IList<Property> _properties;

        public ParseObject()
        {
            _properties = new List<Property>();
        }

        public IList<Property> Properties => _properties;

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Select(p => p.Name);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties.First(p => p.Name.Equals(binder.Name)).Value = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _properties.FirstOrDefault(p => p.Name.Equals(binder.Name)).Value;
            return true;
        }

        public void AddProperty(Property property)
        {
            _properties.Add(property);
        }

        public void AddProperty(string name, object value)
        {
            var property = new Property()
            {
                Name = name,
                Value = value
            };

            _properties.Add(property);
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            _properties.First(p => p.Name.Equals(propertyName)).Value = value;
        }

        public object GetPropertyValue(string propertyName)
        {
            return _properties.First(p => p.Name.Equals(propertyName)).Value;
        }
    }

    public class Property
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
