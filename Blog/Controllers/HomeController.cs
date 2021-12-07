using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blog.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;


namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private BlogContext db;

        public HomeController(BlogContext context)
        {
            db = context;
        }

        //[Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var PostList = db.Post.Include(i => i.Images).ToList();
            PostList = GetPostsUploadImage(PostList);

            return View(PostList);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index()
        {
            var iFile = @"C:\Users\HPPavalion\Source\Repos\Blog\Blog\wwwroot\img\intro-pic.jpg";
            byte[] imageData = null;
            FileInfo fInfo = new FileInfo(iFile);
            long numBytes = fInfo.Length;
            FileStream fStream = new FileStream(iFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);
            imageData = br.ReadBytes((int)numBytes);
            // получение расширения файла изображения не забыв удалить точку перед расширением
            string iImageExtension = (Path.GetExtension(iFile)).Replace(".", "").ToLower();

            db.Images.Add(new Images
            {
                PostId = 1,
                FileName = "intro - pic.jpg",
                Title = "test",
                ImageData = imageData
            });
            await db.SaveChangesAsync();

            return View(await db.Post.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UnAuthenticated()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");

            return Ok("success");
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }

            return buffer;
        }

        public List<Post> GetPostsUploadImage (List<Post> PostList)
        {
            var fileName = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\upload\img\";
            for (int i = 0; i < PostList.Count; i++)
            {
                for (int j = 0; j < PostList[i].Images.Count; j++)
                {
                    PostList[i].Images[j].ImagePath = fileName + PostList[i].Images[j].FileName;
                    if (!System.IO.File.Exists(PostList[i].Images[j].ImagePath) && PostList[i].Images[j].ImageData != null)
                    {
                        System.IO.File.WriteAllBytes(fileName, PostList[i].Images[j].ImageData);
                        System.Diagnostics.Process.Start(fileName);
                    }
                }
            }

            return PostList;
        }
    }
}
