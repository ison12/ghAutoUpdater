using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Utils
{
    /// <summary>
    /// Jsonユーティリティ。
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// Jsonメッセージをオブジェクトへデシリアライズします。
        /// </summary>
        public static T Deserialize<T>(string message)
        {
            using (var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(message)))
            {
                //var setting = new DataContractJsonSerializerSettings()
                //{
                //    UseSimpleDictionaryFormat = true,
                //};
                var serializer = new DataContractJsonSerializer(typeof(T)/*, setting*/);
                return (T)serializer.ReadObject(stream);
            }
        }

    }
}
