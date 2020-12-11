using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Core
{
    public static class BlockDataHelper
    {
        public const string ContentProperty = "Content";

        public static IEnumerable<TLink> GetActualMergedData<TSource, TLink, TKey>(JToken sourceItems, JToken linkItems, 
            Expression<Func<TSource, TKey>> sourceIdSelector,
            Expression<Func<TLink, TKey>> linkIdSelector) where TLink : new()
        {
            if(sourceItems == null || sourceItems.Type == JTokenType.Null)
                throw new ArgumentException($"Параметр '{nameof(sourceItems)}' не должен быть null-ом");
            if (linkItems == null || linkItems.Type == JTokenType.Null)
                throw new ArgumentException($"Параметр '{nameof(linkItems)}' не должен быть null-ом");

            if (!(sourceItems is JArray))
                throw new ArgumentException($"Параметр '{nameof(sourceItems)}' должен быть типа JArray");
            if (!(linkItems is JArray))
                throw new ArgumentException($"Параметр '{nameof(linkItems)}' должен быть типа JArray");

            var sourceIdProp = ReflectionExtensions.GetPropertyInfo(sourceIdSelector);
            var sourceItemIdKey = sourceIdProp.Name;
            var linkIdProp = ReflectionExtensions.GetPropertyInfo(linkIdSelector);
            var linkIdKey = linkIdProp.Name;
            var actualSourceIds = sourceItems.Select(fdp => fdp[sourceItemIdKey].Value<TKey>());
            var oldSourceIds = linkItems.Select(item => item[linkIdKey].Value<TKey>());
            var existingLinks = linkItems
                .Where(item => actualSourceIds.Contains(item[linkIdKey].Value<TKey>()))
                .Select(item => (TLink) JsonConvert.DeserializeObject(item.ToString(), typeof(TLink))).ToList();
            var newSourceItems = sourceItems.Where(fdp => !oldSourceIds.Contains(fdp[sourceItemIdKey].Value<TKey>())).ToList();

            var newDisciplineScopes = newSourceItems.Select(f =>
            {
                var newItem = new TLink();
                var key = f[sourceItemIdKey].Value<TKey>();
                linkIdProp.SetValue(newItem, key);
                return newItem;
            });

            var actualLinks = existingLinks.Concat(newDisciplineScopes).ToList();

            foreach (var sourceItem in sourceItems)
            {
                var fdpId = sourceItem[sourceItemIdKey].Value<TKey>();
                var ds = actualLinks.FirstOrDefault(s => Equals(linkIdProp.GetValue(s), fdpId));
                yield return ds;
            }
        }

        public static IEnumerable<TLink> GetActualMergedData<TSource, TLink, TKey>(TSource[] sourceItems, TLink[] linkItems,
            Expression<Func<TSource, TKey>> sourceIdSelector,
            Expression<Func<TLink, TKey>> linkIdSelector) where TLink : new()
        {
            if (sourceItems == null)
                throw new ArgumentException($"Параметр '{nameof(sourceItems)}' не должен быть null-ом");
            if (linkItems == null)
                throw new ArgumentException($"Параметр '{nameof(linkItems)}' не должен быть null-ом");

            //var sourceIdProp = ReflectionExtensions.GetPropertyInfo(sourceIdSelector);
            //var sourceItemIdKey = sourceIdProp.Name;
            var linkIdProp = ReflectionExtensions.GetPropertyInfo(linkIdSelector);
            //var linkIdKey = linkIdProp.Name;

            var getSourceId = sourceIdSelector.Compile();
            var getLinkId = linkIdSelector.Compile();

            var actualSourceIds = sourceItems.Select(source => getSourceId(source));
            var oldSourceIds = linkItems.Select(link => getLinkId(link));
            var existingLinks = linkItems.Where(link => actualSourceIds.Contains(getLinkId(link))).ToList();
            var newSourceItems = sourceItems.Where(source => !oldSourceIds.Contains(getSourceId(source))).ToList();

            var newDisciplineScopes = newSourceItems.Select(f =>
            {
                var newItem = new TLink();
                var key = getSourceId(f);
                linkIdProp.SetValue(newItem, key);
                return newItem;
            });

            var actualLinks = existingLinks.Concat(newDisciplineScopes).ToList();

            foreach (var sourceItem in sourceItems)
            {
                var fdpId = getSourceId(sourceItem);
                var ds = actualLinks.FirstOrDefault(s => Equals(getLinkId(s), fdpId));
                yield return ds;
            }
        }

        public static string PrepareData(JToken content)
        {
            return new JObject(new JProperty(ContentProperty, content)).ToString();
        }

        public static string PrepareData(object contentObject)
        {
            var contentValue = contentObject == null ? null : JToken.FromObject(contentObject);
            return new JObject(new JProperty(ContentProperty, contentValue)).ToString();
        }

        public static JToken GetContent(string blockData)
        {
            return JObject.Parse(blockData).First.First;
        }

        public static JToken GetContent(this VersionedDocumentBlock block)
        {
            return JObject.Parse(block.Data).First.First;
        }

        public static IEnumerable<VersionedDocumentBlock> GetIndependentBlocks(ApplicationDbContext db,
            VersionedDocument doc)
        {
            var possibleIndependentBlocks = new List<VersionedDocumentBlock>();
            foreach (var link in doc.BlockLinks.ToList())
            {
                var b = link.DocumentBlock;
                do
                {
                    if (b.Links.All(l => l.DocumentId == doc.Id))
                    {
                        var buf = b;
                        b = b.PreviousBlock;
                        possibleIndependentBlocks.Add(buf);
                    }
                    else
                        b = null;
                } while (b != null);
            }
            var possibleIndependentBlockIds = possibleIndependentBlocks.Select(b => b.Id).ToList();
            var independentBlocks = new List<VersionedDocumentBlock>();
            foreach (var block in possibleIndependentBlocks)
            {
                var blockId = block.Id;
                if (!db.VersionedDocumentBlocks.Any(b =>
                    b.PreviousBlockId == blockId && !possibleIndependentBlockIds.Contains(b.Id)))
                {
                    independentBlocks.Add(block);
                }
            }

            return independentBlocks;
        }
    }
}