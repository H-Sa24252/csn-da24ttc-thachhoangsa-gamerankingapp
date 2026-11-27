# Kiểm thử mô phỏng trận và lịch sử thi đấu

Ngày kiểm thử 27/11/2026. Dùng .NET SDK 10.0.403, EF Core 10.0.15, SQL Server `.\SQLDEV`, Windows Authentication, Playwright trên Edge và HTTP tại `http://localhost:5050`. Bản Release build với 0 lỗi, 0 cảnh báo. Migration `20261127150046_AddMatchRequestId` đã áp dụng và EF xác nhận không còn thay đổi mô hình chờ migration.

| Mã | Kết quả thực tế |
| --- | --- |
| TC01 | PASS. Gửi biểu mẫu qua Edge cho hai người chơi khác nhau. SQL ghi đúng hai người tham gia, một người thắng và một người thua. |
| TC02 | PASS. Chọn trùng người chơi làm xuất hiện lỗi. |
| TC03 | PASS. Gửi ID không tồn tại bị từ chối. Bản ghi trận và thành tích giữ nguyên sau hai yêu cầu sai. |
| TC04 | PASS. Người thắng tăng 50 điểm và 100 EXP trong SQL Server. |
| TC05 | PASS. Người thua tăng 5 điểm và 20 EXP trong SQL Server. |
| TC06 | PASS. Số trận thắng, thua và tổng số trận hiển thị khớp dữ liệu. Người ngoài trận giữ nguyên chỉ số. Cả hai cột mốc thành tích của người tham gia khớp giờ UTC của trận. |
| TC07 | PASS. Sau trận đầu, bảng xếp hạng hiển thị 100% cho người thắng và 0% cho người thua. |
| TC08 | PASS. Kiểm thử C# xác nhận 499 EXP là cấp 1, 500 EXP là cấp 2. Trên website, người chơi QA đạt đúng 500 EXP và bảng cấp độ hiển thị cấp 2. |
| TC09 | PASS. Một lần gửi hợp lệ tạo đúng một bản ghi. |
| TC10 | PASS. Hai bảng xếp hạng phản ánh điểm, EXP và thống kê mới. |
| TC11 | PASS. Sau trận đầu, hai người chơi dẫn đầu lần lượt hạng 1 và 2. Mười hồ sơ mẫu có cùng thành tích và cùng mốc nên đồng hạng 3. |
| TC12 | PASS. Lịch sử hiển thị đúng đối thủ, kết quả và mức thưởng đối chiếu với SQL. Trang lịch sử rỗng hiển thị thông báo trước khi người chơi có trận. |
| TC13 | PASS. Khách truy cập lịch sử hoặc mô phỏng được chuyển đến đăng nhập. |
| TC14 | PASS. Phiên B không thấy trận riêng giữa A và C khi mở lịch sử có ID của A. Trang lịch sử của A không hiển thị email đối thủ C. |
| TC15 | PASS. Đối chiếu dữ liệu SQL trước và sau khi khởi động lại website. Trận và thành tích giữ nguyên, bảng xếp hạng vẫn hiển thị. |
| TC16 | PASS. Gửi lại tuần tự và gửi song song cùng một biểu mẫu trả về cùng mã trận. Với yêu cầu khác nhau chạy đồng thời trên cùng người chơi, tổng thành tích đối soát đúng với số trận trong SQL. |
| TC17 | PASS. Kiểm thử có chủ đích trên SQL Server cho phép lệnh ghi đầu tiên hoàn thành rồi làm lệnh thứ hai lỗi. Sau rollback, số trận, chỉ số và mốc UTC của toàn bộ người chơi giữ nguyên. |
| TC18 | PASS trên cả hai bảng. Bảng tổng điểm đã mở tự nhận thống kê từ một trận đã lưu. Kiểm thử bổ sung xác nhận bảng cấp độ đang mở tự hiển thị cấp 2 và 500 EXP mà không tải lại trang. |

## Kiểm thử bổ sung bảng cấp độ

Playwright trên Edge mở bảng cấp độ tại `http://localhost:5087`. Hai yêu cầu định kỳ đến endpoint cấp độ trả HTTP 200 và chỉ chứa các dòng của bảng. Playwright chèn dòng mô phỏng cấp 2 và 500 EXP vào phản hồi thứ hai. Dòng mới xuất hiện trên trang đang mở, URL giữ nguyên và không có lỗi JavaScript. Phép thử chỉ thay đổi phản hồi trong trình duyệt. Số dòng và checksum SQL của `Players`, `Matches` và `AspNetUsers` khớp trước và sau kiểm thử.

## Dữ liệu và nhật ký kiểm thử

- Kiểm tra bàn giao qua SQL ghi nhận 19 người chơi, gồm 10 hồ sơ mẫu, hai hồ sơ QA Chat 05, sáu hồ sơ QA Chat 06 và một hồ sơ có sẵn khác. Bảng `Matches` có chín trận.

- Trước khi thử, database có 13 người chơi, trong đó có 10 hồ sơ mẫu và hai tài khoản QA từ Chat 05. Bảng `Matches` trống.
- Lượt Playwright đầu đã tạo ba hồ sơ QA mang hậu tố `mwz41wpk` trước khi dừng vì script xử lý kết quả trống từ sqlcmd sai. Lượt chạy lại mang hậu tố `mwz42s83` tạo ba hồ sơ QA có ID 17, 18, 19 và chín trận có ID 2, 3, 5, 7, 8, 9, 10, 11, 12. Các hồ sơ QA được giữ lại theo yêu cầu.
- Khi gửi trùng đồng thời, EF ghi lỗi trùng chỉ mục SQL 2601. Dịch vụ đọc lại trận đã lưu và trả về thành công cho cả hai yêu cầu. Đối soát không thấy cộng trùng. Các lần lưu lỗi có thể để lại khoảng trống trong dãy mã trận tự tăng.
- Trang lịch sử lấy tên hiển thị hiện tại của đối thủ. Điểm và EXP theo mức cố định của v0.1.

## Kiểm tra lại khi demo

Chạy migration và website theo README. Mở bảng tổng điểm ở một cửa sổ và mô phỏng ở cửa sổ đã đăng nhập. Chọn hồ sơ của bạn làm một trong hai người tham gia nếu muốn xem trận trong lịch sử cá nhân. Sau khi gửi, đối chiếu điểm, EXP và kết quả giữa màn hình mô phỏng, hai bảng xếp hạng và lịch sử.

Cổng 5049 đã có tiến trình sử dụng trong lúc kiểm thử bàn giao. Lệnh chạy bản Release trên cổng 5050 trong README đã được kiểm tra thành công.
