# Bài 1 – Tổng quan hệ thống và MEP
Họ tên: Đỗ Trọng Dũng
MSSV: DTC245200730
Lớp: CNTT K23D
 
## 1. Cài đặt
<img width="1482" height="762" alt="{4999394D-830A-4CD5-B095-D07B137D874D}" src="https://github.com/user-attachments/assets/d1085996-d790-49da-8f32-862031f39ef1" />
 
## 2. MepDemo (Bước 7)
Chạy `dotnet run`, `dotnet run -- 1`, `dotnet run -- 3`, `dotnet run -- 4` trong thư mục `MepDemo`.

1. Ở MEP 3, mỗi bài nộp sub-x được mấy worker xử lý?  
   Mỗi bài chỉ do 1 worker chấm. Có 2 worker (Worker-A, Worker-B) cùng lấy việc từ một hàng đợi, bài nào worker này lấy rồi thì worker kia không lấy nữa. Lần chạy thử: Worker-A chấm sub-1 và sub-4, Worker-B chấm sub-2 và sub-3. Chạy lại thì có thể chia khác đi nhưng không có bài nào bị chấm 2 lần.
2. Ở MEP 4, một sự kiện được gửi tới mấy dịch vụ?  
   Tới cả 3 dịch vụ: ScoreService (lưu điểm), NotificationService (báo cho sinh viên) và StatsService (cập nhật thống kê). Worker chỉ phát sự kiện `SubmissionGraded` 1 lần, bus gửi cho tất cả các bên đã đăng ký. Khác với MEP 3 là ở MEP 3 mỗi việc chỉ 1 worker làm, còn ở đây bên nào đăng ký cũng nhận được.
3. Mỗi MEP 1 câu mô tả:
   - Request–response: client gửi yêu cầu rồi chờ server trả lời mới làm tiếp, trong demo chờ quá 1 giây thì client tự báo timeout.
   - One-way: bên gửi gửi log xong là làm việc khác luôn, không chờ AuditLog ghi xong.
   - Queue-based: API đẩy việc vào hàng đợi và trả ngay `processing`, các worker lấy ra xử lý dần, mỗi việc chỉ một worker làm.
   - Publish–subscribe: một sự kiện phát ra thì mọi bên đã đăng ký đều nhận, bên phát không cần biết ai nhận.
   - Streaming: client mở một lần rồi nhận cập nhật liên tục (`received` → `processing 30%` → `processing 70%` → `graded: 8.5`), không phải gửi yêu cầu mới cho mỗi lần cập nhật.
 
## 3. BasicWebApi (Bước 8)
(chèn ảnh chụp REST Client: 200, 201, 400)

Trả lời câu hỏi mục 8.4:
1. Hai dòng `builder.Services.AddSingleton…` và `var app = builder.Build();`, dòng nào phải đứng trước? Vì sao?  
   `AddSingleton` phải đứng trước. Dịch vụ phải được đăng ký vào `builder.Services` trước khi gọi `Build()`, vì lúc `Build()` danh sách dịch vụ được chốt lại để tạo ra `app`. Build xong rồi mới đăng ký thì sẽ bị lỗi, và endpoint `POST /greetings` cũng không lấy được `GreetingService` để dùng.
2. Trong `/hello/{name}?language=en`, tham số `name` lấy từ đâu, `language` lấy từ đâu?  
   `name` lấy từ route, tức là đoạn `{name}` trên đường dẫn (`/hello/An` thì `name = "An"`). `language` lấy từ query string, là phần sau dấu `?`. Minimal API dựa vào tên tham số: tên nào trùng với `{...}` trong route thì lấy từ route, còn lại thì lấy từ query. `language` khai báo `string?` nên không truyền cũng được, lúc đó API trả `Xin chào, An!`.
3. Vì sao POST hợp lệ trả `Results.Created` (201) chứ không phải `Results.Ok` (200)?  
   Vì POST này tạo ra một dữ liệu mới (lời chào có Id). Mã 201 nói rõ là đã tạo mới, còn 200 chỉ báo thành công chung chung. `Results.Created` còn gửi kèm header `Location: /greetings/1` cho biết dữ liệu vừa tạo nằm ở đâu, client có thể dùng địa chỉ này để đọc lại.
 
## 4. MiniDistributedSystem (Bước 9)
(chèn ảnh chụp 2 terminal API + Client)

