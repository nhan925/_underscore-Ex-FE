namespace student_management_fe.Helpers;

public class ErrorResponse
{
    public int Status { get; set; }

    public string Message { get; set; }

    public ErrorResponse(int status, string message)
    {
        Status = status;
        Message = message;
    }
}

