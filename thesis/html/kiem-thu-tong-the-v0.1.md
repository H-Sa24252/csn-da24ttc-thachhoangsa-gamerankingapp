# Kiểm thử tổng thể GameRanking v0.1

Ngày kiểm thử 28/11/2026. Điểm bắt đầu là commit `1f67a8eb0291a0c95a439d735505c3237c26215f`, nhánh `main` đi trước `origin/main` ba commit theo ref cục bộ. Working tree ban đầu sạch.

## Kết quả trực tiếp của Chat 08

| Mã | Điều kiện và thao tác | Kết quả mong đợi | Thực tế và trạng thái |
| --- | --- | --- | --- |
| C08-01 | Gốc repository, SDK theo `global.json`, chạy build Release chuẩn | CLI chọn 10.0.403, build thành công | **PASS sau kiểm thử lại**. Lần đầu bị chặn vì thiếu 10.0.403. Sau khi cài bổ sung, CLI trả 10.0.403 và build Release có 0 lỗi, 0 cảnh báo. |
| C08-02 | Gọi MSBuild.dll 10.0.401 trực tiếp từ thư mục cha, build Release solution và project kiểm thử, đầu ra `bin/Chat08Verified` | Biên dịch thành công | **PASS phương án dự phòng**. Hai lượt build sau sửa giao diện đều 0 lỗi và 0 cảnh báo. |
| C08-03 | Windows Authentication, truy vấn `.\SQLDEV`, chạy bản Release trên localhost | SQL kết nối được, website phục vụ HTTP | **PASS**. GameRankingDemo có hai migration `20261127141429_InitialCreate` và `20261127150046_AddMatchRequestId`. Website phục vụ cổng 5050, QA chạy riêng cổng 5087. SQL trong sandbox lỗi tìm instance, chạy ngoài sandbox theo phê duyệt thành công. |
| C08-04 | Khách mở hồ sơ, lịch sử và mô phỏng, mở các trang công khai | Trang riêng chuyển đến đăng nhập, trang công khai tải được | **PASS** qua Edge. Rà soát mã nguồn xác nhận chính sách kiểm tra antiforgery toàn cục với biểu mẫu POST. |
| C08-05 | Đăng ký QA, mở lịch sử rỗng, thử cặp trùng và ID không tồn tại | Hồ sơ được tạo, lịch sử rỗng có thông báo, yêu cầu sai giữ nguyên SQL | **PASS** trên QA. |
| C08-06 | Mô phỏng với hai người chơi mới, đối chiếu SQL | Một thắng và một thua, đúng 50/100 và 5/20, cập nhật chỉ số và hai mốc, người khác giữ nguyên | **PASS** trên QA, gồm tổng trận và tỷ lệ 100%/0% ở trận đầu. SQL và web hiển thị cấp đúng khi đạt 540 EXP. |
| C08-07 | Mở lịch sử của người tham gia và xem từ tài khoản khác | Đúng đối thủ/thưởng, đúng phạm vi tài khoản | **PASS**. Tài khoản khác thấy trận của chính mình và không thấy trận riêng giữa hai hồ sơ còn lại. |
| C08-08 | Gửi lại tuần tự và song song cùng biểu mẫu, sau đó gửi hai yêu cầu khác nhau có người chơi chung (TC16) | Yêu cầu trùng dùng cùng mã trận, yêu cầu khác tạo trận riêng | **PASS** trên QA. Đối chiếu tổng chỉ số được ghi ở phần kết quả bổ sung bên dưới. |
| C08-09 | Mở hai bảng trước khi lưu trận (TC18) và kiểm tra dữ liệu vượt ngưỡng | Các dòng tự cập nhật ở tab đang hiển thị | **PASS** trên QA với cả hai bảng. Bảng tổng điểm cập nhật qua yêu cầu lấy dòng và bộ đếm tải trang giữ nguyên. Bảng cấp độ đang mở nhận EXP mới sau trận. |
| C08-10 | Build và chạy DLL của SortingChecks Release | Sáu thuật toán, hai bảng và Quick lớn đạt kỳ vọng | **PASS**. 72 ca thuật toán, 48 kết quả benchmark, hai bảng chính, hai ca Quick Sort 100.000 phần tử. Exit code 0. |
| C08-11 | Gọi bộ sinh đầu vào thật, seed 42 hai lần rồi seed 43, 100 phần tử cho hai tiêu chí × bốn dạng | Cùng seed có cùng hồ sơ và thứ tự, seed khác cho đầu vào khác | **PASS**. So sánh trực tiếp toàn bộ record theo thứ tự. |
| C08-12 | Bốn cấu hình qua MVC gồm Quick 100 ngẫu nhiên, cả sáu trên 1.000 đã sắp xếp, cả sáu trên 500 thứ tự ngược, Merge 5.000 nhiều trùng, thử ba đầu vào sai | Kết quả khớp tham chiếu, từ chối kích thước/seed/tiêu chí sai | **PASS**. 14 hàng đạt và ba lỗi biểu mẫu. 10.000 có trong danh sách và đã được thử trực tiếp ở Chat 07. |
| C08-13 | Trên GameRankingDemo, so 21 dòng của từng bảng trước và sau polling trong Edge | Thứ tự và nội dung từng bảng ổn định | **PASS** ở 1366×768 và 390×844. Dùng từng dòng để loại chênh lệch khoảng trắng giữa HTML toàn trang và partial. |
| C08-14 | SHA-256 toàn bộ cột SQL của Players và Matches trước/sau benchmark | Số dòng và SHA-256 khớp | **PASS**. SQL tạo JSON theo khóa chính với giá trị NULL, rồi băm SHA-256. Không có đăng ký hoặc trận xen giữa. |
| C08-15 | Chạy các trang công khai trên Edge ở hai kích thước nêu trên | Tiêu đề hiển thị, trang không tràn ngang | **PASS** theo kiểm tra DOM. Bảng dùng khung cuộn ngang riêng. Phạm vi giao diện riêng tư và nút menu có tại phần kết quả bổ sung. |

