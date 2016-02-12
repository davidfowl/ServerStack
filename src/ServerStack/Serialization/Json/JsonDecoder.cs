using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerStack.Serialization.Json
{
    public class JsonDecoder : IFrameDecoder<JObject>
    {
        public Task<JObject> Decode(Stream input)
        {
            var reader = new JsonTextReader(new StreamReader(input));

            return Task.FromResult(JObject.Load(reader));
        }
    }
}
