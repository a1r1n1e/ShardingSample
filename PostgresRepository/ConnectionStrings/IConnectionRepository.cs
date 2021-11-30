namespace PostgresRepository
{
    public interface IConnectionRepository
    {
        public string GetConnectionString(ConnectionPickerOptions options);
    }

    public class ConnectionPickerOptions
    {

    }
}
