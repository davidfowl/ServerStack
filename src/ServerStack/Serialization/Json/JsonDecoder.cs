using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerStack.Serialization.Json
{
    public class JsonDecoder<T> : IFrameDecoder<T>
    {
        public Task Decode(Stream input, List<T> results)
        {
            var reader = new JsonTextReader(new StreamReader(input));

            results.Add(JObject.Load(reader).ToObject<T>());
            return Task.FromResult(0);
        }
    }

    public class JsonDecoder : IFrameDecoder<JObject>
    {
        public Task Decode(Stream input, List<JObject> results)
        {
            var reader = new JsonTextReader(new StreamReader(input));

            results.Add(JObject.Load(reader));
            return Task.FromResult(0);
        }
    }
}
