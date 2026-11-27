# Kiểm thử Chat 07 — GameRanking v0.1

## Kiểm thử ban đầu của Codex

Kiểm thử ngày 27/11/2026 với .NET SDK 10.0.403, SQL Server `.\SQLDEV`, database `GameRankingDemo` và bản Release trên cổng 5087. Đã khôi phục phụ thuộc NuGet hiện có. Build solution và bộ kiểm thử đều thành công với 0 lỗi, 0 cảnh báo khi xuất sang thư mục riêng. Tiến trình demo cũ đang giữ tệp thực thi mặc định.

## Thuật toán và giao diện

| Phạm vi | Kết quả |
| --- | --- |
| Bubble, Selection, Insertion, Merge, Quick và Heap | Mỗi thuật toán đạt 12 ca C# gồm hai bộ khóa và sáu dạng mảng nhỏ. Có mảng rỗng, một phần tử, hai phần tử, dữ liệu có hai thứ tự và phần tử lặp. |
| Hai bảng chính | Hai bộ kỳ vọng kiểm tra thứ tự theo thời điểm, EXP trong cùng cấp độ, ranh giới cấp độ, thứ tự ID và Dense Ranking 1, 1, 2, 3, 4. |
| Dữ liệu giả lập | 48 kết quả đạt trên hai tiêu chí, bốn đặc điểm dữ liệu và sáu thuật toán. Mỗi kết quả đối chiếu đủ năm lượt. |
| Quick Sort | Đạt hai ca 100.000 phần tử với thứ tự ngược và các khóa bằng nhau. |
| Chạy từ trang MVC | HTTP trả đúng sáu hàng tại 100, 1.000 và 5.000 người chơi ở cả hai tiêu chí. Chạy riêng Merge trên 10.000 hồ sơ thứ tự ngược trả đúng một hàng. So sánh sáu thuật toán trên 500 hồ sơ nhiều giá trị trùng trả đúng sáu hàng. |
| Ràng buộc đầu vào | Giá trị 99.999 và mã tiêu chí ngoài danh sách bị từ chối. Edge xác nhận biểu mẫu hiển thị lựa chọn 10.000. |
| Phạm vi đo | Kiểm tra mã nguồn xác nhận mảng gốc tạo một lần, mỗi lần sắp xếp nhận bản sao mới trước mốc bắt đầu và kiểm chứng diễn ra sau mốc kết thúc. |

## Đối chiếu SQL trước và sau benchmark

Dùng `sqlcmd` đọc toàn bộ cột của từng bảng, sắp xếp theo khóa chính, chuyển thành JSON gồm cả giá trị NULL rồi tính SHA-256 trong SQL Server. Phép đối chiếu bao gồm điểm, EXP, thắng, thua, hai mốc UTC, phiên bản dòng và toàn bộ trận. Chỉ in số dòng và dấu kiểm.

| Bảng | Dòng trước | Dòng sau | SHA-256 giống nhau trước và sau |
| --- | ---: | ---: | --- |
| `Players` | 19 | 19 | `60AAE46CF1F3D679838DF3C382C9D103951C888D5E1460733B270BE5C7882D14` |
| `Matches` | 12 | 12 | `B018C2B21753D5609E3D091FA42E6438AF34086B250DE11E59B6BFAC4D8125BD` |

## Hồi quy sau khi hoàn tất benchmark

- HTTP xác nhận cả hai bảng chính trả trang đầy đủ và trả riêng các dòng qua `?rows=true`. Khách truy cập hồ sơ, lịch sử hoặc mô phỏng được chuyển đến trang đăng nhập.
- Edge headless đăng ký hai hồ sơ QA, đăng xuất, đăng nhập lại và xem hồ sơ đúng tài khoản. Trận mô phỏng xuất hiện trong lịch sử cá nhân với đúng đối thủ.
- Cả hai bảng đang mở tự hiển thị điểm, EXP, thắng và thua mới sau trận mà không tải lại trang. Người thắng nhận 50 điểm và 100 EXP. Người thua nhận 5 điểm và 20 EXP.
- Kiểm thử hồi quy có chủ đích đã thêm hai hồ sơ QA (ID 20, 21) và trận ID 16. SQL ghi nhận 21 hồ sơ và 13 trận sau kiểm thử này. Hai mốc thành tích của mỗi người tham gia đều khớp thời điểm UTC của trận. Dữ liệu QA được giữ lại.
- Phạm vi hồi quy tập trung vào luồng chính. Chưa chạy lại toàn bộ 18 ca Chat 06, ví dụ gửi đồng thời và ép lỗi giao dịch.

## Chạy lại

```powershell
dotnet run --project tests/GameRanking.SortingChecks -c Release
```

Khi tệp thực thi của bản demo đang bị khóa, build và chạy từ thư mục đầu ra riêng.

```powershell
dotnet build GameRanking.slnx -c Release --no-restore -p:OutputPath=bin/Chat07Verified/ -p:UseAppHost=false
$env:ASPNETCORE_ENVIRONMENT = 'Development'
Push-Location scr/GameRanking.Web
dotnet bin/Chat07Verified/GameRanking.Web.dll --urls http://localhost:5087
Pop-Location
```

