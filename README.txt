GameApi - ASM 1 (GAM106) - bản dùng SQL Server (SSMS)

1) Mở appsettings.json, sửa Server cho đúng tên server bạn đang kết nối trong SSMS
   (ví dụ localhost hoặc TÊN-MÁY\\SQLEXPRESS - trong file JSON phải viết hai dấu \\).
2) Mở terminal tại thư mục này:
     dotnet build
     dotnet ef migrations add InitialCreate
   (chưa có dotnet-ef: dotnet tool install --global dotnet-ef)
3) Chạy:
     dotnet run
   App tự tạo database + bảng (Database.Migrate) và mở http://localhost:5080/swagger
   Hoặc tạo tay bằng: dotnet ef database update
4) Mở SSMS -> Connect -> chuột phải Databases -> Refresh -> game_asm1_demo -> Tables
   (dbo.Accounts, dbo.Characters).
5) Làm 12 ca TC01..TC12 theo mục 13 của PDF.

Lưu ý: đề gốc dùng MySQL. Hãy hỏi giảng viên xem có chấp nhận SQL Server không.
Nếu máy dùng SDK 9 và muốn khớp: đổi <TargetFramework> thành net9.0 trong GameApi.csproj.