1. Client gọi những endpoint nào của API? Mỗi lần gọi là một cặp request–response gồm method, URL, mã trạng thái gì?  
   Client gọi 2 endpoint:

   | Method | URL | Mã trạng thái |
   |--------|-----|---------------|
   | GET | `/api/assignments` | 200 OK, body là mảng bài tập |
   | POST | `/api/submissions` (body JSON gồm `assignmentId`, `studentCode`, `repositoryUrl`) | 201 Created kèm header `Location`; dữ liệu sai thì 400 Bad Request |

   `GET /` có trong API nhưng client không gọi, endpoint này chỉ để mở bằng trình duyệt hoặc `requests.http` xem API có đang chạy không.
2. Client có tham chiếu (dùng chung) code của API không? Hai bên "thống nhất" với nhau bằng gì?  
   Không. `Submission.Client.csproj` không có `ProjectReference` nào tới `Submission.Api`, client tự khai báo lại 3 record `Assignment`, `CreateSubmissionRequest`, `SubmissionResult` giống bên API. Hai bên thống nhất với nhau qua hợp đồng dữ liệu: method, URL của endpoint và dạng JSON (tên trường, kiểu dữ liệu). Chỉ cần JSON khớp nhau thì client viết bằng ngôn ngữ khác cũng gọi được API.
3. Vì sao địa chỉ API được truyền qua tham số `-- http://localhost:5000` mà không viết cứng trong code?  
   Vì địa chỉ API thay đổi theo môi trường: chạy trên máy mình là `localhost:5000`, máy khác có thể dùng cổng khác, đưa lên server thì lại là địa chỉ khác. Truyền qua tham số thì đổi địa chỉ không phải sửa code rồi build lại (giống thí nghiệm d ở mục 5). Không truyền thì client dùng mặc định `http://localhost:5000`.
 
## 5. Thí nghiệm lỗi (Bước 10)
Làm trên `MiniDistributedSystem`, xong mỗi thí nghiệm thì trả code về như cũ.

| # | Dự đoán | Thực tế | Đúng/Sai | Giải thích |
|---|---------|---------|----------|------------|
| a<br>Thêm bài tập 3 vào `assignments` của API | Client không cần sửa, chạy lại sẽ in ra 3 bài tập | Không sửa gì ở client, client vẫn in đủ 3 bài (`- 3: Bai tap 3`) và nộp bài bình thường (`Trang thai: received`) | Đúng | Client không cần sửa. Client đọc JSON vào `List<Assignment>` rồi dùng `foreach` nên API trả về bao nhiêu bài thì in bấy nhiêu. Thêm bài tập chỉ là thêm dữ liệu, còn dạng JSON `{ id, title, description }` vẫn giữ nguyên. Khi nào API đổi tên hoặc đổi kiểu của trường thì client mới phải sửa theo. |
| b<br>Tắt API (Ctrl+C) rồi chạy client | Client báo lỗi không kết nối được tới API | Client dừng ngay ở `GET /api/assignments`, lỗi `HttpRequestException: No connection could be made because the target machine actively refused it` (bên trong là `SocketException (10061)`) | Đúng | Đây là lỗi kết nối: API đã tắt nên không có chương trình nào nghe ở cổng 5000, máy từ chối kết nối. Client và API là 2 tiến trình riêng, nói chuyện với nhau qua mạng, nên bên kia có thể bị tắt, bị treo, sai địa chỉ hoặc mạng bị đứt mà client không biết trước được. Còn gọi hàm trong cùng chương trình thì hàm nằm sẵn trong cùng tiến trình, lúc biên dịch đã kiểm tra là có hàm đó nên lần gọi nào cũng tới được. |
| c<br>Đổi `StudentCode: "SV001"` thành `StudentCode: ""` | API trả về lỗi 400 và client báo lỗi | Client vẫn in danh sách bài tập, đến lúc nộp bài thì API trả `400 Bad Request`, body `{"message":"Du lieu nop bai khong hop le"}`. Client dừng ở `EnsureSuccessStatusCode()` với lỗi `Response status code does not indicate success: 400 (Bad Request).` | Đúng | `EnsureSuccessStatusCode()` kiểm tra mã trả về có phải 2xx không, nếu không thì ném exception. Thử bỏ dòng này đi thì client không báo lỗi gì mà in ra `Ma luot nop: 00000000-0000-0000-0000-000000000000` và `Trang thai:` để trống, nghĩa là lỗi bị bỏ qua mà không ai biết. Máy chủ phải tự kiểm tra dữ liệu vì ai cũng gửi request tới API được (client khác, `curl`, REST Client...), không phải lúc nào cũng là client của mình. Client có kiểm tra thì cũng chỉ giúp người dùng tiện hơn thôi. |
| d<br>Chạy API ở cổng 5050, client vẫn gọi 5000 | Client lỗi không kết nối được, giống thí nghiệm b | Client báo lỗi giống b (`HttpRequestException ... actively refused it`) vì cổng 5000 không có API nào chạy. Chạy lại client bằng `dotnet run --project src/Submission.Client -- http://localhost:5050` thì chạy bình thường, ra `Trang thai: received` | Đúng | Sửa bằng cách truyền địa chỉ API lúc chạy client, không sửa code: client lấy địa chỉ từ `args[0]`, không truyền thì mới dùng `http://localhost:5000`. Cách khác là chạy lại API ở cổng 5000 (`--urls http://localhost:5000`). Địa chỉ API là cấu hình nên để ngoài code, đổi máy hay đổi cổng không phải sửa code rồi build lại. |
 
