using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Helpers.Models
{
    public class PFObject : DynamicObject, INotifyPropertyChanged
    {
        private readonly IList<Property> _properties;

        public event PropertyChangedEventHandler PropertyChanged;

        public PFObject()
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
            try
            {
                _properties.First(p => p.Name.Equals(binder.Name)).Value = value;
            } catch(Exception e)
            {
                AddProperty(binder.Name, value);
                Debug.WriteLine(e.Message + " : Property auto-assigned");
            }
            NotifyPropertyChanged(binder.Name);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = _properties.FirstOrDefault(p => p.Name.Equals(binder.Name)).Value.ToString();
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message + " : Value may be null");
                result = String.Empty;
            }
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
            NotifyPropertyChanged(propertyName);
        }

        public object GetPropertyValue(string propertyName)
        {
            return _properties.First(p => p.Name.Equals(propertyName)).Value;
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    public class Property : IEquatable<Property>, IEqualityComparer<Property>
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public bool Equals(Property x, Property y)
        {
            if (x == null || y == null)
                return false;
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(Property obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(Property other)
        {
            if (other == null)
                return false;
            if (this.Name == other.Name)
                return true;
            return false;
        }
    }
}
