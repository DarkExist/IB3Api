﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Core.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image{ get; set; }
        public Guid CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
		public ICollection<User> Users { get; set; } = new List<User>();
	}
}
