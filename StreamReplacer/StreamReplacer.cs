using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StreamReplaced
{
    public class StreamReplacer : Stream
    {
        private Stream BaseStream;
        
        private List<byte> Buffer = null;
        private int BufLen = 0;

        private Dictionary<byte[], byte[]> KeyValuePairs = new Dictionary<byte[], byte[]>();

        public StreamReplacer(Stream baseStream)
        {
            this.BaseStream = baseStream ?? throw new ArgumentNullException("baseStream", "Null streams are not allowed.");
        }

        public void Replace(byte[] from, byte[] to)
        {
            if (Buffer != null) throw new Exception("You cannot define a replacement after reading begins.");

            KeyValuePairs.Add(from, to);
            var l = from.Length;
            if (l > BufLen) BufLen = l;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Buffer == null) Buffer = new List<byte>(BufLen);
            int len = Math.Max(0, count + BufLen - Buffer.Count);
            byte[] temp = new byte[len];
            len = BaseStream.Read(temp, 0, temp.Length);
            for(int i = 0; i< len; i++) Buffer.Add(temp[i]);
            temp = Buffer.ToArray();

            foreach (var kv in KeyValuePairs) {
                temp = ByteReplace(temp, kv.Key, kv.Value, 0, temp.Length);
            }
            Buffer = new List<byte>(temp);

            int returnLen = Math.Min(temp.Length, count);
            for(int i=0; i< returnLen; i++) buffer[i+offset] = temp[i];
            Buffer.RemoveRange(0, returnLen);

            return returnLen; 
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            BaseStream.Dispose();
        }

        public static byte[] ByteReplace(byte[] content, byte[] from, byte[] to, int start, int end)
        {
            if (from.Length == 0) return content;

            var result = new List<byte>();

            for(int i = 0; i < content.Length; i++)
            {
                if(i < start || i >= end)
                {
                    result.Add(content[i]);
                    continue;
                }
                
                bool match = true;
                for (int j = 0; j < from.Length; j++)
                {
                    if (i + j > end || content[i + j] != from[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    result.AddRange(to);
                    i += from.Length - 1;
                }
                else
                {
                    result.Add(content[i]);
                }
            }

            return result.ToArray();
        }
    }
}
