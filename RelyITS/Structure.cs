using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace RelyITS
{
    public class Structure
    {
        [XmlRoot(ElementName = "UnitID")]
        public class unitID
        {
            [XmlElement(ElementName = "UnitID")]
            public int UnitID { get; set; }
        }
        [XmlRoot(ElementName = "Transaction")]
        public class transaction
        {
            [XmlElement(ElementName = "BusinessUnit")]
            public List<unitID> UnitIDs { get; set; }
            [XmlElement(ElementName = "SequenceNumber")]
            public int SequenceNumber { get; set; }
            [XmlElement(ElementName = "RetailTransaction")]
            public List<retailTransaction> retailTransactions { get; set; }
        }
        [XmlRoot("POSLog", Namespace = "http://www.nrf-arts.org/IXRetail/namespace/")]
        public class POSLog
        {
            [XmlElement(ElementName = "Transaction")]
            public transaction transaction { get; set; }
        }
        [XmlRoot(ElementName = "Quantity")]
        public class quantity
        {

            [XmlAttribute(AttributeName = "Units")]
            public string units { get; set; }
            [XmlAttribute(AttributeName = "UnitOfMeasureCode")]
            public string unitOfMeasureCode { get; set; }
        }

        [XmlRoot(ElementName = "Sale")]
        public class Sale
        {
            [XmlElement(ElementName = "Quantity")]
            public decimal Quantity { get; set; }
        }
        [XmlRoot(ElementName = "LineItem")]
        public class lineItem
        {
            [XmlElement(ElementName = "Sale")]
            public Sale Sale { get; set; }
            [XmlAttribute(AttributeName = "CancelFlag")]
            public string cancelFlag { get; set; }
            [XmlAttribute(AttributeName = "EntryMethod")]
            public string entryMethod { get; set; }

        }
        [XmlRoot(ElementName = "RetailTransaction")]
        public class retailTransaction
        {
            [XmlElement(ElementName = "LineItem")]
            public lineItem lineItem { get; set; }
            [XmlElement(ElementName = "Total")]
            public decimal Total { get; set; }
        }
    }
}
