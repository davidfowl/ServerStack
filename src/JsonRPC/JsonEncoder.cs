using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerStack.Protocols;

namespace JsonRPC
{

    public class JsonDecoder : IStreamDecoder<JObject>
    {
        public Task<bool> TryDecode(Stream input, out JObject frame)
        {
            var reader = new JsonTextReader(new StreamReader(input));

            frame = JObject.Load(reader);

            return Task.FromResult(true);
        }
    }

    public class JsonEncoder : IStreamEncoder<JObject>
    {
        public Task Encode(Stream output, JObject value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.ToString());

            return output.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
