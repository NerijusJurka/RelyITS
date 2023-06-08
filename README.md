README.md (.NET Coding Exercise)

# .NET coding exercise

## Task Scope
The goal of this task is to develop a .NET Core solution that retrieves sales data statistics from incoming XML receipts.

## Specifications
The proposed solution is to read incoming messages, store relevant data in a database and
produce a scheduled report at the end of the day. The solution should have the following
functionality:

1. Reading and producing messages from/to events managing platforms (e.g. Apache Kafka or RabbitMQ). Incoming message has a POSLog format.

2. Database migration scripts for creating the necessary tables.

3. Scheduled job that runs once a day at 1am and produces JSON message to queue with sales grouped by store. It also should save calculated values to the database.
List of output fields in the file are:

   JSON field name | Field name in XML file | Description
   --- | --- | ---
   Date |  |Today's date
   StoreID | UnitID | Internal ID of the Store
   TotalItems | RetailTransaction\LineItem\Sale\Quantity | Amount of items sold
   TotalAmount | RetailTransaction\Total[TotalType="TransactionNetAmount"] | Total amount paid in the receipt
   TotalReceipts | | Total amount of receipts received from the store

4. Endpoints for:
   - Getting specific receipt data by receipt number. Receipt number in XML corresponds to Transaction/SequenceNumber
   - Getting calculated sales data for the specified date range
