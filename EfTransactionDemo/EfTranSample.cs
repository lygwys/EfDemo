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
