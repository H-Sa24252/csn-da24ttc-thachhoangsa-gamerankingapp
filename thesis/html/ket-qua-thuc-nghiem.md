# Kết quả thực nghiệm — GameRanking v0.1

## Bộ đo ban đầu của Codex

Đo ngày 27/11/2026 trên website tại `http://localhost:5087/SortingBenchmark`. Bản Release, .NET SDK 10.0.403, target net10.0, EF Core 10.0.15. Máy Windows dùng Intel Core i7-13620H, 10 nhân, 16 luồng và khoảng 15,71 GiB RAM vật lý. Website demo cũ vẫn chạy trên máy. Các tính năng khác của hệ điều hành và tải nền không được cô lập.

Mỗi dòng là một lần gửi biểu mẫu **So sánh cả sáu** với dữ liệu ngẫu nhiên và seed 42. Mỗi ô là trung vị của năm lần chạy, tính theo mili giây. Sáu thuật toán trong từng dòng nhận các bản sao của đúng một mảng đầu vào. Cả 36 kết quả đều đạt đối chiếu.

| Tiêu chí | Người chơi | Bubble | Selection | Insertion | Merge | Quick | Heap |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Tổng điểm | 100 | 0,2120 | 0,1461 | 0,1430 | 0,0465 | 0,0646 | 0,0756 |
| Tổng điểm | 1.000 | 8,2909 | 4,1731 | 2,6398 | 0,4951 | 0,5842 | 0,7146 |
| Tổng điểm | 5.000 | 181,6468 | 89,7028 | 48,7705 | 2,4980 | 2,6198 | 2,4395 |
| Cấp độ | 100 | 0,1238 | 0,0839 | 0,0705 | 0,0144 | 0,0184 | 0,0214 |
| Cấp độ | 1.000 | 10,2731 | 5,7804 | 2,7142 | 0,2614 | 0,3053 | 0,3491 |
| Cấp độ | 5.000 | 159,9428 | 59,1954 | 30,9692 | 1,0734 | 1,1025 | 1,3728 |

### Nhận xét

Ở 5.000 người chơi, Bubble, Selection và Insertion tăng thời gian rõ rệt so với Merge, Quick và Heap. Merge có trung vị thấp nhất ở năm cấu hình đã đo. Heap thấp nhất ở cấu hình 5.000 người chơi theo tổng điểm, chênh khoảng 0,06 ms so với Merge.

Các cấu hình chạy theo thứ tự trong bảng, trên cùng tiến trình web và một seed. Việc phân lịch CPU của hệ điều hành, thu gom rác và tối ưu JIT theo tầng có thể ảnh hưởng số liệu, nhất là khi chênh lệch nhỏ. Các số trên mô tả lượt đo này. So sánh tổng quát hơn cần thêm seed, nhiều phiên độc lập và nhiều kiểu đầu vào.

Đã chạy thêm Merge Sort riêng trên 10.000 người chơi có thứ tự ngược và so sánh cả sáu trên 500 người chơi nhiều giá trị trùng. Hai lượt đều đạt kiểm tra đúng đắn.

[Phương pháp và độ phức tạp](thuat-toan-sap-xep.md) · [Kiểm thử và đối chiếu SQL](kiem-thu-chat-07.md)

## Bộ đo nghiệm thu của chủ dự án

Chủ dự án thực hiện ngày 28/11/2026 trên bản Release tại `http://localhost:5050`, dùng SDK 10.0.403 và target net10.0. Biên bản chưa ghi CPU, RAM hay tải hệ thống của lượt đo này. Các giá trị dưới đây là trung vị của năm lần sắp xếp, tính theo ms. Tất cả cấu hình dùng seed 42.

