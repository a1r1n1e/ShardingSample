using System;

namespace PostgresRepository
{
    public class ConnectionRepository : IConnectionRepository
    {
        protected static Random _random = new Random();

        protected static string[] _connectionStringPool = { 
            "host=postgres_image_0;port=5432;database=postgres;username=postgres;password=postgres",
            "host=postgres_image_1;port=5432;database=postgres;username=postgres;password=postgres",
            "host=postgres_image_2;port=5432;database=postgres;username=postgres;password=postgres"
        };

        public string GetConnectionString(ConnectionPickerOptions options)
        {

            return _connectionStringPool[_random.Next() % 3];
        }
    }
}
