using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Processors
{
    public class ApprovalActProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;
        private readonly ApplicationDbContext _db;

        public ApprovalActProcessor(IVersionedDocumentInspector inspector, ApplicationDbContext db)
        {
            _inspector = inspector;
            _db = db;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = data as JArray;

            for(int i = 0; i <items.Count; i++)
            {
                var listObj = items[i].Value<object>();
                var list = JsonConvert.DeserializeObject<ApprovalAct>(listObj.ToString());
                items[i] = JToken.Parse(JsonConvert.SerializeObject(list));
            }

            return items;
        }
    }
}