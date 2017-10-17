using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfDemo.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [ConcurrencyCheck]//字段锁，同时修改字段会抛出异常
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }

        //[Timestamp]//时间戳（行版本）
        //public Byte[] RowVersion { get; set; }

    }
}
