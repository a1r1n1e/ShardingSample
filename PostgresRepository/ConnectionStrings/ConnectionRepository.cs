using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace PostgresRepository
{
    public class ConnectionRepository : IConnectionRepository, IDisposable
    {
        protected static Random Random = new Random();

        protected ShardInfo _masterShard { get; set; }

        protected Dictionary<long, ShardInfo> _knownShards { get; set; }


        protected ConcurrentDictionary<long, NpgsqlConnection> _connectionsStorage = new ConcurrentDictionary<long, NpgsqlConnection>(Environment.ProcessorCount, 3);

        public void Dispose()
        {
            foreach (var connection in _connectionsStorage.Values)
            {
                connection.Dispose();
            }
        }

        public ConnectionRepository()
        {

            _masterShard = new ShardInfo
            {
                ShardId = 0,
                ConnectionString = "host=postgres_image_0;port=5432;database=postgres;username=postgres;password=postgres"
            };

            _knownShards = new Dictionary<long, ShardInfo> 
            {
                { 
                    _masterShard.ShardId, 
                    _masterShard 
                },
                { 
                    1, 
                    new ShardInfo
                    {
                        ShardId = 1,
                        ConnectionString = "host=postgres_image_1;port=5432;database=postgres;username=postgres;password=postgres"
                    } 
                },
                { 
                    2,
                    new ShardInfo
                    {
                        ShardId = 2,
                        ConnectionString = "host=postgres_image_2;port=5432;database=postgres;username=postgres;password=postgres"
                    }
                }
            };

        }

        public long GetShardsCount()
        {
            return _knownShards.Count;
        }

        public NpgsqlConnection GetConnection(ConnectionPickerOptions options)
        {
            
            switch (options.ConnectionType)
            {
                case ConnectionType.Random:
                    return GetRandom();
                case ConnectionType.Master:
                    return GetMaster();
                case ConnectionType.ByShardKey:
                    return GetShard(options);
                default:
                    throw new NotImplementedException($"Unsupported ConnectionType: {options.ConnectionType.ToString()}");
            }
            
        }

        protected NpgsqlConnection GetRandom()
        {

            ShardInfo shard = _knownShards[Random.Next() % _knownShards.Keys.Count];
            
            var conn = _connectionsStorage.GetOrAdd(shard.ShardId, new NpgsqlConnection(shard.ConnectionString));
            if (conn != null && conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            return conn;

        }

        protected NpgsqlConnection GetMaster()
        {
            var conn = _connectionsStorage.GetOrAdd(_masterShard.ShardId, new NpgsqlConnection(_masterShard.ConnectionString));
            if (conn != null && conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            return conn;
        }

        protected NpgsqlConnection GetShard(ConnectionPickerOptions options)
        {
            if (!options.ShardKey.HasValue)
            {
                throw new ArgumentException($"Can't get sharded connection with no ShardId specified");
            }

            if (!_knownShards.TryGetValue(options.ShardKey.Value, out ShardInfo shard))
            {
                throw new ArgumentException($"Passed ShardId is not a valid one: {options.ShardKey.Value}");
            }

            var conn = _connectionsStorage.GetOrAdd(shard.ShardId, new NpgsqlConnection(shard.ConnectionString));
            if (conn != null && conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            return conn;
        }
    }
}
