using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Models.Claims
{
    public enum ClaimType { Name, Role, Expires};
    public class Claim
    {
        public ClaimType Type { get; private set; }
        public string Value { get; private set; }
 

        public Claim(ClaimType type, string value)
        {
            Type = type;
            Value = value;
        }

		public Claim(ClaimType type, List<string> values)
		{
			Type = type;
			Value = values;
		}

		public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }

}
