using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace trueLab22
{
    class Program
    {
        static SqlConnection connection = new SqlConnection(SQLConnect.SqlConnection());
        static void Main(string[] args)
        {
            connection.Open();
            Initialization();


            while (true)
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name, Message FROM dbo.Chat", connection))
                {
                    // Create a dependency and associate it with the SqlCommand.
                    SqlDependency dependency = new SqlDependency(command);
                    // Maintain the reference in a class member.

                    // Subscribe to the SqlDependency event.
                    dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Process the DataReader.
                    }
                    AddMessage("user2");
                    Thread.Sleep(1000);
                }
            }
        }

        static void Initialization()
        {
            // Create a dependency connection.
            SqlDependency.Start(SQLConnect.SqlConnection(), null);
        }


        // Handler method
        static void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"WITH SRC AS (SELECT TOP (" + 1 + ") Id, Name, " +
                "Message FROM Chat ORDER BY Id DESC) SELECT * FROM SRC ORDER BY Id", connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataTable dt in ds.Tables)
            {
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    int i = 0;
                    foreach (object cell in cells)
                    {
                        if (i != 0)
                        {
                            Console.Write("{0}", cell);
                        }
                        if (i == 1)
                        {
                            Console.Write(": ");
                        }
                        i++;
                    }
                    Console.WriteLine();
                }
            }
        }

        static void AddMessage(string login)
        {
            string message = Console.ReadLine();
            //SqlConnection connection = DBUtils.GetDBConnection();
            //connection.Open();

            string sql = "INSERT INTO dbo.Chat(Name, Message) VALUES(@name, @message)";

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            // Добавить параметр (Написать короче).
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = login;
            cmd.Parameters.Add("@message", SqlDbType.VarChar).Value = message;

            // Выполнить Command (Используется для delete, insert, update).
            int rowCount = cmd.ExecuteNonQuery();
        }

        static void OutputNewLine(int count)
        {
            
        }
    }
}
