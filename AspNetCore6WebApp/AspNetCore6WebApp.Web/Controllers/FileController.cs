using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Hosting;

namespace AspNetCore6WebApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public string StoreDirName = "store";
        
        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        /// https://tool.oschina.net/commons
        private readonly static Dictionary<string, string> _contentTypes = new()
        {
            { ".avi", "video/avi" },
            { ".bmp", "image/bmp" },
            { ".csv", "text/csv" },
            { ".doc", "application/msword" },
            { ".docm", "application/vnd.ms-word.document.macroEnabled.12" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".dot", "application/msword" },
            { ".dotm", "application/vnd.ms-word.template.macroEnabled.12" },
            { ".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
            { ".gif", "image/gif" },
            { ".gz", "application/gzip" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".json", "application/json" },
            { ".mp4", "video/mpeg4" },
            { ".mpeg", "video/mpeg" },
            { ".mpg", "video/mpeg" },
            { ".odp", "application/vnd.oasis.opendocument.presentation" },
            { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
            { ".odt", "application/vnd.oasis.opendocument.text" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".pot", "application/vnd.ms-powerpoint" },
            { ".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
            { ".potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
            { ".ppa", "application/vnd.ms-powerpoint" },
            { ".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
            { ".pps", "application/vnd.ms-powerpoint" },
            { ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
            { ".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".rar", "application/vnd.rar" },
            { ".tar", "application/x-tar" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" },
            { ".txt", "text/plain" },
            { ".wav", "audio/wav" },
            { ".xla", "application/vnd.ms-excel" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlt", "application/vnd.ms-excel" },
            { ".xlam", "application/vnd.ms-excel.addin.macroEnabled.12" },
            { ".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
            { ".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
            { ".xltm", "application/vnd.ms-excel.template.macroEnabled.12" },
            { ".zip", "application/zip" },
            { ".7z", "application/x-7z-compressed" }
        };

        private static string GetContentType(string path)
        {
            string? ext = System.IO.Path.GetExtension(path)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ext) || _contentTypes.ContainsKey(ext) == false) return string.Empty;
            return _contentTypes[ext];
        }

        //private string GetHttpRequestQuery(string key)
        //{
        //    if (string.IsNullOrWhiteSpace(key)) return string.Empty;
        //    return Request.Query.ContainsKey(key) ? Request.Query[key] : string.Empty;
        //}

        /// Download file by HTTP Get Request https://localhost:44353/api/FileHandler/{filename}
        /// https://blog.johnwu.cc/article/ironman-day23-asp-net-core-upload-download-files.html
        /// https://www.c-sharpcorner.com/article/upload-download-files-in-asp-net-core-2-0/
        /// https://blog.miniasp.com/post/2019/12/08/Understanding-File-Providers-in-ASP-NET-Core-and-Download-Physical-File
        [HttpGet("{filename}")] // GET /api/FileHandler/{filename}
        public async Task<IActionResult> DownloadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return NotFound();
            //string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), this.StoreDirName, filename);
            string path = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, this.StoreDirName, filename);
            var ms = new System.IO.MemoryStream();
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                await fs.CopyToAsync(ms);
            }
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            /// https://stackoverflow.com/questions/45727856/how-to-download-a-file-in-asp-net-core
            return new Microsoft.AspNetCore.Mvc.FileStreamResult(ms, GetContentType(path))
            {
                FileDownloadName = filename
            };
        }

        /// https://blog.johnwu.cc/article/ironman-day23-asp-net-core-upload-download-files.html
        /// https://www.c-sharpcorner.com/article/upload-download-files-in-asp-net-core-2-0/
        /// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-6.0
        [HttpPost]
        //public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        //public async Task<IActionResult> UploadFiles(ICollection<IFormFile> files)
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            if ((files?.Count ?? 0) < 1) files = Request.Form.Files;
            if (files == null) return NotFound();
            long size = files.Sum(f => f.Length);
            //string folder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), this.StoreDirName);
            string folder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, this.StoreDirName);
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string path = System.IO.Path.Combine(folder, System.IO.Path.GetFileName(file.FileName));
                    using var fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                    await file.CopyToAsync(fs);
                }
            }
            return Ok(new { count = files.Count, size });
        }

        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    if (file == null) file = Request.Form.Files["FileUploader1"];
        //    if (file == null) file = Request.Form.Files["FileUploader2"];
        //    if (file == null) return NotFound();
        //    string folder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, this.StoreDirName);
        //    if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
        //    string path = System.IO.Path.Combine(folder, file.FileName);
        //    using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
        //    {
        //        await file.CopyToAsync(fs);
        //    }
        //    return Ok(new { count = 1, size = file.Length });
        //}
    }
}
