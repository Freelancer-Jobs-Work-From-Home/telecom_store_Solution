# Telecom Store Solution

## 📌 Giới thiệu
**Telecom Store Solution** là một ứng dụng web được xây dựng bằng **ASP.NET Core 8.0 MVC**. 

---

## 🛠 Yêu cầu hệ thống
### **1. Cài đặt các công cụ cần thiết**
Để chạy dự án, bạn cần cài đặt:
- **.NET SDK 8.0**: [Tải tại đây](https://dotnet.microsoft.com/en-us/download)
- **SQL Server**: [Tải tại đây](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

Kiểm tra phiên bản .NET đã cài đặt:
```sh
 dotnet --version
```
Nếu kết quả là `8.x.x`, bạn đã cài đặt đúng.

---

## 🚀 Hướng dẫn cài đặt và chạy dự án

### **1. Clone repository**
```sh
git clone <repository-url>
cd telecom_store_Solution
```

### **2. Khôi phục các package NuGet**
Chạy lệnh sau để tải về các package cần thiết:
```sh
dotnet restore
```

### **3. Cấu hình chuỗi kết nối database**
Mở file `appsettings.json` và cập nhật thông tin kết nối SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```
Mở BussinessObject=> Data =>  Chỉnh sửa chuỗi kết nối trong AppDbContext.cs.
"Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"



### **4. Cập nhật cơ sở dữ liệu**
Nếu dự án sử dụng **Entity Framework Core**, chạy lệnh sau để áp dụng migrations:
```sh
dotnet ef database update
```

### **5. Chạy dự án**
Chạy lệnh sau để khởi động ứng dụng:
```sh
dotnet run
```
Hoặc, nếu chạy bằng Visual Studio, nhấn **F5** hoặc chọn **Run without Debugging (Ctrl + F5)**.

---

Tài khoản Admin	
Admin@gmail.com
123

