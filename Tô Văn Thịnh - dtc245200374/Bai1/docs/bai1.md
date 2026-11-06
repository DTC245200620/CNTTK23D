# Bài 1 — Thực hành MEP 3

Họ tên: Tô Văn Thịnh  
MSSV: DTC245200374  
Lớp: CNTT K23D

## Thực hiện

Chạy chương trình MepDemo bằng lệnh:

dotnet run -- 3

## Ảnh kết quả

![Kết quả chạy MEP 3](anh/mep3.png)

## Nhận xét

- Worker-A xử lý sub-1 và sub-4.
- Worker-B xử lý sub-2 và sub-3.
- Cả bốn bài đều được xử lý xong.
- Mỗi bài chỉ được một worker xử lý.
- Hai worker chia nhau công việc từ cùng hàng đợi.
- Việc phân công có thể thay đổi giữa các lần chạy.
