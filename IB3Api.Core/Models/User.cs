using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

		public ICollection<Post> Posts { get; set; } = new List<Post>();
		public ICollection<Role> Roles { get; set; } = new List<Role>();
        public Profile ProfileRef { get; set; }
	}
}
