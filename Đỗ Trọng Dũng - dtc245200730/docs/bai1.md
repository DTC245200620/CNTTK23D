# Bài 1 – Tổng quan hệ thống và MEP
Họ tên: Đỗ Trọng Dũng
MSSV: DTC245200730
Lớp: CNTT K23D
 
## 1. Cài đặt
<img width="1482" height="762" alt="{4999394D-830A-4CD5-B095-D07B137D874D}" src="https://github.com/user-attachments/assets/d1085996-d790-49da-8f32-862031f39ef1" />
 
## 2. MepDemo (Bước 7)
- Trả lời 3 câu hỏi; mỗi MEP 1 câu mô tả
 
## 3. BasicWebApi (Bước 8)
- Ảnh chụp REST Client: 200, 201, 400
Ảnh request 200
<img width="1920" height="1140" alt="{CEBEB182-17C3-4B13-8A8F-0B07B702724A}" src="https://github.com/user-attachments/assets/c8db4204-6bc2-4335-bdbf-d96838c1aa43" />
<img width="1920" height="1140" alt="{1DC020B7-77A8-4303-BE02-4D2B59864B69}" src="https://github.com/user-attachments/assets/ae9eac63-3023-4c35-bfb5-165563ad38d3" />Ảnh request 201
<img width="1920" height="1140" alt="{412631B3-F767-41BE-8B54-343EFA68BE2A}" src="https://github.com/user-attachments/assets/a5d24b30-13d5-4f12-bd0a-0bb86763bd3e" />Ảnh request 400
<img width="1920" height="1140" alt="{FAC67DF3-5B7F-4A42-BEF4-508E0BE85257}" src="https://github.com/user-attachments/assets/a289df59-a620-4879-8f17-a8e8a7440265" />

- Trả lời 3 câu hỏi mục 8.4
 
## 4. MiniDistributedSystem (Bước 9)
- Ảnh chụp 2 terminal API + Client
<img width="1920" height="1140" alt="{4489A74F-4AB3-4F88-96DA-93B719C91A76}" src="https://github.com/user-attachments/assets/91e6939f-9180-422a-b342-826a3b89ca6b" />
<img width="1920" height="1140" alt="{8858C60C-2031-4D69-A4D8-F70CF2BC016D}" src="https://github.com/user-attachments/assets/2d8692d1-a7a9-4022-a873-c90bc723d9ea" />

- Trả lời 3 câu hỏi
 
## 5. Thí nghiệm lỗi (Bước 10)
Làm trên `MiniDistributedSystem`; sau mỗi thí nghiệm đã trả code về như cũ.

