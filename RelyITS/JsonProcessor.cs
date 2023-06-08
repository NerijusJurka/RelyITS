using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RelyITS
{
    public class JsonProcessor
    {
        public void ReadAndWriteToJson(string message)
        {
            var deserializedObject = DeserializeFromXML(message);
            var filePath = @"C:\temp\Receipt.json";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            var jsonData = File.ReadAllText(filePath);
            var receiptList = JsonConvert.DeserializeObject<List<ReceiptJson>>(jsonData) ?? new List<ReceiptJson>();

            receiptList.Add(new ReceiptJson()
            {
                Date = DateTime.Now.Date,
                StoreID = deserializedObject.transaction.UnitIDs[0].UnitID,
                TotalItems = deserializedObject.transaction.retailTransactions[0].Total,
                TotalAmount = deserializedObject.transaction.retailTransactions[0].lineItem.Sale.Quantity,
                TotalReceipts = 1
            });

            var instance = new DatabaseConnection();
            instance.Connection(deserializedObject.transaction.SequenceNumber, deserializedObject.transaction.UnitIDs[0].UnitID, deserializedObject.transaction.retailTransactions[0].lineItem.Sale.Quantity, deserializedObject.transaction.retailTransactions[0].Total);

            jsonData = JsonConvert.SerializeObject(receiptList);
            File.WriteAllText(filePath, jsonData);

            var messageSender = new MessageSender();
            messageSender.Send("SiustiJson", jsonData);
        }

        private Structure.POSLog DeserializeFromXML(string message)
        {
            var serializer = new XmlSerializer(typeof(Structure.POSLog));
            Structure.POSLog result;
            using (TextReader reader = new StringReader(message))
            {
                result = (Structure.POSLog)serializer.Deserialize(reader);
            }
            return result;
        }
    }
}
