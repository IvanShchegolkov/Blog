using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int PostPersonId { get; set; }
        public string PostTitle { get; set; }
        public string PostImg { get; set; }
        public string PostText { get; set; }
    }
}
