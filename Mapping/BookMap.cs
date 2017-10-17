using EfDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfDemo.Mapping
{
    public class BookMap : EntityTypeConfiguration<Book>
    {
        public BookMap()
        {
            this.Property(b => b.Name).HasMaxLength(20);
            this.Property(b => b.Price).HasColumnType("money");

            //this.Property(b => b.RowVersion).IsRowVersion();//启用行版本，解决并发
            this.Property(b => b.Price).IsConcurrencyToken();//启用字段检查
        }
    }
}
