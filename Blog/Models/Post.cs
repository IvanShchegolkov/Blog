using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Title { get; set; }
        public byte[] Img { get; set; }
        public List<Images> Images { get; set; }
        public string PostText { get; set; }
    }

    public class Images
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public byte[] ImageData { get; set; }
        public string ImagePath { get; set; }
    }
}
