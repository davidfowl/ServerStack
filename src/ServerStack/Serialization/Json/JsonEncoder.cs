using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerStack.Serialization.Json
{
    public class JsonEncoder<T> : IFrameEncoder<T>
    {
        public Task Encode(Stream output, T value)
        {
            var serializer = new JsonSerializer();

            using (var writer = new StreamWriter(output) { AutoFlush = true })
            {
                serializer.Serialize(writer, value);
            }

            return Task.FromResult(0);
        }
    }

    public class JsonEncoder : IFrameEncoder<JObject>
    {
        public virtual Task Encode(Stream output, JObject value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.ToString());

            return output.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