| # | Dự đoán | Thực tế | Đúng/Sai | Giải thích |
|---|---------|---------|----------|------------|
| **a**<br>Thêm bài tập 3 vào `assignments` của API | _(ghi dự đoán trước khi chạy)_ | Không sửa client mà client vẫn in ra 3 bài tập (`- 3: Bai tap 3`), sau đó nộp bài bình thường (`Trang thai: received`). | | Client **không cần sửa**. Client không gắn cứng số bài tập: nó đọc mảng JSON thành `List<Assignment>` rồi `foreach`, nên API trả bao nhiêu phần tử thì in bấy nhiêu. Thêm bài tập chỉ thay đổi **dữ liệu**, còn **hợp đồng dữ liệu** (hình dạng JSON `{ id, title, description }`) vẫn giữ nguyên. Client chỉ phải sửa khi hợp đồng thay đổi (đổi tên, kiểu hay ý nghĩa của trường). |
| **b**<br>Tắt API (Ctrl+C) rồi chạy client | _(ghi dự đoán trước khi chạy)_ | Client dừng ngay ở request đầu tiên (`GET /api/assignments`) với lỗi `HttpRequestException: No connection could be made because the target machine actively refused it`, bên trong là `SocketException (10061)`. | | Đây là lỗi kết nối: không có tiến trình nào nghe ở cổng đó nên hệ điều hành từ chối kết nối. Gọi qua mạng có thể thất bại vì client và API là **hai tiến trình độc lập** nói chuyện qua mạng: bên kia có thể đã tắt, bị treo, quá tải hoặc sai địa chỉ, mạng có thể đứt hay chậm, và bên gọi không biết trước điều đó. Gọi một hàm trong cùng chương trình thì hàm nằm cùng tiến trình, cùng bộ nhớ và đã được trình biên dịch kiểm tra là có tồn tại, nên lời gọi luôn tới được hàm. Nếu chương trình chết thì bên gọi cũng chết theo, không có chuyện "một bên còn, một bên mất". |
| **c**<br>Đổi `StudentCode: "SV001"` thành `StudentCode: ""` | _(ghi dự đoán trước khi chạy)_ | Client vẫn in danh sách bài tập, nhưng khi nộp bài thì API trả **400 Bad Request** với body `{"message":"Du lieu nop bai khong hop le"}`. Client dừng ở `EnsureSuccessStatusCode()` với lỗi `HttpRequestException: Response status code does not indicate success: 400 (Bad Request).` | | `EnsureSuccessStatusCode()` kiểm tra mã trạng thái có thuộc nhóm 2xx không, nếu không thì ném exception để client không đọc nhầm body lỗi như thể là kết quả. Khi thử bỏ dòng này, client không báo lỗi mà in `Ma luot nop: 00000000-0000-0000-0000-000000000000` với `Trang thai:` rỗng, tức là lỗi bị "nuốt" âm thầm. Máy chủ phải tự kiểm tra dữ liệu vì nó không kiểm soát được ai gửi request: client cũ, client có lỗi, `curl`/REST Client hay kẻ xấu đều gửi thẳng tới API được. Kiểm tra ở client chỉ giúp người dùng tiện hơn; server mới là chốt chặn cuối cùng bảo vệ dữ liệu. |
| **d**<br>Chạy API ở cổng 5050, client vẫn gọi 5000 | _(ghi dự đoán trước khi chạy)_ | Client lỗi giống thí nghiệm b (`HttpRequestException ... actively refused it`) vì không có API nào ở cổng 5000. Sau khi sửa bằng cách truyền địa chỉ qua tham số dòng lệnh `dotnet run --project src/Submission.Client -- http://localhost:5050`, client chạy bình thường và in `Trang thai: received`. | | Sửa mà **không sửa code**: client đọc địa chỉ API từ `args[0]` (không có mới dùng mặc định `http://localhost:5000`), nên chỉ cần truyền `-- http://localhost:5050`. Cách khác là chạy lại API ở đúng cổng client đang gọi (`--urls http://localhost:5000`). Địa chỉ API là **cấu hình**, được tách khỏi code, nên đổi môi trường không phải sửa và biên dịch lại. Lưu ý: nếu cổng 5000 đang có một tiến trình khác nghe (ví dụ một API khác vẫn còn chạy), client sẽ gọi nhầm vào đó mà không hề báo lỗi. |
 
## 6. Dự án EduSubmit tự làm (Bước 11-12)
- Ảnh chụp kết quả
<img width="1920" height="1140" alt="{68CA4620-D9A7-420E-934C-BD624DC60728}" src="https://github.com/user-attachments/assets/8b192831-1122-4721-9998-762611da21b5" />

- Phần phải xem code mẫu
Trong lúc viết có đối chiếu code mẫu MiniDistributedSystem/src (Submission.Api/Program.cs, Submission.Client/Program.cs) và file requests.http mẫu để giữ đúng hợp đồng dữ liệu (3 record), cách kiểm tra dữ liệu trả 400 và cách trả Results.Created(...) kèm header Location.
Phần Bước 12 không có trong code mẫu:
API lưu lượt nộp vào List<SubmissionResult> (có lock vì nhiều request có thể chạy song song).
Thêm endpoint GET /api/submissions/{id:guid}: tìm bằng FirstOrDefault, không có thì trả Results.NotFound().
Client đọc lại lượt nộp qua header Location mà API trả về.
Thêm 2 request 200/404 vào requests.http; request 200 lấy id từ request nộp bài bằng # @name createSubmission.

- Luồng thông điệp:
Client --(HTTP GET /api/assignments)------> Api : request-response (200 + danh sách bài tập)
Client --(HTTP POST /api/submissions)-----> Api : request-response (201 Created + Location; 400 nếu dữ liệu sai)
Client --(HTTP GET /api/submissions/{id})-> Api : request-response (200 + lượt nộp; 404 nếu không có) – Bước 12
Cả 3 đều là request-response: client await và chờ phản hồi rồi mới làm bước tiếp theo.
POST tuy là lệnh ghi dữ liệu nhưng không phải one-way, vì client chờ mã 201 và đọc mã lượt nộp trong body để dùng cho request sau.
Vì phải chờ phản hồi nên client phụ thuộc vào việc API đang chạy: nếu API tắt, client dừng ngay ở request đầu tiên (GET /api/assignments) với lỗi HttpRequestException: No connection could be made because the target machine actively refused it.
 
## 7. Khó khăn gặp phải và cách giải quyết
Không có.
