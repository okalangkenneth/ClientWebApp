using MyNamespace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWebApp.Extensions
{
    public static class StreamExtensions
    {
        public static T ReadAndDeserializeFromJson<T>(this Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new NotSupportedException(nameof(stream));


            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static async Task WriteAndSerializeToJsonAsync<T>(this Stream stream, T objectToWrite) 
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new NotSupportedException(nameof(stream));

            using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(), 1024, true))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonTextWriter, objectToWrite);
                    await jsonTextWriter.FlushAsync();
                }
            }

        }
    }
}
