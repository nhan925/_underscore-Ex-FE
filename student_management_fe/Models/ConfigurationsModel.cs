namespace student_management_fe.Models;

public class ConfigurationModel<T>
{
    public int Id { get; set; }

    public string Type { get; set; }

    public T Value { get; set; }

    public bool IsActive { get; set; }
}