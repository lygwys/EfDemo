using EfDemo.Concurrency;
using EfDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrencySapmple defau = new ConcurrencySapmple();
            defau.DefaultConcurrencyTest();

            Console.ReadKey();
            //DemoDbContext context = new DemoDbContext();
            //context.Books.Add(new Book { Name = "C#入门", Price = 88, CreateDate = DateTime.Now });
            //context.Books.Add(new Book { Name = "C++", Price = 108, CreateDate = DateTime.Now.AddDays(-10) });
            //context.Books.Add(new Book { Name = "C#从入门到精通", Price = 388, CreateDate = DateTime.Now });
            //context.Books.Add(new Book { Name = "C语言入门", Price = 288, CreateDate = DateTime.Now });
            //context.Books.Add(new Book { Name = "JAVA入门", Price = 188, CreateDate = DateTime.Now });
            //context.Database.Log = Console.WriteLine;//日志输出到控制台
            //context.SaveChanges();  
            //Console.ReadKey();
        }
    }
}
