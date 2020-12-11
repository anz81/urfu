using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Urfu.Its.Common;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.DataContext
{
    /// <summary>
    /// Категории файлов. Предположительно будут файлы для ОХОП и Практик.
    /// Нужно для разделения файлов по папкам.
    /// </summary>
    public enum FileCategory
    {
        [Display(Name = "OHOP")]
        OHOP,

        [Display(Name = "Practice")]
        Practice,

        [Display(Name = "PracticeCompanies")]
        PracticeCompanies

        //Обязательно указывать Display Name !!!
    }
    
    public static class FileStorageHelper
    {
        private readonly static ApplicationDbContext db = new ApplicationDbContext();
        private readonly static string root = ConfigurationManager.AppSettings["FileFolder"];
        /// <summary>
        /// Сохраняет файл на диск и делает запись в БД. 
        /// Путь сохранения файла: {FileFolder из config(root)}\{category}\{folder}\{fileName for storage}
        /// </summary>
        /// <param name="file">Содержимое файла</param>
        /// <param name="category">Категория файла</param>
        /// <param name="folder">Папка для отдельного расположения файла внутри директории для категории</param>
        /// <param name="comment">Комментарий</param>
        /// <param name="id">id записи в таблице FileStorage в случае, если файл надо перезаписать. Запись и файл удаляются и создаются новые</param>
        /// <returns>id записи в таблице FileStorage</returns>
        public static int? SaveFile(IFormFile file, FileCategory category, string folder = null, string comment = null, int? id = null)
        {
            var user = new HttpContextUserNameProvider().ToString();           
            var categoryFolder = EnumHelper<FileCategory>.GetDisplayValue(category);

           try
            { 
                folder = string.IsNullOrWhiteSpace(folder) ? $"{categoryFolder}" : $"{categoryFolder}\\{folder}";

                // проверяем наличие директории. Создаем ее, если нужно
                var folderPath = $"{root}\\{folder}";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                
                var nameForStorage = $"{GuidHelper.GetGuid()}{file.FileName}"; // название файла для хранения. Не берем название от пользователя из-за возможности возникновения дублей
                var  relativePath = $"{folder}\\{nameForStorage}";
                var fullPath = GetFilePath(relativePath);
                Stream stream = file.OpenReadStream();
                if (stream.Length != 0)
                    using (FileStream fileStream = File.Create(fullPath, (int)stream.Length))
                    {
                        // Размещает массив общим размером равным размеру потока
                        // Могут быть трудности с выделением памяти для больших объемов
                        byte[] data = new byte[stream.Length];

                        stream.Read(data, 0, (int)data.Length);
                        fileStream.Write(data, 0, data.Length);
                    }
//                file.SaveAs(fullPath);

                if (id.HasValue) 
                    RemoveFile(id.Value);

                var newData = new FileStorage()
                {
                    HttpUser = user,
                    Ip = new HttpContextIpProvider().ToString(),
                    FileNameForUser = file.FileName,
                    FileName = nameForStorage,
                    Path = relativePath, // храним относительную ссылку
                    Comment = comment,
                    Date = DateTime.Now                  
                };

                db.FileStorage.Add(newData);
                db.SaveChanges();

                Logger.Info($"Сохранен файл {fullPath}. FileStorageId = {newData.Id}");
                return newData.Id;
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при сохранении файла {file.FileName}, категория файла {categoryFolder}");
                Logger.Error(ex);
                return null;
               
            }
        }

        public static int? SaveFile(MemoryStream file, string filename, FileCategory category, string folder = null, string comment = null, int? id = null)
        {
            var user = new HttpContextUserNameProvider().ToString();
            var categoryFolder = EnumHelper<FileCategory>.GetDisplayValue(category);

            try
            {
                folder = string.IsNullOrWhiteSpace(folder) ? $"{categoryFolder}" : $"{categoryFolder}\\{folder}";

                // проверяем наличие директории. Создаем ее, если нужно
                var folderPath = $"{root}\\{folder}";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var nameForStorage = $"{GuidHelper.GetGuid()} {filename}"; // название файла для хранения. Не берем название от пользователя из-за возможности возникновения дублей
                var relativePath = $"{folder}\\{nameForStorage}";
                var fullPath = GetFilePath(relativePath);
                Stream stream = file;
                if (stream.Length != 0)
                    using (FileStream fileStream = File.Create(fullPath, (int)stream.Length))
                    {
                        // Размещает массив общим размером равным размеру потока
                        // Могут быть трудности с выделением памяти для больших объемов
                        byte[] data = new byte[stream.Length];

                        stream.Read(data, 0, (int)data.Length);
                        fileStream.Write(data, 0, data.Length);
                    }
                //                file.SaveAs(fullPath);

                if (id.HasValue)
                    RemoveFile(id.Value);

                var newData = new FileStorage()
                {
                    HttpUser = user,
                    Ip = new HttpContextIpProvider().ToString(),
                    FileNameForUser = filename,
                    FileName = nameForStorage,
                    Path = relativePath, // храним относительную ссылку
                    Comment = comment,
                    Date = DateTime.Now
                };

                db.FileStorage.Add(newData);
                db.SaveChanges();

                Logger.Info($"Сохранен файл {fullPath}. FileStorageId = {newData.Id}");
                return newData.Id;
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при сохранении файла {filename}, категория файла {categoryFolder}");
                Logger.Error(ex);
                return null;

            }
        }

        /// <summary>
        /// Удаляет файл с диска и запись из БД
        /// </summary>
        /// <param name="id">id записи в таблице FileStorage</param>
        /// <returns>true - файл и запись удалены успешно. false - ошибка при удалении, смотреть в лог</returns>
        public static bool RemoveFile(int id)
        {
            try
            {
                var file = db.FileStorage.FirstOrDefault(f => f.Id == id);

                if (file == null)
                {
                    Logger.Info($"Ошибка при удалении файла FileStorageId = {id}. Запись не найдена.");
                    return false;
                }

                var path = GetFilePath(file.Path);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                    Logger.Info($"Удален файл {path}. FileStorageId = {id}");

                    db.FileStorage.Remove(file);                    
                    db.SaveChanges();

                    return true;
                }

                Logger.Info($"Ошибка при удалении файла. FileStorageId = {id}. Файл не найден {path}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при удалении файла FileStorageId = {id}.");
                Logger.Error(ex);

                return false;
            }

        }

        public static byte[] GetBytes(int id)
        {
            var data = db.FileStorage.FirstOrDefault(s => s.Id == id);
            var fullPath = GetFilePath(data?.Path);

            var bytes = System.IO.File.ReadAllBytes(fullPath);
            return bytes;
        }

        public static Stream GetStream(string relativePath)
        {
            var fullPath = GetFilePath(relativePath);

            var input = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return input;
        }

        public static string GetFilePath(string relativePath)
        {
            return $"{root}\\{relativePath}";
        }

        /// <summary>
        /// Добавление файлов в архив из хранилища.
        /// </summary>
        /// <param name="zipArchive">Архив, в который добавляются файлы из хранилища</param>
        /// <param name="fileStorageIds">Список id файлов из таблицы FileStorage</param>
        /// <param name="folder">Название папки, если нужно файлы складывать в нее. Иначе просто в архив.</param>
        public static void AddFolderToZipArchive(ZipArchive zipArchive, IEnumerable<int> fileStorageIds, string folder = null)
        {
            foreach (var fileId in fileStorageIds)
            {
                var file = db.FileStorage.FirstOrDefault(f => f.Id == fileId);
                if (file == null)
                    continue;

                var entryName = string.IsNullOrWhiteSpace(folder) ? file.FileNameForUser : $"{folder}/{file.FileNameForUser}";
                var fileEntry = zipArchive.CreateEntry(entryName, CompressionLevel.Fastest);

                using (var entryStream = fileEntry.Open())
                {
                    using (var stream = File.Open(GetFilePath(file.Path), FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        stream.CopyTo(entryStream);
                    }
                }
            }
        }
    }
}