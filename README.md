# CreditReportCRUD
Application receives data from SQL Database that originates from a REST API in an online customer portal


Parse Reports is the main application file

GetData receives data from its origin by calling GetLastRecord and InsertData

GetLast receives ID of last inserted datarow and returns it

InsertData inserts parsed data into SQL Table

