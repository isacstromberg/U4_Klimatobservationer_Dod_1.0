using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace U4_Klimatobservationer_Dod_1._0.Repostories
{
    internal class Db_Repository
    {
        private string _connectionString;
        public Db_Repository()
        {
            
            var config = new ConfigurationBuilder().AddUserSecrets<Db_Repository>()
                        .Build();

            _connectionString = config.GetConnectionString("dbConn");
        }


    }
}
