using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Permissions;
using System.Net;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.ConstrainedExecution;
using Urfu.Its.Integration;
using System.Configuration;
using Urfu.Its.Common;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.Admin)]
    public class FilesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly string _path = ConfigurationManager.AppSettings["FileFolder"];

        public ActionResult Index(string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var files = Directory.GetFiles(_path).Select(f => new
                {
                    fullPath = f,
                    fileName = f.Split('\\').Last()
                });

                return Json(
                    new
                    {
                        data = files,
                        total = files.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                return View();
            }
        }

        public ActionResult Download(string path, string fileName)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var st = file.OpenReadStream();
                if (st.Length != 0)
                    using (FileStream fileStream = System.IO.File.Create($"{_path}\\{System.IO.Path.GetFileName(file.FileName)}", (int)st.Length))
                    {
                        // Размещает массив общим размером равным размеру потока
                        // Могут быть трудности с выделением памяти для больших объемов
                        byte[] data = new byte[st.Length];

                        st.Read(data, 0, (int)data.Length);
                        fileStream.Write(data, 0, data.Length);
                    }
            }
            catch (Exception ex)
            {
                Logger.Info("Ошибка при загрузке файла");
                Logger.Error(ex);
            }
            return Json(new { success = true });//, "text/html", Encoding.Unicode);
        }

        public ActionResult Delete(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Ошибка при удалении файла");
                Logger.Error(ex);
            }
            return Json(new { success = true });//, "text/html", Encoding.Unicode);
        }
    }
}