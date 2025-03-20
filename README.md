# [FE] Student Management

## Cấu trúc source code
```
student_management_fe/
│
├── Program.cs                   
│
├── App.razor                   
│
├── Views/
│   ├── Pages/
│   ├── Layout/
│   └── Shared/
│
├── Models/                     
│
├── Services/                  
│
├── Authentication/              
│
└── wwwroot/
```

## Hướng dẫn cài đặt & chạy chương trình
- Sử dụng Visual Studio 2022
- Cài gói `ASP.NET and web development`
- Cần chạy source Backend trước khi chạy source này
- Chạy source bằng cách nhấn `F5` hoặc `Ctrl + F5`

## Lưu ý
- Tài khoản mặc định để log in là:
    ```
    username: admin
    password: admin
    ```
- Link trang web: `https://localhost:7088`

## Hình ảnh các chức năng
### Thêm sinh viên
<img src="./pics/add_student.gif" width=50%/>

### Xóa sinh viên
<img src="./pics/delete_student.gif" width=50%/>

### Cập nhật thông tin sinh viên
<img src="./pics/edit_student.gif" width=50%/>

### Tìm kiếm sinh viên
<img src="./pics/search_student.gif" width=50%/>

### Đổi tên & thêm mới khoa, chương trình, tình trạng sinh viên
<img src="./pics/faculty.gif" width=50%/>
<img src="./pics/program.gif" width=50%/>
<img src="./pics/status.gif" width=50%/>

### Tìm theo khoa (filter theo khoa)
<img src="./pics/filter.gif" width=50%/>

### Export từ Excel / Json
<img src="./pics/export.gif" width=50%/>

### Import từ Excel / Json
<img src="./pics/import.gif" width=50%/>

### Logging
<img src="./pics/logging.gif" width=50%/>


