using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOF.Framework.Data;

namespace Ranger.TestConsole
{
    public class DavinciTsDbContext : DbClient
    {
        public ITable<Candidate> Candidates { get; set; }

        public DavinciTsDbContext() : this(ConfigurationManager.AppSettings["azure:sqldb:connectionstring2"]) { }
        public DavinciTsDbContext(string ConnectionString) : base(ConnectionString) { }

        protected override void DefiningModelStrategies(ModelStrategyBuilder builder)
        {
            base.DefiningModelStrategies(builder);
        }
    }
}
