using Npgsql;

namespace PostgresRepository
{
    public interface IConnectionRepository
    {

        public long GetShardsCount();

        public NpgsqlConnection GetConnection(ConnectionPickerOptions options);

    }

    public class ConnectionPickerOptions
    {
        public ConnectionType ConnectionType { get; protected set; }
        public long? ShardKey { get; protected set; }
        protected ConnectionPickerOptions()
        {
        }
        
        public static ConnectionPickerOptions GenerateRandom()
        {
            return new ConnectionPickerOptions
            {
                ConnectionType = ConnectionType.Random,
                ShardKey = null
            };
        }

        public static ConnectionPickerOptions GenerateMaster()
        {
            return new ConnectionPickerOptions
            {
                ConnectionType = ConnectionType.Master,
                ShardKey = null
            };
        }

        public static ConnectionPickerOptions GenerateSharded(long shardKey)
        {
            return new ConnectionPickerOptions
            {
                ConnectionType = ConnectionType.Random,
                ShardKey = shardKey
            };
        }
    }

    public enum ConnectionType
    {
        Random,
        Master, 
        ByShardKey
    }
}
