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
            var PostList = db.Post.ToList();
            for(int i = 0; i < PostList.Count; i++)
            {
                PostList[i].Images = db.Images.Where(p => p.PostId == PostList[i].Id).ToList();
            }


            return View(PostList);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index()
        {
            //var iFile = @"C:\Users\User\Desktop\sql\c#\Blog-master\Blog\wwwroot\img\intro-pic.jpg";
            //byte[] imageData = null;
            //FileInfo fInfo = new FileInfo(iFile);
            //long numBytes = fInfo.Length;
            //FileStream fStream = new FileStream(iFile, FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fStream);
            //imageData = br.ReadBytes((int)numBytes);
            //// получение расширения файла изображения не забыв удалить точку перед расширением
            //string iImageExtension = (Path.GetExtension(iFile)).Replace(".", "").ToLower();

            //db.Images.Add(new Images
            //{
            //    PostId = 1,
            //    FileName = "intro - pic.jpg",
            //    Title = "test",
            //    ImageData = imageData
            //});
            //await db.SaveChangesAsync();

            return View(await db.Post.ToListAsync());
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


        //private static void GetImageBinaryFromDb()
        //{
        //    // получаем данные их БД
        //    List<byte[]> iScreen = new List<byte[]>(); // сделав запрос к БД мы получим множество строк в ответе, поэтому мы их сможем загнать в массив/List
        //    List<string> iScreen_format = new List<string>();
        //    using (SqlConnection sqlConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=dbtest; User Id=sa; Password=pass"))
        //    {
        //        sqlConnection.Open();
        //        SqlCommand sqlCommand = new SqlCommand();
        //        sqlCommand.Connection = sqlConnection;
        //        sqlCommand.CommandText = @"SELECT [screen], [screen_format] FROM [report] WHERE [id] = 1"; // наша запись в БД под id=1, поэтому в запросе "WHERE [id] = 1"
        //        SqlDataReader sqlReader = sqlCommand.ExecuteReader();
        //        byte[] iTrimByte = null;
        //        string iTrimText = null;
        //        while (sqlReader.Read()) // считываем и вносим в лист результаты
        //        {
        //            iTrimByte = (byte[])sqlReader["screen"]; // читаем строки с изображениями
        //            iScreen.Add(iTrimByte);
        //            iTrimText = sqlReader["screen_format"].ToString().TrimStart().TrimEnd(); // читаем строки с форматом изображения
        //            iScreen_format.Add(iTrimText);
        //        }
        //        sqlConnection.Close();
        //    }
        //    // конвертируем бинарные данные в изображение
        //    byte[] imageData = iScreen[0]; // возвращает массив байт из БД. Так как у нас SQL вернёт одну запись и в ней хранится нужное нам изображение, то из листа берём единственное значение с индексом '0'
        //    MemoryStream ms = new MemoryStream(imageData);
        //    Image newImage = Image.FromStream(ms);

        //    // сохраняем изоражение на диск
        //    string iImageExtension = iScreen_format[0]; // получаем расширение текущего изображения хранящееся в БД
        //    string iImageName = @"C:\result_new" + "." + iImageExtension; // задаём путь сохранения и имя нового изображения
        //    if (iImageExtension == "png") { newImage.Save(iImageName, System.Drawing.Imaging.ImageFormat.Png); }
        //    else if (iImageExtension == "jpg" || iImageExtension == "jpeg") { newImage.Save(iImageName, System.Drawing.Imaging.ImageFormat.Jpeg); }
        //    else if (iImageExtension == "gif") { newImage.Save(iImageName, System.Drawing.Imaging.ImageFormat.Gif); }
        //    // и т.д., можно все if заменить на одну строку "newImage.Save(iImageName)", насколько это правильно сказать не могу, но работает
        //}
    }
}
