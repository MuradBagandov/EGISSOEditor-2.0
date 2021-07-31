using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Infrastuctures.Enums
{
    public class ValueDescription
    {
        public Enum Value { get; private set; }

        public string Description { get; private set; }

        public ValueDescription(Enum value, string description)
        {
            Value = value;
            Description = description;
        }
    }
}
