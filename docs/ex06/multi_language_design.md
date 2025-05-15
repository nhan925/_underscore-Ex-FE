# Phương án thiết kế đa ngôn ngữ cho Ứng dụng Quản lý sinh viên

## Static Contents:
- Sử dụng file resx của .NET
- Có thể sử dụng thêm dịch tự động nếu ngôn ngữ không hỗ trợ sẵn

## Dynamic Contents:
- Với các đối tượng (bảng) có nội dung cần đa ngôn ngữ --> tạo thêm 1 bảng translation
- Khi user query sử dụng content negotiation (Accept-Language) để xác định ngôn ngữ
- Với các thao tác GET 
	+ Query kèm theo ngôn ngữ mà FE yêu cầu --> server dùng middleware để tách context ngôn ngữ ra
	+ Nếu không tồn tại bản dịch --> Sử dụng AI (local) để dịch và lưu vào bảng translation
- Với thao tác tạo, update 
	+ Chèn nội dung (EN) vào bảng chính và các bản dịch vào bảng translation
	+ Nếu nội dung từ FE không phải là EN --> dùng AI (local) để dịch sang EN
- Để giảm độ trễ và chi phí --> dùng LM Studio để chạy model local cùng máy host server BackEnd
	