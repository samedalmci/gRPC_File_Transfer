# gRPC_File_Transfer
 Bu proje, gRPC kullanarak dosya yükleme (Upload) ve indirme (Download) işlemlerini gerçekleştiren bir uygulamadır. Yüksek performanslı veri transferi sağlar ve istemci-sunucu iletişimini hızlandırır.
 
✨ Özellikler

Dosya Yükleme: Kullanıcılar istedikleri dosyaları sunucuya yükleyebilir.

Dosya İndirme: Sunucuda bulunan dosyalar, isme göre istemciler tarafından indirilebilir.

Gerçek Zamanlı İşlem: gRPC'nin hızlı ve optimize edilmiş protokolü sayesinde minimum gecikmeyle veri alışverişi yapılabilir.



👨‍💻Dosya yüklemek için:

Yüklemek istediğiniz dosyanın yolunu grpcClient'ın içindeki program.cs'de bulunan alana girin.

sırası ile grpcServer ve grpcClient'ı önce dotnet build sonra dotnet run komutu ile çalıştırın.

Dosya başarıyla grpcServer'ın içindeki wwwroot/file klasörüne yüklenir.


👨‍💻Dosya indirmek için:

İndirmek istediğiniz dosyanın ismini ve uzantısını grpcDownLoadClient'ın içindeki program.cs'de bulunan alanlara girin.

sırası ile grpcServer ve grpcDownLoadClient'ı önce dotnet build sonra dotnet run komutu ile çalıştırın.

Dosya istemciye başarılı şekilde indirilir.


🌟 Teknolojiler

C# – Ana programlama dili

.NET 9.0 – Uygulamanın çalıştığı çerçeve

gRPC – Sunucu-istemci iletişimi için yüksek performanslı framework

Protocol Buffers (proto3) – Veri serileştirme için kullanılan dil

