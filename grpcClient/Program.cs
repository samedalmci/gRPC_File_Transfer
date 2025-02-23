using Google.Protobuf;
using Grpc.Net.Client;
using grpcFileTransportClient;
using System.IO;

namespace grpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5082");
            var client = new FileService.FileServiceClient(channel);

            string file = @"C:\Users\samed\Desktop\SamedDeneme.mp4";

            using FileStream fileStream = new FileStream(file, FileMode.Open);

            var content = new BytesContent
            {
                FileSize = fileStream.Length,
                ReadedByte = 0,
                Info = new grpcFileTransportClient.fileInfo { FileName = Path.GetFileName(fileStream.Name) }  // Uzantıyı da dahil ediyoruz
            };

            var upload = client.FileUpload();

            byte[] buffer = new byte[2048];
            while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                content.Buffer = ByteString.CopyFrom(buffer, 0, content.ReadedByte);
                await upload.RequestStream.WriteAsync(content);
            }

            // Veriyi göndermeyi bitiriyoruz
            await upload.RequestStream.CompleteAsync();

            // Dosya akışını kapatıyoruz
            fileStream.Close();
        }
    }
}
