using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApiii.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public class APIResponse
        {
            public int ResponseCode { get; set; }
            public string Result { get; set; }
            public string ErrorMessage { get; set; }
        }

        private readonly IWebHostEnvironment environment;

        public string Filepath { get; private set; }

        public ImageController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile form, string productcode)
        {
            APIResponse response = new();
            try
            {
                string filepath = GetFilePath(productcode);
                if (!Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }

                string imagepath = Path.Combine(filepath, productcode + ".png");
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }

                using FileStream stream = System.IO.File.Create(imagepath);
                await form.CopyToAsync(stream);
                response.ResponseCode = 200;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }


        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new();
            int passcount = 0;
            int errorcount = 0;

            try
            {
                string filepath = GetFilePath(productcode);
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                foreach (var file in filecollection)
                {
                    string imagepath = filepath + "\\" + file.FileName;

                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }

                    await using FileStream stream = System.IO.File.Create(imagepath);
                    await file.CopyToAsync(stream);
                    passcount++;

                }

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                errorcount++;
                response.ResponseCode = 500; // Hata durumunu ifade eden bir HTTP kodu
                response.ErrorMessage = ex.Message;
            }

            response.Result = passcount + " Files uploaded & " + errorcount + " files failed";
            return Ok(response);
        }


        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string Imageurl = string.Empty;
           
            string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string filepath = GetFilePath(productcode);
                string imagepath = filepath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {

            }
            return Ok(Imageurl);


        }



        [HttpGet("ConvertToBase64")]
        public IActionResult ConvertToBase64(string productcode)
        {
            string filepath = GetFilePath(productcode);
            string[] files = Directory.GetFiles(filepath);
            List<string> base64Strings = new List<string>();

            foreach (var file in files)
            {
                string imageBase64 = ToBase64(file); 
                base64Strings.Add(imageBase64);
            }

            return Ok(base64Strings);
        }

        private string ToBase64(string filePath)
        {
            try
            {
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        byte[] imageBytes = ms.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file '{filePath}' to Base64: {ex.Message}");
                return null;
            }
        }











        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productcode)
        {
            List<string> Imageurl = new();

            string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string Filepath = GetFilePath(productcode);
               
                if(System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))

                        {
                            string _Imageurl = hosturl + "/Upload/product/" + productcode + "/" + filename;
                            Imageurl.Add(_Imageurl);
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {

            }
            return Ok(Imageurl);


        }




        [HttpGet("Download")]
        public async Task<IActionResult> Download(string productcode)
        {
            //string Imageurl = string.Empty;

            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string filepath = GetFilePath(productcode);
                string imagepath = filepath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new();
                    using(FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }

                    stream.Position = 0;
                    return File(stream, "image/png", productcode + "png");
                    //Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";

                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
          

        }




        [HttpGet("Remove")]
        public async Task<IActionResult> Remove(string productcode)
        {
            //string Imageurl = string.Empty;

            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string filepath = GetFilePath(productcode);
                string imagepath = filepath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");

                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

         

        [HttpGet("MultiRemove")]
        public async Task<IActionResult> MultiRemove(string productcode)
        {
            //string Imageurl = string.Empty;

            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string Filepath = GetFilePath(productcode);


                if(System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [NonAction]
        private string GetFilePath(string productcode)
        {
            return Path.Combine(environment.WebRootPath, "Upload", "product", productcode);
        }




    }
}
