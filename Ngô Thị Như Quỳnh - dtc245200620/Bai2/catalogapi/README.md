# Catalog API – Phần 2

## Bước 9 – Thí nghiệm phân tán

Phiên kiểm thử dùng cổng `5101` vì cổng `5100` đang có một phiên Catalog API khác.

| Thí nghiệm | Kết quả quan sát |
| --- | --- |
| Chạy máy khách lần hai khi máy chủ vẫn chạy | Máy khách liệt kê 4 sản phẩm, tạo sản phẩm `[5]` và nhận `Location: /api/products/5`. |
| Dừng máy chủ rồi chạy máy khách | Máy khách báo `HttpRequestException`. Kết nối đến `localhost:5101` bị từ chối (`SocketException` 10061). |
| Khởi động lại máy chủ rồi gọi `GET /api/products` | API trả `200 OK` với 3 sản phẩm mẫu có ID `1`, `2`, `3`. |
| Chạy máy khách với cổng `5999` | Máy khách báo `HttpRequestException`. Kết nối đến `localhost:5999` bị từ chối (`SocketException` 10061). |

### Câu hỏi về hệ phân tán

- **A.** Hàm nội bộ chạy trong cùng tiến trình. Yêu cầu HTTP cần kết nối đến tiến trình máy chủ, nên kết nối có thể bị từ chối khi API dừng hoặc khi máy khách dùng sai cổng.
- **B.** Danh sách `products` nằm trong bộ nhớ của `Catalog.Api`. Các lần chạy máy khách đọc cùng trạng thái khi máy chủ còn chạy. Khởi động lại API tạo lại danh sách mẫu.
- **C.** Nếu đổi thuộc tính DTO thành `Ten` và sửa các chỗ tham chiếu tương ứng, mã máy khách vẫn biên dịch. Khi đọc JSON, `Ten` nhận `null` vì API gửi trường `name`.

## Bước 10 – Phiếu tự kiểm tra

| STT | Yêu cầu | Kết quả thực tế | Tự đánh giá |
| --- | --- | --- | --- |
| 1 | `GET /` | `200 OK`, JSON báo API đang chạy | ✓ |
| 2 | `GET /api/products` | `200 OK`, 3 sản phẩm mẫu | ✓ |
| 3 | `GET /api/products/1` | `200 OK`, sản phẩm `[1]` | ✓ |
| 4 | `GET /api/products/999` | `404 Not Found` với `ApiError` có mã `PRODUCT_NOT_FOUND` | ✓ |
| 5 | `GET /api/products/abc` | `404 Not Found`, body rỗng | ✓ |
| 6 | `POST` sản phẩm hợp lệ | `201 Created`, `Location: /api/products/4` | ✓ |
| 7 | `POST` sản phẩm có giá âm | `400 Bad Request` với `ApiError` có mã `INVALID_PRODUCT` | ✓ |
| 8 | `POST` thiếu `Content-Type` | `415 Unsupported Media Type` | ✓ |
| 9 | Máy khách chạy đủ 4 nấc | Liệt kê sản phẩm, tạo `[4]`, đọc chi tiết `[1]`, nhận lỗi `400` khi gửi dữ liệu sai | ✓ |
| 10 | Chạy máy khách khi máy chủ dừng | `HttpRequestException` do kết nối đến `localhost:5101` bị từ chối | ✓ |

`CatalogApi.slnx` build thành công với .NET SDK `10.0.403`, 0 cảnh báo và 0 lỗi. Có thể xem các mốc thực hành bằng `git log --oneline`.
