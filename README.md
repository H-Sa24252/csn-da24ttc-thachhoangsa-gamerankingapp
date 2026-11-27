# ỨNG DỤNG QUẢN LÝ VÀ XẾP HẠNG NGƯỜI CHƠI GAME
> **Đồ án cơ sở ngành Công nghệ Thông tin – Đại học Trà Vinh**

## 📋 THÔNG TIN TÁC GIẢ & GIẢNG VIÊN HƯỚNG DẪN
* **Họ và tên sinh viên:** Thạch Hoàng Sa
* **Mã số sinh viên:** 110124252
* **Mã lớp:** DA24TTC
* **Số điện thoại:** 0373107408
* **Email:** tsusa1605@gmail.com
* **Giảng viên hướng dẫn:** ThS. Lê Phong Dũ

---

## 📂 CẤU TRÚC THƯ MỤC REPOSITORY
* `/setup`: Tập tin cài đặt CSDL và dữ liệu thực nghiệm.
* `/scr`: Mã nguồn chính của ứng dụng Web C#.
* `/progress-report`: Báo cáo tiến độ hàng tuần.
* `/thesis`: Báo cáo (.doc, .pdf), slide (.pptx) và tài liệu tham khảo.

---

## 🛠️ HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY CHƯƠNG TRÌNH (`/setup`)
Cài .NET SDK 10.0.403 và chuẩn bị SQL Server có instance `.\SQLDEV`. Tài khoản Windows chạy migration cần quyền tạo hoặc cập nhật database `GameRankingDemo`. Ứng dụng dùng Windows Authentication. Tùy chọn tin cậy chứng chỉ SQL trong cấu hình phục vụ demo trên máy cục bộ.

Chạy các lệnh từ thư mục gốc repository:

```powershell
dotnet --version
dotnet restore GameRanking.slnx
dotnet tool restore
dotnet build GameRanking.slnx --no-restore
dotnet ef database update --project scr/GameRanking.Web --no-build
dotnet run --project scr/GameRanking.Web --no-build --launch-profile http
```

Mở [trang chủ](http://localhost:5049), sau đó vào **Xếp hạng tổng điểm** hoặc **Xếp hạng cấp độ** để xem danh sách người chơi. Khi bảng người chơi còn trống, ứng dụng tạo 10 hồ sơ mẫu. Dùng **Đăng ký** để tạo tài khoản riêng và **Hồ sơ của tôi** để xem thành tích hoặc sửa tên hiển thị. Có thể mở `GameRanking.slnx` trong Visual Studio 2026 và chọn `GameRanking.Web` làm project khởi động.

Sau khi đăng nhập, chọn **Mô phỏng trận đấu**, chọn hai người chơi khác nhau rồi gửi biểu mẫu. Hệ thống chọn ngẫu nhiên người thắng, lưu kết quả và cộng điểm, EXP cho cả hai người. Từ **Hồ sơ của tôi**, mở **Xem lịch sử thi đấu** để xem những trận mà người chơi của tài khoản đã tham gia.

Hai bảng xếp hạng dùng Merge Sort và Dense Ranking. Khi trang bảng xếp hạng đang hiển thị, các dòng tự cập nhật định kỳ khoảng 5 giây. Với database từ Chat 05, chạy `dotnet ef database update` theo lệnh đầy đủ ở trên để bổ sung khóa chống lưu trùng trước khi chạy website.

Nếu cổng 5049 đang bận, chạy trên [localhost:5050](http://localhost:5050) bằng lệnh sau:

```powershell
dotnet build GameRanking.slnx -c Release --no-restore
dotnet run --project scr/GameRanking.Web -c Release --no-build --launch-profile http -- --urls http://localhost:5050
```

[Thiết kế dữ liệu](thesis/html/thiet-ke-du-lieu-ban-dau.md) · [Kiểm thử nền tảng Chat 05](thesis/html/kiem-thu-gameranking-v0.1.md) · [Kiểm thử mô phỏng và lịch sử Chat 06](thesis/html/kiem-thu-chat-06.md)
