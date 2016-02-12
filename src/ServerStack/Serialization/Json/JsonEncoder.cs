using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ServerStack.Serialization.Json
{
    public class JsonEncoder : IFrameEncoder<JObject>
    {
        public Task Encode(Stream output, JObject value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.ToString());

            return output.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
