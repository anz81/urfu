using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.OHOPModels;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Processors
{
    public class FilesProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;
        private readonly ApplicationDbContext _db;

        public FilesProcessor(IVersionedDocumentInspector inspector, ApplicationDbContext db)
        {
            _inspector = inspector;
            _db = db;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = data as JArray;

            try
            {
                items = JArray.Parse(
                            JsonConvert.SerializeObject(
                                    items.Select(item => new FileStorageInfo()
                                    {
                                        FileId = item.Value<int>(nameof(FileStorageInfo.FileId)),
                                        FileName = item.Value<string>(nameof(FileStorageInfo.FileName))
                                    })
                            )
                        );
            }
            catch { }

            return items;
        }
    }
}