Chọn **Thử nghiệm thuật toán** và dùng lại seed 42 để tái tạo dữ liệu với cùng tiêu chí, kích thước và đặc điểm. [Bảng số liệu thực nghiệm](ket-qua-thuc-nghiem.md)

## Nghiệm thu của chủ dự án ngày 28/11/2026

Chủ dự án xác nhận **12/12 ca đạt**. Đợt nghiệm thu dùng .NET SDK 10.0.403, target net10.0, cấu hình Release, SQL Server `.\SQLDEV`, database `GameRankingDemo` và website tại `http://localhost:5050`. Chủ dự án đã chạy thành công hai lệnh sau.

```powershell
dotnet build GameRanking.slnx -c Release --no-restore
dotnet run --project scr/GameRanking.Web -c Release --no-build --launch-profile http -- --urls http://localhost:5050
```

| Mã | Nội dung | Kết quả |
| --- | --- | --- |
| TC01 | Website và điều hướng | Đạt |
| TC02 | Quick Sort với 100 người chơi | Đạt |
| TC03 | Sáu thuật toán, tổng điểm, 1.000 người chơi | Đạt |
| TC04 | Sáu thuật toán, cấp độ, 1.000 người chơi | Đạt |
| TC05 | Dữ liệu đã sắp xếp | Đạt |
| TC06 | Dữ liệu thứ tự ngược | Đạt |
| TC07 | Dữ liệu nhiều giá trị trùng | Đạt |
| TC08 | Chức năng seed | Đạt |
| TC09 | Sáu thuật toán với 10.000 người chơi | Đạt |
| TC10 | Tính độc lập dữ liệu | Đạt |
| TC11 | Hồi quy nghiệp vụ | Đạt |
| TC12 | Bộ kiểm thử tự động C# | Đạt |

- **TC08.** Ba lượt dùng seed 42, 42 và 100. Cả 18 kết quả đối chiếu đều đạt. Chưa so trực tiếp mảng đầu vào hoặc mã băm để chứng minh hai lượt cùng seed sinh dữ liệu giống hệt.
- **TC09.** Hoàn tất sau khoảng mười mấy giây chờ trên website. Chủ dự án ghi nhận ứng dụng hoạt động bình thường. [Số liệu trung vị](ket-qua-thuc-nghiem.md#bộ-đo-nghiệm-thu-của-chủ-dự-án)
- **TC10.** Đối chiếu thủ công trước và sau benchmark ghi nhận 21 → 21 người chơi và 13 → 13 trận. `PlayerId`, `DisplayName`, `TotalScore`, `TotalExp`, `Wins` và `Losses` không đổi. Cả hai bảng hoạt động bình thường và không phát hiện dữ liệu benchmark trong dữ liệu chính. Chủ dự án quan sát khác biệt thứ tự, chưa xác định giữa hai màn hình hay trong cùng một bảng. Dấu kiểm SHA-256 ở mục trên thuộc lượt kiểm thử Codex ngày 27/11.
- **TC11.** Đạt 22/22 tiêu chí gồm đăng nhập, hồ sơ, thành tích ban đầu, mô phỏng trận hợp lệ, cộng điểm và EXP, cập nhật thắng/thua/tổng số trận/cấp độ/tỷ lệ thắng, tự cập nhật hai bảng, lịch sử chính xác không trùng trong ca thử, giới hạn cập nhật ở hai người tham gia, đăng xuất và bảo vệ trang yêu cầu xác thực. Có đúng một trận mới và không thêm người chơi. Biên bản chưa ghi tên người thắng và người thua. Sau nghiệm thu, chủ dự án ghi nhận **21 người chơi và 14 trận**.

### TC12

Chủ dự án chạy `dotnet run --project tests/GameRanking.SortingChecks -c Release`. Đầu ra ghi nhận 72 ca thuật toán, 48 kết quả benchmark, hai bảng chính và hai ca Quick Sort quy mô lớn.

```text
PASS Bubble (12 ca)
PASS Selection (12 ca)
PASS Insertion (12 ca)
PASS Merge (12 ca)
PASS Quick (12 ca)
PASS Heap (12 ca)
PASS 2 bảng chính, 48 kết quả benchmark và 2 ca Quick Sort lớn.
```

`$LASTEXITCODE` trả về `0`.

### Nội dung bàn giao sang Chat 08

- So sánh trực tiếp đầu vào cùng seed và xác minh thứ tự của từng bảng khi dữ liệu giữ nguyên.
- Kiểm thử đồng thời, gửi yêu cầu lặp, rollback giao dịch, quyền truy cập và bảo mật chuyên sâu.
- Rà soát giao diện trình diễn, tiến trình và cổng chạy, nguy cơ khóa tệp build, dữ liệu QA và phương án dữ liệu demo.
- Hoàn thiện hướng dẫn chạy và kịch bản demo. Benchmark hiện mở công khai, với một lượt chạy đồng thời trên mỗi tiến trình.
