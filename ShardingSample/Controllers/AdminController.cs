using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostgresRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShardingSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IDataRepository _dataRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IDataRepository dataRepository, ILogger<AdminController> logger)
        {
            _dataRepository = dataRepository;
            _logger = logger;
        }

        [HttpGet("~/Get")]
        public IEnumerable<string> Get()
        {
            var rnd = new Random();
            return Enumerable.Range(1, 5).Select(x => rnd.Next().ToString());
        }

        [HttpGet("~/TestNpgsql")]
        public IEnumerable<string> TestNpgSql()
        {
            yield return _dataRepository.Test();
        }
    }
}
