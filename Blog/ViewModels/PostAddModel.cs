using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Blog.ViewModels
{
    public class PostAddModel
    {
        [Required(ErrorMessage = "Не указан заголовок")]
        public string PostTitle { get; set; }

        public IFormFile PostImage { get; set; }

        [Required(ErrorMessage = "Не указан текст поста")]
        public string PostText { get; set; }
    }
}