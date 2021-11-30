using Npgsql;

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
            using var con = new NpgsqlConnection(_connectionRepository.GetConnectionString(new ConnectionPickerOptions()));
            con.Open();

            var sql = "SELECT version()";

            using var cmd = new NpgsqlCommand(sql, con);

            var version = cmd.ExecuteScalar().ToString();

            return version;
        }
    }
}
