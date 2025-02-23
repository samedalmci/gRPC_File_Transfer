using Grpc.Net.Client;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using grpcFileTransportDownLoadClient; // fileTransport'u kaldır

namespace grpcDownLoadClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5082");
            var client = new FileService.FileServiceClient(channel); // file.FileService kaldırıldı

            string downloadPath = @"C:\\Users\\samed\\Desktop\\gRpcFile\\grpcDownLoadClient\\DownloadFiles";

            var fileInfo = new fileInfo // fileInfo burada doğrudan çağrılıyor
            {
                FileExtension = ".mp4",
                FileName = "SamedDeneme"
            };

            FileStream fileStream = null;
            var request = client.FileDownload(fileInfo);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            int count = 0;
            long chunkSize = 0;

            while (await request.ResponseStream.MoveNext(cancellationTokenSource.Token))
            {
                var current = request.ResponseStream.Current;
                if (current == null || current.Info == null)
                {
                    Console.WriteLine("Geçersiz veri alındı.");
                    break;
                }

                if (count++ == 0)
                {
                    string filePath = Path.Combine(downloadPath, $"{current.Info.FileName}{current.Info.FileExtension}");
                    fileStream = new FileStream(filePath, FileMode.Create);
                    fileStream.SetLength(current.FileSize);
                }

                var buffer = current.Buffer.ToByteArray();
                await fileStream.WriteAsync(buffer, 0, current.ReadedByte);

                chunkSize += current.ReadedByte;
                Console.WriteLine($"{Math.Round(((double)chunkSize * 100 / current.FileSize))}%");
            }

            Console.WriteLine("Yüklendi...");
            if (fileStream != null)
            {
                await fileStream.DisposeAsync();
                fileStream.Close();
            }
        }
    }
}
