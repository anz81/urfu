using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Common
{
    public static class GuidHelper
    {
        /// <summary>
        /// Генерирует ключ заданной длины без -
        /// </summary>
        /// <param name="length">Длина ключа по умолчанию 32</param>
        /// <returns></returns>
        public static string GetGuid(int length = 32)
        {
            string guid = "";
            while (guid.Length < length)
            {
                guid += Guid.NewGuid().ToString().Replace("-", "");
            }
            guid = guid.Substring(0, length);
            return guid;
        }
    }
}
