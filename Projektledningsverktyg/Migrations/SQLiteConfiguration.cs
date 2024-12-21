using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite.EF6;

namespace Projektledningsverktyg.Migrations
{
    internal class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetProviderServices("System.Data.SQLite",
                (System.Data.Entity.Core.Common.DbProviderServices)
                SQLiteProviderFactory.Instance.GetService(
                    typeof(System.Data.Entity.Core.Common.DbProviderServices)));
        }
    }
}
