using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MySqlBulkLoaderDemo
{
    public static class MySqlHelper
    {
        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="_mySqlConnection"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int BulkLoad(MySqlConnection _mySqlConnection, DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList();
            MySqlBulkLoader bulk = new MySqlBulkLoader(_mySqlConnection)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = table.TableName + ".csv",
                NumberOfLinesToSkip = 0,
                TableName = table.TableName,

            };

            bulk.Columns.AddRange(columns);
            return bulk.Load();
        }
    }
}
