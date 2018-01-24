using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.WebApi.Owin
{
    /// <summary>
    /// A prototype HttpContent that demonstrates how to efficiently send static files via WebApi+Owin.
    /// </summary>
    using SendFileFunc = Func<string, long, long?, CancellationToken, Task>;

    public class FileContent : HttpContent
    {

        private const int DefaultBufferSize = 1024 * 64;

        private readonly long? _count;
        private readonly FileInfo _fileInfo;
        private readonly string _fileName;
        private readonly long _offset;



        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>

        public FileContent(string fileName) : this(fileName, 0L, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>

        public FileContent(string fileName, long offset, long? count)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            this._fileInfo = new FileInfo(fileName);
            if (!this._fileInfo.Exists)
            {
                throw new FileNotFoundException(string.Empty, fileName);
            }
            if (offset < 0L || offset > this._fileInfo.Length)
            {
                throw new ArgumentOutOfRangeException("offset", offset, string.Empty);
            }
            if (count.HasValue && count.Value - offset > this._fileInfo.Length)
            {
                throw new ArgumentOutOfRangeException("count", count.Value, string.Empty);
            }
            this._fileName = fileName;
            this._offset = offset;
            this._count = count;
            long num = this._count ?? (this._fileInfo.Length - this._offset);
            long to = this._offset + num - 1L;
            base.Headers.ContentRange = new ContentRangeHeaderValue(this._offset, to, this._fileInfo.Length);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sendFileFunc"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task SendFileAsync(Stream stream, SendFileFunc sendFileFunc, CancellationToken cancel)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (sendFileFunc == null)
            {
                throw new ArgumentNullException("sendFileFunc");
            }

            return sendFileFunc(_fileName, _offset, _count, cancel);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (FileStream fileStream =
                new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, DefaultBufferSize,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                fileStream.Seek(_offset, SeekOrigin.Begin);
                // await fileStream.CopyToAsync(stream, bufferSize, _count);
            }
            throw new NotImplementedException();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>

        protected override bool TryComputeLength(out long length)
        {
            length = (this._count ?? (this._fileInfo.Length - this._offset));
            return true;
        }



    }
}
