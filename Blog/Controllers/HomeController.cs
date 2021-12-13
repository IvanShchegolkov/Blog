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
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Media;
//using Microsoft.AspNet.Identity;

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

        [Authorize]
        public IActionResult Addpost()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addpost(PostAddModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PostTitle != null && model.PostText != null)
                {
                    var user = db.Person.Where(u => u.Login == User.Identity.Name).FirstOrDefault();
                    Post post = new Post();
                    post.PostText = model.PostText;
                    post.Title = model.PostTitle;
                    post.PersonId = user.Id;

                    db.Post.Add(post);
                    await db.SaveChangesAsync();

                    if (model.PostImage != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(model.PostImage.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.PostImage.Length);
                        }
                        Images images = new Images();
                        images.ImageData = imageData;
                        images.PostId = post.Id;
                        images.FileName = model.PostImage.FileName;
                        images.Title = model.PostImage.FileName;

                        db.Images.Add(images);
                        await db.SaveChangesAsync();
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные данные");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UnAuthenticated()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");

            var jsonString = JsonConvert.SerializeObject("success");

            return Ok(jsonString);
        }

        private static byte[] ReadFile(string filePath)
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
                if (PostList[i].Images.Count > 0)
                {
                    for (int j = 0; j < PostList[i].Images.Count; j++)
                    {
                        if (PostList[i].Images[j].FileName != null || PostList[i].Images[j].FileName != "")
                        {
                            PostList[i].Images[j].ImagePath = fileName + PostList[i].Images[j].FileName;
                            if (!System.IO.File.Exists(PostList[i].Images[j].ImagePath) && PostList[i].Images[j].ImageData != null)
                            {
                                System.IO.File.WriteAllBytes(PostList[i].Images[j].ImagePath, PostList[i].Images[j].ImageData);
                                //System.Diagnostics.Process.Start(fileName);
                            }
                        }
                    }
                }
            }

            return PostList;
        }

        private System.Drawing.Image CropImage(System.Drawing.Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            System.Drawing.Image destinationImage = new Bitmap(destinationWidth, destinationHeight);

            using (Graphics g = Graphics.FromImage(destinationImage))
                g.DrawImage(
                  sourceImage,
                  new Rectangle(0, 0, destinationWidth, destinationHeight),
                  new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                  GraphicsUnit.Pixel
                );

            return destinationImage;
        }

        public System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            return (System.Drawing.Image)(new Bitmap(imgToResize, size));
        }
        public void MainTest()
        {
            string path = "C:\\Images\\img1.jpg";
            System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            Bitmap imgbitmap = new Bitmap(img);
            System.Drawing.Image resizedImage = resizeImage(imgbitmap, new Size(200, 200));
        }
    }
}
