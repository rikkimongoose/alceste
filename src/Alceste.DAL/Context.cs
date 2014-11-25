using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alceste.DAL.DataTypes;

namespace Alceste.DAL
{
    public class Context : DbContext
    {
        public Context()
            : base("CacheDbContext")
        { }

        public DbSet<CachedItem> CachedItems { get; set; }
        public DbSet<CachedItemImage> CachedItemImages { get; set; }
    }
}
