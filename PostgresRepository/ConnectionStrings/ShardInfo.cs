using System;
using System.Collections.Generic;
using System.Text;

namespace PostgresRepository
{
    public class ShardInfo
    {

        internal long ShardId { get; set; }
        internal string ConnectionString { get; set; }

    }
}
