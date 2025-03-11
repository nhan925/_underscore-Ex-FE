using System.Text.Json.Serialization;

namespace student_management_fe.Models;

public class AuthResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}
