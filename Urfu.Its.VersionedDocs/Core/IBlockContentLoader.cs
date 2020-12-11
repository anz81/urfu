using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Urfu.Its.VersionedDocs.Core
{
    /// <summary>
    /// Загрузчик данных блока. Может использоваться для документов с актуальной структурой данных, соответсвующей текущей структуре в системе
    /// </summary>
    public interface IBlockContentLoader
    {
        JToken LoadContent(JToken blockContent);
        bool IsLoadRequired(JToken blockContent);
    }

    public interface IBlockContentProcessor
    {
        JToken ProcessContent(JToken data);
    }

    public abstract class ObjectBlockContentLoader<T> : IBlockContentLoader
    {
        protected abstract T LoadAnyContent(JToken blockContent);

        public JToken LoadContent(JToken blockContent)
        {
            object content = LoadAnyContent(blockContent);
            JToken jToken;
            switch (content)
            {
                case null:
                    jToken = null;
                    break;
                case string str:
                    jToken = new JValue(str);
                    break;
                case JObject jObject:
                    jToken = jObject;
                    break;                
                case IEnumerable enumerable:
                    var evaluated = enumerable.Cast<object>().ToList();
                    jToken = JArray.FromObject(evaluated);
                    break;
                default:
                    jToken = JObject.FromObject(content);
                    break;
            }
            return jToken;
        }

        [Obsolete("Не видно смысла в этой функции. Будет удалена. Не использовать!")]
        public virtual bool IsLoadRequired(JToken blockContent)
        {
            return true;
            /*if (blockContent is JObject objectContent)
            {
                return objectContent.Properties()
                    .Select(p => p.Value)
                    .OfType<JValue>()
                    .All(p => p.Value == null);
            }
            else if (blockContent is JArray arrayContent)
            {
                
            }*/
        }
    }
}