### Kết quả bổ sung trên GameRankingChat08QA

| Mã | Điều kiện và thao tác | Kết quả mong đợi | Thực tế và trạng thái |
| --- | --- | --- | --- |
| C08-16 | Dùng Edge trên bản Release sau khi sửa menu. Đăng ký, đổi tên, mở menu di động, đăng xuất, đăng nhập lại và mở hồ sơ với ID khác | Menu trở về trạng thái mở được, thông tin hồ sơ là của phiên hiện tại và tên mới lưu bền | **PASS**. Mở lại ba trang riêng tư ở cả 390×844 và 1366×768, không tràn ngang cấp trang. |
| C08-17 | EF dùng batch một lệnh trên CSDL QA, interceptor cho lệnh ghi thứ nhất chạy rồi làm lệnh thứ hai lỗi (TC17) | Rollback toàn bộ trận và thành tích | **PASS**. Interceptor ghi nhận đúng một lệnh ghi hoàn thành, lệnh thứ hai thất bại, hai dấu SHA-256 giữ nguyên. Chỉ tiến trình kiểm thử dùng interceptor. |
| C08-18 | Đối soát SQL sau các lần gửi trùng và yêu cầu đồng thời có người chơi chung | Số thắng, thua, điểm và EXP khớp các trận đã lưu | **PASS**. Không có hồ sơ sai lệch. |
| C08-19 | Dừng và khởi động lại tiến trình QA, đọc lại SQL và bảng tổng điểm | Ứng dụng phục vụ HTTP và dữ liệu giữ nguyên | **PASS**. Trước và sau có 14 người chơi, 8 trận và hai dấu SHA-256 khớp. Hai dấu băm của GameRankingDemo cũng khớp với lần đo benchmark. |
| C08-20 | Chạy kiểm tra ranh giới cấp độ C# | 499 EXP ứng với cấp 1, 500 EXP ứng với cấp 2 | **PASS**. |

### SHA-256 của GameRankingDemo

| Bảng | Số dòng trước và sau | SHA-256 trước và sau |
| --- | ---: | --- |
| Players | 21 | `5EDD12510632DA3BB2E3B88642C98825A0E1C64368D6434D054274FF5EA086EE` |
| Matches | 14 | `E1EDAF875069FFAFEB241567241AD1C50488BA7CD95A063C075A8895909C721B` |

### Tiêu chí xếp hạng và TC10

