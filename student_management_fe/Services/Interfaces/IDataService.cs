namespace student_management_fe.Services;

public interface IDataService
{
    object Data { get; set; }

    void SetData<T>(T data);

    T GetData<T>() where T : class;
}
