using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerStack.Serialization;

namespace ChatSample
{
    public class ChatMessageDecoder : IFrameDecoder<ChatMessage>
    {
        public async Task Decode(Stream input, List<ChatMessage> results)
        {
            var reader = new StreamReader(input);
            var line = await reader.ReadLineAsync();

            if (!string.IsNullOrEmpty(line))
            {
                results.Add(new ChatMessage
                {
                    Message = line
                });
            }
        }
    }

    public class ChatMessageEncoder : IFrameEncoder<ChatMessage>
    {
        public Task Encode(Stream output, ChatMessage value)
        {
            var buffer = Encoding.UTF8.GetBytes(value.Message);

            return output.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
