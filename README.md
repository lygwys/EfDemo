# EfDemo -零度分享第14期(EF批量数据 死锁检验 事务 并发）练习
数据库分区分表、复制、AlwaysOn高可用性
0:21:45
### 搭框架
- 新建EfDemo控制台项目
- 安装EF框架
- 添加数据库连接字符串
- 新建/Models/Book类
-
    namespace EfDemo.Models
    {
	    public class Book
	    {
		    public int ID { get; set; }
		    public string Name { get; set; }
		    public decimal Price { get; set; }
		    public DateTime CreateDate { get; set; }
	    }
    }

- 新建映射类/Maping/BookMap
-  
    namespace EfDemo.Mapping
    {
	    public class BookMap : EntityTypeConfiguration<Book>
	    {
		   	 public BookMap()
		    {
			    this.Property(b => b.Name).HasMaxLength(20);
			    this.Property(b => b.Price).HasColumnType("money");
		    }
	    }
    }

- 新建数据库上下文类
-  
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

- 允许数据迁移
· >Enable-Migrations ·
 Configuration.cs  `AutomaticMigrationsEnabled = true;`
- 数据迁移  
 `>Updata-database`

0:39:30
### EF扩展提高性能
- 安装EntityFramework.Extended
- 新建类EfExtendedDemo/EfExtendedSample.cs
-
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
    
···
0:59:40
### 事务
1:33:05
### 文件事务
要下载一个老外写的文件底层操作类TransactedFile.cs  
` TransactedFile.Delete(@"C:\1.jpg")`  


1:40:44
### EF方面事务

    using EfDemo.Models;
    using System.Data.SqlClient;
    using System.Transactions;
    /// <summary>
    /// EF事务示例 引用System.Transactions;
    /// </summary>
    namespace EfDemo.EfTransactionDemo
    {
	    public class EfTranSample
	    {
		    public void TransactionScoptTest()//常规的用法
		    {
		    	using (var tran = new TransactionScope())
			    {
				    DemoDbContext db1 = new DemoDbContext();
				    db1.Books.Find(1).Name = "C+++";
				    DemoDbContext db2 = new DemoDbContext();
				    db2.Books.Find(2).Name = "C++++";
				    db1.SaveChanges();
				    db2.SaveChanges();
				    tran.Complete();
			    }
		    }
		    
		    public void Ef6TransactionTest()//EF6将语句嵌入事务
		    {
			    DemoDbContext db = new DemoDbContext();
			    using (var tran = db.Database.BeginTransaction())
			    {
			    	try
				    {
					    string sql = "";
					    db.Database.ExecuteSqlCommand(sql);
					    db.Books.Find(1).Name = "JJJ";
					    db.SaveChanges();
				    }
				    catch 
				    {
				   	 	tran.Rollback();
				    }
			    }
		    }
		    
		    public void ExistTransactionTest()//将DbContext嵌入事务
		    {
			    using (var conn = new SqlConnection("connString"))//using System.Data.SqlClient;
			    {
				    conn.Open();
				    using (var sqlTran = conn.BeginTransaction())
					    {
					    	try
					    {
						    SqlCommand command = new SqlCommand();
						    command.Connection = conn;
						    command.Transaction = sqlTran;
						    command.CommandText = "sql";
						    command.ExecuteNonQuery();
						    DemoDbContext db = new DemoDbContext();
						    db.Database.UseTransaction(sqlTran);
						    db.Books.Find(1).Price = 11;
						    db.SaveChanges();
						    sqlTran.Commit();
					    }
					    catch 
					    {
					    	sqlTran.Rollback();   
					    }
				    }
			    }
		    }
	    }
    }
···
1:54:50
### EF异步
` db2.SaveChangesAsync(); `

1:55:30 
### EF并发处理
-  
	{
		public class ConcurrencySapmple
		{
			public void DefaultConcurrencyTest()//并发处理前提Book类加字段锁[ConcurrencyCheck]（BookMap启用字段检查this.Property(b => b.Price).IsConcurrencyToken();）或加时间戳[Timestamp]public Byte[] RowVersion { get; set; }（BookMap中启用行版本this.Property(b => b.RowVersion).IsRowVersion();）
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