## 6. Dự án EduSubmit tự làm (Bước 11-12)
### Ảnh chụp kết quả
(chèn ảnh chụp 2 terminal: Submission.Api và Submission.Client)

Kết quả Submission.Client (`dotnet run --project src/Submission.Client -- http://localhost:5000`):
```
Danh sach bai tap:
- 1: Bai tap 1 (Tao API dau tien)
- 2: Bai tap 2 (Goi API tu client)
Da nop bai. Ma luot nop: c8624d1b-6987-4598-9977-49799450ba61
Trang thai: received
Doc lai luot nop tu API:
- Id: c8624d1b-6987-4598-9977-49799450ba61
- Bai tap: 1
- Sinh vien: DTC245200730
- Trang thai: received
- Nhan luc: 2026-11-06 03:56:46 UTC
```

Kết quả `requests.http`:

| # | Request | Kết quả |
|---|---------|---------|
| 1 | `GET /` | 200 OK, `{ "service": "Submission.Api", "status": "running" }` |
| 2 | `GET /api/assignments` | 200 OK, mảng 2 bài tập |
| 3 | `POST /api/submissions` hợp lệ | 201 Created, header `Location: /api/submissions/{id}` |
| 4 | `POST /api/submissions` thiếu `studentCode` | 400 Bad Request |
| 5 | `GET /api/submissions/{id vừa tạo}` (Bước 12) | 200 OK, trả lại đúng lượt nộp |
| 6 | `GET /api/submissions/00000000-0000-0000-0000-000000000000` (Bước 12) | 404 Not Found |

### Phần nào phải xem code mẫu?
- Code Bước 11–12 được viết với sự hỗ trợ của trợ lý AI (Claude Code). Trong lúc viết có đối chiếu code mẫu `MiniDistributedSystem/src` (`Submission.Api/Program.cs`, `Submission.Client/Program.cs`) và file `requests.http` mẫu để giữ đúng hợp đồng dữ liệu (3 record), cách kiểm tra dữ liệu trả 400 và cách trả `Results.Created(...)` kèm header `Location`.
- Phần Bước 12 không có trong code mẫu:
  - API lưu lượt nộp vào `List<SubmissionResult>` (có `lock` vì nhiều request có thể chạy song song).
  - Thêm endpoint `GET /api/submissions/{id:guid}`: tìm bằng `FirstOrDefault`, không có thì trả `Results.NotFound()`.
  - Client đọc lại lượt nộp qua header `Location` mà API trả về.
  - Thêm 2 request 200/404 vào `requests.http`; request 200 lấy `id` từ request nộp bài bằng `# @name createSubmission`.

### Luồng thông điệp
```
Client --(HTTP GET /api/assignments)------> Api : request-response (200 + danh sách bài tập)
Client --(HTTP POST /api/submissions)-----> Api : request-response (201 Created + Location; 400 nếu dữ liệu sai)
Client --(HTTP GET /api/submissions/{id})-> Api : request-response (200 + lượt nộp; 404 nếu không có) – Bước 12
```
- Cả 3 đều là request-response: client `await` và chờ phản hồi rồi mới làm bước tiếp theo.
- `POST` tuy là lệnh ghi dữ liệu nhưng không phải one-way, vì client chờ mã 201 và đọc mã lượt nộp trong body để dùng cho request sau.
- Vì phải chờ phản hồi nên client phụ thuộc vào việc API đang chạy: nếu API tắt, client dừng ngay ở request đầu tiên (`GET /api/assignments`) với lỗi `HttpRequestException: No connection could be made because the target machine actively refused it`.
 
## 7. Khó khăn gặp phải và cách giải quyết
