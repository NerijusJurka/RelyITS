using System;
using System.Collections.Generic;
using System.Text;

namespace RelyITS
{
    public class ReceiptJson
    {
        public DateTime Date { get; set; }
        public int StoreID { get; set; }
        public decimal TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalReceipts { get; set; }
    }
}
