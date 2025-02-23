using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using grpcFileTransportServer;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace grpcServer.Services
{
    public class FileTransportService : FileService.FileServiceBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileTransportService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<Empty> FileUpload(IAsyncStreamReader<BytesContent> requestStream, ServerCallContext context)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "files");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream? fileStream = null;
            try
            {
                int count = 0;
                decimal chunkSize = 0;

                while (await requestStream.MoveNext())
                {
                    if (count++ == 0)
                    {
                        string filePath = Path.Combine(path, $"{requestStream.Current.Info.FileName}{requestStream.Current.Info.FileExtension}");
                        fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        // `SetLength()` gerekmez, çünkü dosya zaten yazılıyor.
                    }

                    var buffer = requestStream.Current.Buffer.ToByteArray();
                    await fileStream!.WriteAsync(buffer, 0, buffer.Length);

                    // İlerleme yüzdesi yazdır
                    Console.WriteLine($"{Math.Round(((chunkSize += requestStream.Current.ReadedByte) * 100) / requestStream.Current.FileSize)}%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FileUpload Error: {ex.Message}");
            }
            finally
            {
                if (fileStream != null)
                {
                    await fileStream.DisposeAsync();
                    fileStream.Close();
                }
            }

            return new Empty();
        }

        public override async Task FileDownload(fileInfo request, IServerStreamWriter<BytesContent> responseStream, ServerCallContext context)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "files");
            string filePath = Path.Combine(path, $"{request.FileName}{request.FileExtension}");

            if (!File.Exists(filePath))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "File not found"));
            }

            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[2048];

            BytesContent content = new BytesContent
            {
                FileSize = fileStream.Length,
                Info = new fileInfo
                {
                    FileName = Path.GetFileNameWithoutExtension(fileStream.Name),
                    FileExtension = Path.GetExtension(fileStream.Name)
                },
                ReadedByte = 0
            };

            while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                content.Buffer = ByteString.CopyFrom(buffer, 0, (int)content.ReadedByte);
                await responseStream.WriteAsync(content);
            }
        }
    }
}
