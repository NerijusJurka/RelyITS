using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelyITS
{
	public class DatabaseConnection
	{
		public void Connection(int SequenceNumber, int StoreID, decimal TotalItems, decimal TotalAmount)
		{
			                                                 //Connection string
			using (SqlConnection openCon = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\jurka\\source\\repos\\RelyITS\\RelyITS\\ReceiptDataBase.mdf;Integrated Security=True"))
			{
				//If table doesnt exist
				openCon.Open();
				using (SqlCommand cmd = openCon.CreateCommand())
				{
					cmd.CommandText = @"SELECT count(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' and TABLE_NAME = 'ReceiptTable'
					BEGIN
						if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ReceiptTable')
						CREATE TABLE ReceiptTable (
						[SequenceNumber] INT NOT NULL,
						[StoreID] INT NOT NULL, 
						[TotalItems] DECIMAL NOT NULL, 
						[TotalAmount] DECIMAL NOT NULL, 
						CONSTRAINT [PK_Table] PRIMARY KEY ([SequenceNumber]),
						);	
					END";
					cmd.ExecuteNonQuery();
				}
				openCon.Close();
				//if exist
				string receipt = "INSERT INTO ReceiptTable (SequenceNumber,StoreID,TotalItems,TotalAmount) VALUES (@SequenceNumber,@StoreID,@TotalItems,@TotalAmount)";
				using (SqlCommand queryReceipt = new SqlCommand(receipt))
				{
					queryReceipt.Connection = openCon;
					queryReceipt.Parameters.Add("@SequenceNumber", System.Data.SqlDbType.Int).Value = SequenceNumber;
					queryReceipt.Parameters.Add("@StoreID", System.Data.SqlDbType.Int).Value = StoreID;
					queryReceipt.Parameters.Add("@TotalItems", System.Data.SqlDbType.Decimal).Value = TotalItems;
					queryReceipt.Parameters.Add("@TotalAmount", System.Data.SqlDbType.Decimal).Value = TotalAmount;
					openCon.Open();
					queryReceipt.ExecuteNonQuery();
				}
			}
		}
	}
}
