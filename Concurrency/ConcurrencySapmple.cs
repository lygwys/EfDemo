using EfDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EfDemo.Concurrency
{
    public class ConcurrencySapmple
    {
        public void DefaultConcurrencyTest()//并发处理前提Book类加字段锁[ConcurrencyCheck]（BookMap启用字段检查this.Property(b => b.Price).IsConcurrencyToken();）或加时间戳//[Timestamp]public Byte[] RowVersion { get; set; }（BookMap中启用行版本this.Property(b => b.RowVersion).IsRowVersion();）
        {
            try
            {
                DemoDbContext db1 = new DemoDbContext();
                DemoDbContext db2 = new DemoDbContext();

                var book1 = db1.Books.Find(1);
                var book2 = db2.Books.Find(1);

                book1.Price += 10;
                book1.Name = "Book1";

                book2.Price += 20;
                book2.Name = "Book2";
                db1.SaveChanges();
                db2.SaveChanges();

            }
            catch (DbUpdateConcurrencyException e)//using System.Data.Entity.Infrastructure;
            {
                e.Entries.Single().Reload();//获取没保存成功的，重新装载一下，把数据库最新版本再装载一下并执行,让并行的都得以执行，不会产生覆盖
            }

        }
    }
}
