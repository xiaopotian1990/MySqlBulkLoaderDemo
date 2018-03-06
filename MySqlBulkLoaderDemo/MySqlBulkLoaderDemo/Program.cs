using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;

namespace MySqlBulkLoaderDemo
{
    class Program
    {
        private static ILog _logger;
        private static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            Init();

            using(MySqlConnection connection=new MySqlConnection($"{_configuration["datasource:url"]}"))
            {
                MySqlTransaction sqlTransaction = null;
                try
                {
                    connection.Open();
                    sqlTransaction = connection.BeginTransaction();

                    DataTable dt = new DataTable
                    {
                        TableName = "data"
                    };
                    dt.Columns.Add("name");
                    dt.Columns.Add("age");

                    for (int i = 1; i <= 10000000; i++)
                    {
                        dt.Rows.Add(new Object[] { "小破天" + i, i });
                    }

                    dt.ToCsv();

                    MySqlHelper.BulkLoad(connection, dt);

                    sqlTransaction.Commit();

                    Console.WriteLine("数据迁移成功！");
                }
                catch(Exception e)
                {
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                    }
                    Console.WriteLine("数据迁移失败：" + e.Message);
                    _logger.Error(e);
                }
                
            }
            Console.ReadLine();
        }

        private static void Init()
        {
            ILoggerRepository repository = LogManager.CreateRepository(@"NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo(@"log4net.config"));
            _logger = LogManager.GetLogger(repository.Name, "NETCorelog4net");

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@"appsettings.json");
            _configuration = builder.Build();
        }
    }
}
