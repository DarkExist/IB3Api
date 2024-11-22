using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Core.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public User UserRef { get; set; }
    }
}