Merge Sort phục vụ hai bảng. Bảng tổng điểm dùng điểm giảm dần rồi mốc đạt điểm tăng dần. Bảng cấp độ dùng cấp độ giảm dần, EXP giảm dần rồi mốc đạt EXP tăng dần. Dense Ranking chỉ tăng hạng khi bộ khóa nghiệp vụ đổi. PlayerId ổn định vị trí hiển thị của những người đồng hạng. Một phép đối chiếu SQL độc lập theo hai bộ khóa trên dữ liệu hiện tại cho 0 vị trí khác nhau. Hai bảng có thể khác thứ tự với dữ liệu khác vì dùng hai bộ khóa. Ghi chú TC10 của chủ dự án không đủ để tái tạo chính xác trạng thái lịch sử. Phép thử ổn định kiểm tra từng bảng qua polling trên dữ liệu hiện tại.

### Phạm vi đo và giới hạn

Rà soát mã xác nhận mỗi cấu hình tạo một mảng gốc, warm-up trên tối đa 128 phần tử, năm lượt với bản sao riêng và trung vị. Stopwatch bao quanh lệnh sắp xếp gồm chi phí cấp phát bên trong thuật toán. Sinh dữ liệu, sao chép và đối chiếu LINQ nằm ngoài khoảng đo. Controller dùng một slot mỗi tiến trình và từ chối khi slot bận. Giới hạn đầu vào 10.000 giảm phạm vi chi phí, nhưng các thuật toán bậc hai vẫn làm tăng tải CPU, nhất là khi gọi lặp hoặc chạy nhiều tiến trình. Không có phép thử tải hàng loạt trong Chat 08.

Đã xem ảnh chụp Edge ở chiều rộng 390 và 1366 px cho trang chủ, đăng ký, đăng nhập, hai bảng và kết quả benchmark. Bảng xếp hạng ở màn nhỏ cần cuộn trong vùng bảng để tới các cột cuối. Các trang riêng tư, thông báo và trạng thái lịch sử rỗng được kiểm tra qua thao tác và DOM.

## Kiểm thử lại sau khi khôi phục SDK

Ngày 28/11/2026, xác minh SHA-512 của gói ZIP và EXE Windows x64 từ Microsoft. EXE có chữ ký Authenticode hợp lệ của Microsoft. Dùng ZIP để cài bổ sung vào thư mục dotnet hiện có qua quyền quản trị, giữ các bản 8.0.425, 10.0.401 và 11.0.100. Từ gốc repository, `dotnet --version` chọn 10.0.403. `dotnet --info` chạy đầy đủ ngoài sandbox. Dấu băm của `global.json` và toàn bộ tệp trong `scr/GameRanking.Web/bin/Chat08Verified/` giữ nguyên.

- `dotnet build GameRanking.slnx -c Release --no-restore` đạt với 0 lỗi và 0 cảnh báo.
- `dotnet run --project tests/GameRanking.SortingChecks -c Release` thoát mã 0. Đạt 72 ca thuật toán, 48 kết quả benchmark, hai bảng và hai ca Quick Sort lớn.
- Lượt chạy trong sandbox ghi NU1900 vì không lấy được dữ liệu kiểm tra lỗ hổng từ NuGet. Restore đúng các phụ thuộc hiện có và chạy lại ngoài sandbox thành công, không xuất hiện cảnh báo này.
- `dotnet run --project scr/GameRanking.Web -c Release --no-build --launch-profile http -- --urls http://localhost:5050` khởi động thành công. Sáu trang công khai trả HTTP 200. So SHA-256 của `Players` và `Matches` trước/sau khởi động cho thấy `GameRankingDemo` giữ 21 hồ sơ, 14 trận và `GameRankingChat08QA` giữ 14 hồ sơ, 8 trận.

## Kết quả lịch sử

- Chat 05 kiểm thử nền tảng tài khoản, hồ sơ và bảng xếp hạng. Xem [biên bản Chat 05](kiem-thu-gameranking-v0.1.md).
- Chat 06 có kết quả trước đây cho TC16 gửi trùng/đồng thời, TC17 rollback và TC18 cập nhật bảng. Xem [biên bản Chat 06](kiem-thu-chat-06.md).
- Chat 07 được chủ dự án nghiệm thu 12/12, gồm bộ kiểm thử tự động thoát mã 0. TC10 ghi 21 người chơi và 13 trận trước/sau benchmark. Trận ở TC11 nâng tổng số lên 14. Xem [biên bản Chat 07](kiem-thu-chat-07.md) và [hai bộ số liệu lịch sử](ket-qua-thuc-nghiem.md).

## Nghiệm thu thủ công của chủ dự án

