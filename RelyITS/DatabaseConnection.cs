using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelyITS
{
    using System.Data.SqlClient;

    public class DatabaseConnection
    {
        public void Connection(int SequenceNumber, int StoreID, decimal TotalItems, decimal TotalAmount)
        {
            // Connection string
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\jurka\\source\\repos\\RelyITS\\RelyITS\\ReceiptDataBase.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Check if the table exists and create it if it doesn't
                connection.Open();
                using (SqlCommand createTableCommand = new SqlCommand())
                {
                    createTableCommand.Connection = connection;
                    createTableCommand.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ReceiptTable')
                    BEGIN
                        CREATE TABLE ReceiptTable (
                            [SequenceNumber] INT NOT NULL,
                            [StoreID] INT NOT NULL,
                            [TotalItems] DECIMAL NOT NULL,
                            [TotalAmount] DECIMAL NOT NULL,
                            CONSTRAINT [PK_Table] PRIMARY KEY ([SequenceNumber])
                        );
                    END";
                    createTableCommand.ExecuteNonQuery();
                }

                // Insert the data into the ReceiptTable
                string insertQuery = "INSERT INTO ReceiptTable (SequenceNumber, StoreID, TotalItems, TotalAmount) VALUES (@SequenceNumber, @StoreID, @TotalItems, @TotalAmount)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@SequenceNumber", SequenceNumber);
                    insertCommand.Parameters.AddWithValue("@StoreID", StoreID);
                    insertCommand.Parameters.AddWithValue("@TotalItems", TotalItems);
                    insertCommand.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
