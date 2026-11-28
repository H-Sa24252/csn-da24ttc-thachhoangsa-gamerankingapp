# Hướng dẫn trình diễn GameRanking v0.1

## Chuẩn bị trên máy trình diễn

Mở PowerShell tại thư mục gốc repository. Cần .NET SDK 10.0.403 theo `global.json`, SQL Server `.\SQLDEV` và tài khoản Windows có quyền vào `GameRankingDemo`. Kiểm tra kết nối, hai migration và cổng trước khi mở ứng dụng.

```powershell
dotnet --version
sqlcmd -S '.\SQLDEV' -E -d GameRankingDemo -b -Q 'SELECT MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;'
Get-NetTCPConnection -State Listen -LocalPort 5050 -ErrorAction SilentlyContinue
dotnet build GameRanking.slnx -c Release --no-restore
dotnet run --project scr/GameRanking.Web -c Release --no-build --launch-profile http -- --urls http://localhost:5050
```

Mở [GameRanking](http://localhost:5050). Kiểm tra hai bảng, đăng nhập bằng tài khoản demo đã chuẩn bị và mở trang thử nghiệm trước giờ trình diễn. Mở hai bảng ở các cửa sổ đang hiển thị để quan sát cập nhật. Chuẩn bị sẵn [kết quả thực nghiệm](ket-qua-thuc-nghiem.md) để giải thích các lượt lớn.

Quy trình chuẩn trên cổng 5050 đã được xác minh lại với SDK 10.0.403 ngày 28/11/2026. Chủ dự án chọn dùng `GameRankingDemo` và cho phép lưu các trận mô phỏng hợp lệ trong buổi tổng duyệt và demo.

- Thiếu SDK 10.0.403 sẽ chặn lệnh CLI chuẩn. Nếu đã có bản xác minh Chat 08 tại `scr/GameRanking.Web/bin/Chat08Verified`, dùng lệnh dự phòng bên dưới trong phiên PowerShell riêng.
- Thiếu package thì chạy `dotnet restore GameRanking.slnx` trên máy có SDK phù hợp. Khi lỗi SQL, kiểm tra đúng instance, database, hai migration và quyền của tài khoản Windows. Seed lúc khởi động cần đọc được bảng Players và có thể tạo hồ sơ mẫu khi bảng trống.
- Nếu cổng hoặc tệp thực thi đang được sử dụng, kiểm tra PID và đường dẫn. Dùng Ctrl+C trong cửa sổ của phiên GameRanking do mình mở, hoặc chọn cổng trống và đổi URL tương ứng. Kết thúc demo bằng Ctrl+C trong cửa sổ chạy ứng dụng.

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
Push-Location scr/GameRanking.Web
dotnet bin/Chat08Verified/GameRanking.Web.dll --urls http://localhost:5050
Pop-Location
```

## Kịch bản 9 phút

| Thời gian | Trình diễn |
| --- | --- |
| 0:00–1:00 | Nêu bài toán xếp hạng trong một trò chơi giả lập. Giới thiệu ASP.NET Core MVC, C#, SQL Server và vai trò của thuật toán. |
| 1:00–2:00 | Đăng nhập, mở hồ sơ và đọc điểm, EXP, cấp độ, thắng, thua, tổng số trận và tỷ lệ thắng. |
| 2:00–4:00 | Mô phỏng giữa hồ sơ demo và một người chơi khác trên database được phép ghi. Chỉ ra thưởng thắng +50 điểm/+100 EXP và thua +5 điểm/+20 EXP. Giải thích cấp độ bằng 1 cộng phần nguyên của EXP chia 500. |
| 4:00–5:00 | Trở lại hai bảng đang hiển thị để quan sát lần làm mới khoảng 5 giây. Nêu khóa sắp xếp riêng của từng bảng và ví dụ đồng hạng 1, 1, 2. |
| 5:00–5:40 | Xem lịch sử của tài khoản tham gia và đối chiếu đối thủ, kết quả, thưởng. |
| 5:40–6:40 | Chạy Quick Sort với 100 hồ sơ ngẫu nhiên, tổng điểm, seed 42. |
| 6:40–7:40 | So sánh cả sáu với 1.000 hồ sơ, cùng seed. Nêu ý nghĩa trung vị năm lần đo, kiểm tra đúng đắn và dữ liệu giả lập trong bộ nhớ. |
| 7:40–9:00 | Dùng số liệu đã ghi để giải thích chi phí khi dữ liệu tăng. Merge Sort có thời gian bậc O(n log n) trong trường hợp xấu và cần bộ nhớ phụ. Kết thúc bằng giới hạn của MVP và các phần đề cương còn lại. |

Trên màn hình nhỏ, cuộn ngang trong vùng bảng để xem các cột thắng, thua, tổng trận và tỷ lệ thắng. Khi trình chiếu, dùng kích thước laptop hoặc rộng hơn để thấy toàn bộ hàng.

## Kết quả tổng duyệt

Chủ dự án xác nhận ngày 28/11/2026 tổng duyệt đạt 15/15 tiêu chí trong 9 phút, trong đợt nghiệm thu 10/10 giai đoạn và 139/139 tiêu chí. Dùng Microsoft Edge để trình diễn. `GameRankingDemo` có 21 người chơi và 16 trận khi đối chiếu sau tổng duyệt. Trận phát sinh hợp lệ trong buổi demo được lưu vào lịch sử.