Chủ dự án xác nhận ngày 28/11/2026 tất cả NT08-01 đến NT08-10 đạt. Đã đối chiếu riêng 20 dòng C08-01–C08-20 trong biên bản kỹ thuật, đều PASS sau khi kiểm thử lại C08-01. Các nhóm kiểm thử có thể chồng lấn và được báo cáo riêng.

| Mã | Nội dung | Kết quả |
| --- | --- | ---: |
| NT08-01 và NT08-02 | SDK, môi trường, build và khởi động | 12/12 |
| NT08-03 | Giao diện, điều hướng và responsive | 14/14 |
| NT08-04 | Đăng nhập và hồ sơ | 16/16 |
| NT08-05 | Mô phỏng trận đấu | 19/19 |
| NT08-06 | Hai bảng và Live Update, gồm kiểm tra bổ sung | 17/17 |
| NT08-07 | Lịch sử thi đấu | 17/17 |
| NT08-08 | Quick Sort với 100 người chơi | 15/15 |
| NT08-09 | So sánh sáu thuật toán | 14/14 |
| NT08-10 | Tổng duyệt demo | 15/15 |
| **Tổng** | **10/10 giai đoạn** | **139/139** |

- **NT08-05.** Trước ca, demo001 có 10 điểm, 40 EXP, 0 thắng, 2 thua. Trận 18 ghi demo001 thắng QA Player B. SQL theo nghiệm thu có 21 người chơi và 14 → 15 trận. Sau ca, demo001 có 60 điểm, 140 EXP, cấp 1, 1 thắng, 2 thua, 3 trận, tỷ lệ thắng 33,33%.
- **NT08-06.** Thứ tự và thứ hạng của cả 21 người chơi ở hai bảng khớp kết quả SQL đối chiếu, gồm nhóm đồng hạng 13. Hai bảng tự cập nhật sau trận, polling ổn định khi dữ liệu giữ nguyên và các phản hồi ghi nhận HTTP 200. QA Player B có 5 điểm, 20 EXP, cấp 1, 0 thắng, 1 thua.
- **NT08-07.** Lịch sử demo001 có 3 trận, trận mới nhất đứng trước. Điểm và EXP đối chiếu khớp hồ sơ. Quyền xem lịch sử giới hạn theo tài khoản và khách được yêu cầu đăng nhập.
- **NT08-08 và NT08-09.** Quick Sort 100 người chơi đạt trung vị 0,0560 ms. Cả 12 kết quả so sánh sáu thuật toán đạt. Số liệu chi tiết có tại [kết quả thực nghiệm](ket-qua-thuc-nghiem.md).
- **NT08-10.** Tổng duyệt 9 phút, đạt 15/15 và không ghi nhận lỗi nghiêm trọng làm gián đoạn. Chủ dự án đăng nhập, trình bày hồ sơ, mô phỏng một trận demo001 thắng, quan sát hai bảng tự cập nhật và lịch sử ghi trận mới. Chủ dự án chạy Quick Sort 100 người chơi, so sánh sáu thuật toán với 1.000 người chơi và giải thích cách đo, kiểm chứng và chọn Merge Sort. Số người chơi giữ 21, số trận tăng 15 → 16.

### Đối chiếu SQL sau tổng duyệt

Truy vấn chỉ đọc xác nhận `GameRankingDemo` có **21 người chơi, 16 trận** và `GameRankingChat08QA` có **14 người chơi, 8 trận**. Hồ sơ demo001 có **110 điểm, 240 EXP, cấp 1, 2 thắng, 2 thua, 4 trận, tỷ lệ thắng 50%**. Trận tổng duyệt có mã **19**, demo001 thắng **QA Player A Updated**, lưu lúc **2026-11-28 04:12:10.6069835 UTC** (11:12:10 ngày 28/11/2026 giờ Việt Nam). Mốc 14 trận ở phần kiểm thử kỹ thuật là ảnh chụp trước hai trận nghiệm thu.

### Thông báo JavaScript khi dùng Cốc Cốc

Chủ dự án xác nhận qua DevTools Sources rằng `Uncaught (in promise) undefined — onboarding.js:48` đến từ `Content scripts → top → Tải video & audio → onboarding.js`. Chưa có bằng chứng lỗi trực tiếp từ GameRanking và không ghi nhận ảnh hưởng đến các chức năng nghiệm thu. Nguyên nhân nội bộ của tiện ích chưa được phân tích. Không có ảnh minh chứng để Codex kiểm tra trực tiếp trong phiên này. Microsoft Edge là trình duyệt đề xuất cho demo.
