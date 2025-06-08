namespace student_management_fe.Services;

public class DataService : IDataService
{
    public object Data { get; set; }

    public void SetData<T>(T data)
    {
        Data = data;
    }

    public T GetData<T>() where T : class
    {
        return Data as T;
    }
}