| Mã | Tiêu chí | Số người chơi | Dữ liệu |
| --- | --- | ---: | --- |
| TC03 | Tổng điểm | 1.000 | Ngẫu nhiên |
| TC04 | Cấp độ | 1.000 | Ngẫu nhiên |
| TC05 | Tổng điểm | 1.000 | Đã sắp xếp |
| TC06 | Tổng điểm | 1.000 | Thứ tự ngược |
| TC07 | Tổng điểm | 1.000 | Nhiều giá trị trùng |
| TC09 | Tổng điểm | 10.000 | Ngẫu nhiên |

| Thuật toán | TC03 | TC04 | TC05 | TC06 | TC07 | TC09 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Bubble Sort | 18.0278 | 25.2695 | 0.0844 | 7.2660 | 9.9985 | 1135.3722 |
| Selection Sort | 9.6315 | 13.6512 | 5.0616 | 7.1828 | 7.9764 | 373.0471 |
| Insertion Sort | 5.8423 | 6.4548 | 0.0811 | 5.5250 | 3.4897 | 219.2118 |
| Merge Sort | 0.9311 | 1.2573 | 0.2955 | 0.1717 | 0.4038 | 4.3431 |
| Quick Sort | 1.0926 | 1.4313 | 0.7327 | 0.1471 | 0.4240 | 3.8620 |
| Heap Sort | 1.5822 | 2.0748 | 1.0931 | 0.3778 | 0.5222 | 5.1272 |

Cả 36 kết quả trong bảng đều đạt đối chiếu. Ở TC02, Quick Sort chạy riêng với 100 người chơi, tổng điểm, dữ liệu ngẫu nhiên và seed 42, đạt đối chiếu với trung vị **0.1873 ms**. Ở TC09, chủ dự án ghi nhận tổng thời gian chờ trên website khoảng mười mấy giây. Thời gian chờ bao gồm cả các lượt đo và xử lý ngoài khoảng đo sắp xếp.

Merge Sort có trung vị thấp nhất ở 3/6 cấu hình, Quick Sort ở 2/6 và Insertion Sort ở 1/6. Bubble và Insertion giảm rõ thời gian trên dữ liệu đã sắp xếp. Khi tăng từ 1.000 lên 10.000 hồ sơ ngẫu nhiên theo tổng điểm, thời gian của nhóm Merge, Quick và Heap tăng ít hơn nhóm Bubble, Selection và Insertion. Thời gian dao động giữa các phiên và dữ liệu hiện có chưa đủ để xác định nguyên nhân của mọi chênh lệch hoặc kết luận một thuật toán luôn nhanh nhất. Hai bảng chính tiếp tục dùng Merge Sort.

## Bộ đo nghiệm thu Chat 08 của chủ dự án

Chủ dự án xác nhận kết quả ngày 28/11/2026. Cả hai ca dùng dữ liệu ngẫu nhiên, seed 42. Mỗi giá trị là trung vị của năm lượt đo riêng phần sắp xếp.

**NT08-08.** Quick Sort, tổng điểm, 100 người chơi, trung vị **0,0560 ms**, đạt đối chiếu. SQL giữ 21 người chơi và 15 trận.

**NT08-09.** So sánh cả sáu thuật toán trên 1.000 người chơi.

| Thuật toán | Tổng điểm (ms) | Cấp độ (ms) |
| --- | ---: | ---: |
| Bubble Sort | 10,9071 | 14,3139 |
| Selection Sort | 3,8214 | 5,8164 |
| Insertion Sort | 2,3200 | 2,9457 |
| Merge Sort | 0,3092 | 0,3710 |
| Quick Sort | 0,8740 | 0,3892 |
| Heap Sort | 1,3269 | 1,2488 |

Cả 12 kết quả đạt đối chiếu. Theo nghiệm thu thủ công, SQL giữ 21 người chơi, 15 trận và thành tích toàn bộ người chơi giữ nguyên. Merge Sort có trung vị thấp nhất ở cả hai tiêu chí trong lượt này và tiếp tục là mặc định. Các số đo chỉ mô tả cấu hình và lượt chạy này. Chênh lệch với Chat 07 chưa đủ để quy cho thay đổi hiệu năng của mã nguồn.
