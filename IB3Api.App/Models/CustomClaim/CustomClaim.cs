using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Models.CustomClaim
{
	public class CustomClaim
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public CustomClaim(string name, string value) 
		{
			Name = name;
			Value = value;
		}
	}
}
