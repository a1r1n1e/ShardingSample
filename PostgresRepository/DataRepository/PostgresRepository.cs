using Npgsql;
using System;

namespace PostgresRepository
{
    public class PostgresRepository : IDataRepository
    {

        private readonly IConnectionRepository _connectionRepository;

        public PostgresRepository(IConnectionRepository connectionRepository)
        {

            _connectionRepository = connectionRepository;

        }

        public string Test()
        {

            var con = _connectionRepository.GetConnection(ConnectionPickerOptions.GenerateRandom());

            var sql = "SELECT version()";

            using var cmd = new NpgsqlCommand(sql, con);

            var version = cmd.ExecuteScalar().ToString();

            return version;

        }

        //TODO: Complete trash
        public bool TrySave<T>(T obj) where T : IBucketable
        {

            var shardId = GetShardByBucket(obj.GetBucketKey());

            var con = _connectionRepository.GetConnection(ConnectionPickerOptions.GenerateSharded(shardId));

            var sql = obj.SaveQuery();

            using var cmd = new NpgsqlCommand(sql, con);
            var affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;

        }

        //TODO: Complete trash
        public bool TryGet<T>(long objKey, out T objectToGet) where T : IBucketable, new()
        {

            objectToGet = new T();

            var shardId = GetShardByBucket(objectToGet.GetBucketKey(objKey));

            var con = _connectionRepository.GetConnection(ConnectionPickerOptions.GenerateSharded(shardId));

            var sql = objectToGet.GetQuery(objKey);

            using var cmd = new NpgsqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var fieldsData = new byte[reader.FieldCount][];
                for(var i = 0; i < reader.FieldCount; ++i)
                {
                    fieldsData[i] = (byte[])reader[i];
                    objectToGet.Init(fieldsData);
                }
            }

            return true;

        }

        public long GetShardByBucket(long bucketId)
        {

            var shardsCount = _connectionRepository.GetShardsCount();

            if (shardsCount <= 0)
            {
                throw new InvalidOperationException("No present Shards");
            }

            return bucketId % shardsCount;

        }
    }
}
