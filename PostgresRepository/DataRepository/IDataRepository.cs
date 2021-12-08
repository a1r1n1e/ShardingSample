namespace PostgresRepository
{
    public interface IDataRepository
    {
        public string Test();

        public long GetShardByBucket(long bucketId);

        public bool TrySave<T>(T obj) where T : IBucketable;

        public bool TryGet<T>(long objKey, out T objectToGet) where T : IBucketable, new();

    }

    public interface IBucketable
    {
        public long GetBucketKey();

        public long GetBucketKey(long objId);

        public string SaveQuery();

        public string GetQuery(long objKey);

        public void Init(byte[][] data);
    }

}
