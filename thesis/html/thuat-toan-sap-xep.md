# Sáu thuật toán sắp xếp — GameRanking

Các thuật toán nhận mảng `PublicPlayer[]` và một bộ so sánh. Mỗi thuật toán thay đổi thứ tự phần tử trong mảng được truyền vào. Cả hai bảng chính gọi Merge Sort trong `PlayerSorting`, sau đó `PlayerRanking` gán Dense Ranking.

| Thuật toán | Cách cài đặt | Tốt nhất | Trung bình | Xấu nhất | Bộ nhớ phụ |
| --- | --- | --- | --- | --- | --- |
| Bubble Sort | Đổi chỗ hai phần tử kề nhau. Dừng khi một lượt không đổi chỗ. | Θ(n) | Θ(n²) | Θ(n²) | O(1) |
| Selection Sort | Tìm phần tử đứng đầu phần chưa sắp xếp rồi đưa về đầu đoạn. | Θ(n²) | Θ(n²) | Θ(n²) | O(1) |
| Insertion Sort | Dịch các phần tử của đoạn đã sắp xếp để chèn phần tử tiếp theo. | Θ(n) | Θ(n²) | Θ(n²) | O(1) |
| Merge Sort | Chia đôi rồi trộn hai đoạn đã sắp xếp. Dùng một vùng đệm chung. | Θ(n log n) | Θ(n log n) | Θ(n log n) | O(n), gồm vùng đệm và ngăn xếp O(log n) |
| Quick Sort | Chọn phần tử giữa làm chốt. Phân hoạch hai phía, đệ quy đoạn nhỏ hơn và lặp trên đoạn còn lại. | Θ(n log n) | Θ(n log n) | Θ(n²) | O(log n) ngăn xếp cả ở trường hợp xấu nhất |
| Heap Sort | Tạo đống từ dưới lên, lần lượt đưa gốc về cuối đoạn rồi khôi phục đống. | Θ(n log n) với các khóa hiển thị phân biệt | Θ(n log n) | Θ(n log n) | O(1) |

Heap Sort xây đống trong O(n). Khi bộ so sánh coi mọi khóa bằng nhau, thao tác khôi phục dừng ngay và phiên bản này có thể chạy Θ(n). Trong đầu vào benchmark, `PlayerId` phân biệt các khóa hiển thị.

## Tiêu chí và gán hạng

- Tổng điểm giảm dần, sau đó mốc đạt điểm tăng dần.
- Cấp độ giảm dần, sau đó EXP giảm dần và mốc đạt EXP tăng dần. Cấp độ được tính bằng `1 + TotalExp / 500` với phép chia nguyên trên EXP không âm.
- Bộ so sánh hiển thị dùng ID tăng dần sau khi khóa nghiệp vụ bằng nhau. Khi gán Dense Ranking, chỉ xét khóa nghiệp vụ. Ví dụ, hai người đầu đồng hạng cho dãy 1, 1, 2, 3.

## Cách thực nghiệm

`SortingBenchmark` tạo một mảng từ seed với ID giả lập, điểm và EXP độc lập, cùng hai mốc UTC. Hai hồ sơ có cùng thành tích và mốc để tạo đồng hạng. Chế độ nhiều giá trị trùng thu hẹp miền sinh. Chế độ có thứ tự chuẩn bị dữ liệu theo bộ so sánh trước phần đo.

Mỗi thuật toán được chạy khởi động trên bản sao tối đa 128 hồ sơ. Sau đó mỗi thuật toán chạy năm lần trên năm bản sao mới. Các lượt chạy tuần tự và luân phiên vị trí chạy. `Stopwatch` bắt đầu sau bước sao chép và dừng ngay khi thuật toán trả về. Phép đo gồm bộ nhớ được cấp phát bên trong thuật toán, kể cả vùng đệm Merge Sort. Sinh dữ liệu, sao chép, gán hạng, đối chiếu, truy vấn SQL và render trang đều nằm ngoài phép đo.

Kết quả tham chiếu dùng khóa LINQ độc lập cho thứ tự và hạng. So sánh toàn bộ dãy gồm hồ sơ và hạng để kiểm tra kích thước, phần tử bị thiếu hoặc lặp, thứ tự và đồng hạng. Mảng gốc được đối chiếu với bản chụp sau toàn bộ lượt thử. Các hồ sơ `PublicPlayer` bất biến nên sao chép mảng là đủ để tách thứ tự từng lượt.

Trang công khai dùng dữ liệu giả lập. Biểu mẫu có kiểm tra chống giả mạo yêu cầu, danh sách kích thước cố định và giới hạn một lượt benchmark tại mỗi tiến trình web.
