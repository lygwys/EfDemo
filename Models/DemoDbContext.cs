using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfDemo.Models
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext() : base("demo") { }        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();//移除约定 让迁移的表名为单数形式，默认是以Books命名数据表
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());//扫描当前配置的东西全加载进来，Mapping就会生效
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Book> Books { get; set; }
    }
}
