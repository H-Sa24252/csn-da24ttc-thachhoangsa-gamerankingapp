# Trạng thái MVP GameRanking v0.1

## Dữ liệu trình diễn

Đầu Chat 08, truy vấn xác nhận `GameRankingDemo` có 21 người chơi, 14 trận và hai migration. Biên bản Chat 05–07 là căn cứ về nguồn gốc QA, gồm 10 mẫu ban đầu, 2 QA Chat 05, 6 QA Chat 06, 2 QA Chat 07 và 1 hồ sơ có sẵn khác theo hồ sơ bàn giao. Các lần kiểm thử cũ có ghi trận. Chưa có căn cứ quy nguồn riêng cho mọi trận. Hai bộ số liệu thực nghiệm lịch sử được giữ nguyên.

Truy vấn hiện tại nhận diện 10 hồ sơ mẫu và 6 hồ sơ có nhãn QA06. Nguồn gốc của 5 hồ sơ còn lại dựa vào biên bản bàn giao, vì nhãn hiện tại chưa đủ để phân nhóm độc lập. SHA-256 của toàn bộ `Players` và `Matches` khớp xuyên suốt các lần đối chiếu Chat 08.

| Phương án | Công sức và cấu hình | Rủi ro, quyền và khôi phục |
| --- | --- | --- |
| A. Dùng GameRankingDemo | Nhanh nhất. Dùng cấu hình ứng dụng mặc định và dữ liệu xếp hạng đã có. | Bảng có dữ liệu QA cũ. Cần sự đồng ý nếu tạo trận hoặc tài khoản khi demo. Chế độ minh họa bằng lịch sử đã lưu tránh phát sinh dữ liệu. Trận mới được lưu lâu dài trong MVP. |
| B. Dùng database trình diễn riêng | Cần thêm thời gian để chạy migration, chuẩn bị tài khoản và chọn dữ liệu dễ giải thích. Đặt chuỗi kết nối cho riêng tiến trình chạy demo. | Cần duyệt database đích, quyền tạo database/migration và ghi dữ liệu. Kiểm tra đúng đích trước khi mô phỏng. Có thể quay lại dữ liệu hiện hữu bằng cách dừng tiến trình riêng và mở lại với cấu hình mặc định. |

Chủ dự án đã chọn **phương án A** cho tổng duyệt và demo ngày 28/11/2026, cho phép mô phỏng trận qua ứng dụng và giữ toàn bộ lịch sử. Đợt khôi phục SDK chỉ kiểm tra đọc và xác nhận `GameRankingDemo` vẫn có 21 hồ sơ, 14 trận. `GameRankingChat08QA` được giữ với 14 hồ sơ, 8 trận.

SDK 10.0.403 đã được cài bổ sung song song trên máy Windows x64. Quy trình build và chạy Release chuẩn trên cổng 5050 đã được xác minh. Bản Release tại `scr/GameRanking.Web/bin/Chat08Verified/` vẫn có thể dùng dự phòng.

## Đối chiếu quy định trường và đề cương

| Trạng thái | Hạng mục và bằng chứng |
| --- | --- |
| Đã hoàn thành cho MVP | Tên repository theo quy ước đồ án cơ sở ngành. README có thông tin tác giả/liên hệ và hướng dẫn. `scr` có ứng dụng web. `thesis/html` có tài liệu thiết kế, sáu thuật toán, hai bộ số liệu và kiểm thử. |
| Hoàn thành một phần | `setup` hiện giữ chỗ, còn thao tác cài đặt nằm ở README và migration trong mã nguồn. Có năm tệp báo cáo trong `progress-report`, cùng commit tiến độ từ 18/10 đến 22/11 rồi ba commit triển khai đến 28/11. Tình trạng cộng tác viên và hồ sơ nộp của trường chưa xác minh. |
| Chưa hoàn thành | `thesis/doc`, `thesis/pdf`, `thesis/abs` và `thesis/refs` có thư mục giữ chỗ. Quy định yêu cầu bản báo cáo có phần đầu, chương 1–5, tài liệu tham khảo và phụ lục, nội dung tối thiểu 30 trang A4, Times New Roman 13, giãn dòng 1,5 và tài liệu tham khảo theo IEEE. Chưa có bản cuối để kiểm tra định dạng. |
| Hoãn sau demo | Tìm kiếm, Top-N, quản lý thông tin trò chơi và hoàn thiện báo cáo, slide, tài liệu tham khảo. Đề cương có những hạng mục này còn MVP tập trung mô phỏng và xếp hạng. |
| Tùy chọn | `soft` và `docker` có thư mục giữ chỗ. MVP dùng .NET CLI và SQL Server cục bộ. |

Trường còn yêu cầu commit ít nhất mỗi tuần, báo cáo tiến độ hằng tuần, nộp bản in và liên kết GitHub, rồi phối hợp fork vào tài khoản khoa khi hoàn tất. Lần rà soát này dùng lịch sử Git cục bộ và các bản gốc ở `TaiLieuThamKhao`.

## Trạng thái nghiệm thu

**MVP v0.1 sẵn sàng trình diễn trong phạm vi đã thống nhất.** Chủ dự án nghiệm thu ngày 28/11/2026 với 10/10 giai đoạn, 139/139 tiêu chí thủ công và tổng duyệt đạt trong 9 phút. Biên bản kỹ thuật của Codex có 20/20 ca C08 đạt. Merge Sort vẫn là thuật toán mặc định của hai bảng.

Đối chiếu chỉ đọc sau tổng duyệt xác nhận `GameRankingDemo` có **21 người chơi, 16 trận**. Số 14 trận ở phần kiểm tra SDK được ghi trước nghiệm thu. Phiên tổng duyệt dùng phương án A. `GameRankingChat08QA` giữ 14 người chơi, 8 trận. [Biên bản và thông tin trận tổng duyệt](kiem-thu-tong-the-v0.1.md)

Nguyên nhân SDK từng biến mất và nguyên nhân nội bộ của thông báo từ tiện ích Cốc Cốc chưa được xác định. Giai đoạn tiếp theo chờ chủ dự án giao nhiệm vụ.
