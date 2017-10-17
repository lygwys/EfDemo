using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using EfDemo.Models;
using EntityFramework.Caching;
using EntityFramework;

namespace EfDemo.EfExtendedDemo
{
    class EfExtendedSample
    {
        public void BatchDelete()//批量删除
        {
            DemoDbContext db = new DemoDbContext();
            db.Books.Where(b => b.Price <= 100).Delete();
            db.SaveChanges();
        }

        public void BatchUpdate()//批量更新
        {
            DemoDbContext db = new DemoDbContext();
            db.Books.Where(b => b.Price <= 100).Update(bc => new Book { Price = bc.Price + 10 });
            db.SaveChanges();
        }

        public void BatchQuery()//批量查询
        {
            DemoDbContext db = new DemoDbContext();
            //var books = db.Books.Where(b => b.Price >= 100).ToList();
            //var books2 = db.Books.Where(b => b.Price <= 100).ToList();
            var q1 = db.Books.Where(b => b.Price >= 100).Future();//加上 .Future()进行标记就可以在一个查询连接中执行多个查询
            var q2 = db.Books.Where(b => b.Price <= 100).Future();
            var books = q1.ToList();
            var books2 = q2.ToList();
        }

        public void CacheQueryResult()//使用缓存
        {
            DemoDbContext db = new DemoDbContext();
            var q1 = db.Books.Where(b => b.Price >= 100).FromCache();
            var q2 = db.Books.Where(b => b.Price >= 100).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromMinutes(10)));//10钟后过期

            var q3 = db.Books.Where(b => b.Price >= 100).FromCache(tags:new[] { "book111","oldbook"});//给缓存打N个标记
            CacheManager.Current.Expire("book111");//让标记的缓存过期

           // Locator.Current.Register<ICacheProvider>(() =>new RedisCache());//配置一下Redis即可实现分布式缓存
        }
    }
}
