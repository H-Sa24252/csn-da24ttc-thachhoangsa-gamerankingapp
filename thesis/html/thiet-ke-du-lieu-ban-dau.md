# Thiết kế dữ liệu ban đầu — GameRanking v0.1

Chủ dự án đã duyệt mô hình, 10 hồ sơ mẫu và phạm vi lịch sử trong Chat 05. Đích triển khai là SQL Server `.\SQLDEV`, cơ sở dữ liệu `GameRankingDemo`, kết nối bằng Windows Authentication.

## Tài khoản

Đề xuất dùng ASP.NET Core Identity với đăng nhập bằng email và mật khẩu. Identity băm mật khẩu, xác thực thông tin đăng nhập và quản lý cookie. Email được chuẩn hóa và có chỉ mục duy nhất để tránh đăng ký trùng. Dùng `IdentityUserContext` cho tài khoản, không cần mô hình vai trò.

| Bảng | Cột và mục đích | Khóa và ràng buộc |
| --- | --- | --- |
| `AspNetUsers` | `Id nvarchar(450)`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail` kiểu `nvarchar(256)`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber` kiểu `nvarchar(max)`, các cờ xác nhận email/điện thoại, xác thực hai bước và khóa tài khoản kiểu `bit`, `LockoutEnd datetimeoffset`, `AccessFailedCount int`. | PK `Id`. Chỉ mục duy nhất trên tên đăng nhập và email đã chuẩn hóa. |
| `AspNetUserClaims` | `Id int`, `UserId nvarchar(450)`, `ClaimType`, `ClaimValue` kiểu `nvarchar(max)`. Lưu thuộc tính xác thực theo chuẩn Identity. | PK `Id`, FK `UserId` tới tài khoản. |
| `AspNetUserLogins` | `LoginProvider`, `ProviderKey` kiểu `nvarchar(128)`, `ProviderDisplayName nvarchar(max)`, `UserId nvarchar(450)`. Bảng hỗ trợ của Identity. | PK ghép `(LoginProvider, ProviderKey)`, FK `UserId` tới tài khoản. |
| `AspNetUserTokens` | `UserId nvarchar(450)`, `LoginProvider`, `Name` kiểu `nvarchar(128)`, `Value nvarchar(max)`. Bảng hỗ trợ của Identity. | PK ghép `(UserId, LoginProvider, Name)`, FK `UserId` tới tài khoản. |

## Người chơi và kết quả

| Bảng | Cột và mục đích | Khóa và ràng buộc |
| --- | --- | --- |
| `Players` | `PlayerId int IDENTITY`, `UserId nvarchar(450) NULL`, `DisplayName nvarchar(80) NOT NULL`, `TotalScore`, `TotalExp`, `Wins`, `Losses` kiểu `int NOT NULL` mặc định 0, `ScoreReachedAtUtc`, `ExpReachedAtUtc` kiểu `datetime2(7) NOT NULL`, `RowVersion rowversion`. | PK `PlayerId`. FK `UserId` tới tài khoản kèm chỉ mục duy nhất có lọc `UserId IS NOT NULL`. Các số liệu thành tích không âm. Tên hiển thị sau khi bỏ khoảng trắng phải có nội dung. |
| `Matches` | `MatchId int IDENTITY`, `WinnerPlayerId`, `LoserPlayerId` kiểu `int NOT NULL`, `PlayedAtUtc datetime2(7) NOT NULL`. Một dòng ghi một kết quả mô phỏng. | PK `MatchId`. Hai FK tới `Players.PlayerId`, kiểm tra hai ID khác nhau và chặn xóa dây chuyền. Chỉ mục theo từng ID người chơi và thời điểm để truy vấn lịch sử. |

- Khi đăng ký, tạo tài khoản và hồ sơ người chơi trong cùng giao dịch. Một tài khoản có một hồ sơ. Đề xuất 10 hồ sơ mẫu có `UserId = NULL` để tham gia mô phỏng mà không cần thông tin đăng nhập mẫu. Dùng cùng một giá trị UTC cho cả hai mốc của 10 hồ sơ. Seed chỉ khi `Players` trống và phải chạy an toàn khi có nhiều tiến trình.
- Công khai tên hiển thị và thành tích. Trang hồ sơ và lịch sử lấy người chơi theo ID tài khoản từ phiên đăng nhập. Lịch sử lọc các trận có người chơi đó thắng hoặc thua, hiển thị đối thủ, kết quả và thời điểm. Đề xuất cho phép sửa tên hiển thị trong Chat 05, email được dùng để đăng nhập.
- Khi mô phỏng, ghi trận và cập nhật cả hai người chơi trong cùng giao dịch. Người thắng nhận 50 điểm và 100 EXP, người thua nhận 5 điểm và 20 EXP. Dùng `RowVersion` để phát hiện cập nhật đồng thời. Cập nhật riêng từng mốc UTC khi chỉ số tương ứng thay đổi.
- Tính cấp độ bằng `1 + TotalExp / 500` với phép chia nguyên, tổng số trận bằng `Wins + Losses`, tỷ lệ thắng bằng `100 × Wins / TotalMatches` và bằng 0% khi chưa đấu.
- Bảng điểm so sánh tổng điểm giảm dần rồi mốc đạt điểm tăng dần. Bảng cấp độ so sánh cấp độ và EXP giảm dần rồi mốc đạt EXP tăng dần. Merge Sort chạy trong C#. Dense Ranking chỉ tăng hạng khi tiêu chí nghiệp vụ thay đổi. ID chỉ ổn định thứ tự hiển thị giữa người đồng hạng.
- Lược đồ hỗ trợ một trò chơi mô phỏng. Top-N và quản lý thông tin trò chơi được thiết kế sau demo. Dữ liệu so sánh sáu thuật toán dùng tập dữ liệu riêng. Chat 06 triển khai mô phỏng trận và lịch sử thi đấu theo nhiệm vụ D.

## Bổ sung từ Chat 06

- `Matches.RequestId uniqueidentifier NULL` có chỉ mục duy nhất với điều kiện `RequestId IS NOT NULL`. Ứng dụng tạo mã cho mỗi biểu mẫu, gắn mã với người gửi và giữ trận đã lưu nếu cùng yêu cầu được gửi lại. Trận có từ trước migration có thể giữ giá trị NULL.
- Ghi trận và cập nhật hai người chơi trong cùng lần lưu EF Core. Nếu phát hiện xung đột bằng `RowVersion`, dịch vụ đọc lại dữ liệu và thử lại có giới hạn. Hai mốc thành tích vẫn là hai cột riêng, cùng nhận giờ UTC của trận vì điểm và EXP đều tăng.
- Lịch sử sử dụng kết quả của trận cùng mức thưởng cố định 50/100 hoặc 5/20. Nếu đồ án cho phép sửa mức thưởng trong tương lai, cần lưu mức thưởng theo trận hoặc phiên bản quy tắc để giữ đúng lịch sử.
