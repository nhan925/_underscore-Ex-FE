using Radzen.Blazor.Rendering;
using student_management_fe.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static ServiceStack.LicenseUtils;

namespace student_management_fe.Services;
public class CourseService
{
    private readonly AuthService _authService;

    public CourseService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<CourseModel>> GetAllCourses()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/course");
        var response = await _authService.SendRequestWithAuthAsync(request);
        return await response.Content.ReadFromJsonAsync<List<CourseModel>>() ?? new List<CourseModel>();
    }

    public async Task<CourseModel> AddCourse(CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/course")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Thêm khóa học không thành công! {errorContent}");
        }

        // Phân tích response theo cấu trúc API trả về
        var responseObj = await response.Content.ReadFromJsonAsync<ApiResponse<CourseModel>>();
        if (responseObj != null && responseObj.Data != null)
        {
            return responseObj.Data;
        }

        throw new Exception("Đã có lỗi xảy ra khi xử lý phản hồi từ server!");
    }

    // Lớp để parse response từ API
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public async Task<string> UpdateCourse(CourseModel course)
    {
        var json = JsonSerializer.Serialize(course);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/course")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _authService.SendRequestWithAuthAsync(request);

        // Đọc nội dung phản hồi
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Xử lý các loại lỗi khác nhau dựa trên status code
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // Cố gắng parse nội dung lỗi để lấy thông báo cụ thể
                try
                {
                    var errorObj = JsonSerializer.Deserialize<ApiErrorResponse>(content);
                    throw new Exception(errorObj?.Message ?? "Cập nhật khóa học không thành công: Dữ liệu không hợp lệ.");
                }
                catch (JsonException)
                {
                    throw new Exception($"Cập nhật khóa học không thành công: {content}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Không tìm thấy khóa học hoặc không có thay đổi nào được thực hiện.");
            }
            else
            {
                throw new Exception($"Cập nhật khóa học không thành công! Mã lỗi: {(int)response.StatusCode}");
            }
        }

        // Parse response để lấy thông báo
        try
        {
            var responseObj = JsonSerializer.Deserialize<ApiSuccessResponse>(content);
            return responseObj?.Message ?? "Cập nhật khóa học thành công";
        }
        catch (JsonException)
        {
            return "Cập nhật khóa học thành công";
        }
    }

    // Lớp để parse response error
    public class ApiErrorResponse
    {
        public string Message { get; set; }
    }

    // Lớp để parse response success
    public class ApiSuccessResponse
    {
        public string Message { get; set; }
    }

    public async Task<string> DeleteCourse(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/course/{id}");
        var response = await _authService.SendRequestWithAuthAsync(request);

        // Đọc nội dung phản hồi
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Cố gắng parse JSON để lấy thông báo lỗi chi tiết
            try
            {   
                var errorDetail = JsonSerializer.Deserialize<JsonElement>(content).GetProperty("details").GetString();

                throw new Exception(errorDetail);
            }
            catch (JsonException)
            {
                throw new Exception($"Xóa khóa học không thành công: {response.StatusCode}");
            }
        }

        // Parse response để lấy thông báo thành công
        try
        {
            var responseObj = JsonSerializer.Deserialize<ApiResponse>(content);
            return responseObj?.message ?? "Xóa khóa học thành công";
        }
        catch (JsonException)
        {
            return content; // Trả về nội dung gốc nếu không parse được JSON
        }
    }

    public class ApiResponse
    {
        public string message { get; set; }
    }

    public async Task<bool> CheckCourseHasStudents(string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/course/{courseId}/has-students");
        var response = await _authService.SendRequestWithAuthAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Không thể kiểm tra thông tin sinh viên đăng ký khóa học!");
        }

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var result = JsonSerializer.Deserialize<HasStudentsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result?.HasStudents ?? false;
        }
        catch (JsonException)
        {
            throw new Exception("Không thể đọc thông tin phản hồi từ server!");
        }
    }

    public class HasStudentsResponse
    {
        public bool HasStudents { get; set; }
    }
}