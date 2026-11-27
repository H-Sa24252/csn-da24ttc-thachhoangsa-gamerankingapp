# Kết quả kiểm thử GameRanking v0.1

Ngày kiểm thử: 27/11/2026. Máy Windows, SDK .NET 10.0.403, ASP.NET Core MVC trên net10.0, EF Core, Identity EF và dotnet-ef 10.0.15.

| Hạng mục | Kết quả thực tế |
| --- | --- |
| Restore và build | Restore solution và local tool thành công. Build sau khi sinh migration hoàn tất với 0 lỗi, 0 cảnh báo. |
| Migration | SQL Server `.\SQLDEV` xác thực bằng tài khoản Windows. Database demo chưa tồn tại trước khi tạo. `20261127141429_InitialCreate` được áp dụng và EF không phát hiện thay đổi mô hình chưa có migration. |
| Lược đồ | SQL Server ghi nhận 7 bảng, gồm 4 bảng Identity, `Players`, `Matches` và `__EFMigrationsHistory`. Kiểm tra 7 khóa chính, 6 khóa ngoại và 3 ràng buộc kiểm tra. Các khóa ngoại và các ràng buộc kiểm tra đều đang bật và được tin cậy. |
| Dữ liệu mẫu | SQL xác nhận 10 hồ sơ không liên kết với tài khoản, số liệu thành tích khởi tạo bằng 0 và dùng chung mốc UTC `2026-11-27 14:16:12.4933296`. Khởi động lại ứng dụng giữ nguyên 10 hồ sơ và mốc thời gian. |
| Hai bảng xếp hạng | HTTP xác nhận 10 hồ sơ mẫu cùng hạng 1 và có tỷ lệ thắng 0%. Kiểm tra Merge Sort bằng dữ liệu riêng trong bộ nhớ xác nhận thứ tự theo điểm, cấp độ, EXP và mốc thời gian, đồng hạng, thứ tự hiển thị theo ID và danh sách rỗng. |
| Tài khoản | HTTP từ chối email sai định dạng và tên hiển thị chỉ có khoảng trắng. Tạo được hai tài khoản QA. SQL xác nhận cả hai mật khẩu có giá trị băm. Đăng xuất chặn truy cập hồ sơ, đăng nhập lại khôi phục quyền truy cập. |
| Hồ sơ và quyền xem | Người chưa đăng nhập được chuyển đến trang đăng nhập. Phiên của tài khoản A vẫn chỉ mở hồ sơ A khi đường dẫn chứa ID của B. Sửa tên hiển thị được lưu và hiện lên bảng xếp hạng. Hai email QA chỉ xuất hiện trong hồ sơ tương ứng trong các trang đã kiểm tra. |
| Trình duyệt | Playwright dùng Edge headless tại `http://localhost:5049`. Kiểm tra điều hướng đến hai bảng xếp hạng, biểu mẫu đăng ký và chuyển hướng đến trang đăng nhập khi mở hồ sơ riêng tư. Không phát hiện lỗi JavaScript của trang trong lượt kiểm tra. |
| Nhật ký | Hai lượt khởi động xác nhận dữ liệu và tài khoản trên `http://localhost:5050` hoàn tất với nhật ký không có cảnh báo hoặc lỗi. |

Hai tài khoản giả lập tạo trong kiểm thử đăng ký vẫn có hồ sơ trong database demo, ngoài 10 hồ sơ mẫu. Mật khẩu thử được tạo khi chạy kiểm thử. Lược đồ lưu kết quả đã có để Chat 06 triển khai mô phỏng và trang lịch sử cá nhân.

## Cách kiểm tra lại

Chạy theo README tại thư mục gốc. Mở hai bảng xếp hạng và kiểm tra 10 dòng mang tên “Người chơi mẫu”. Dừng và chạy lại ứng dụng, rồi kiểm tra số hồ sơ mẫu bằng truy vấn sau.

```sql
SELECT COUNT(*) AS SampleCount,
       MIN(ScoreReachedAtUtc) AS SeedUtc
FROM dbo.Players
WHERE UserId IS NULL;
```

Mở hồ sơ trong cửa sổ riêng tư để kiểm tra chuyển hướng. Đăng ký một tài khoản thử bằng địa chỉ thuộc `example.test`, sửa tên hiển thị, đăng xuất và đăng nhập lại. Kiểm tra tên mới trên bảng công khai và email trong hồ sơ riêng.
