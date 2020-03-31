using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Xml;
using System.Xml.Linq;
namespace LeapCreditParse
{
    class Parsexml
    {

        public Parsexml(string CreditReportid, XElement xmlElement, string BorrowerId, string leadid)
        {

            string ConString2 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
         


            DataSet InquiryDataSet = new DataSet();

            //  XmlReader reader = XmlReader.ReadSubtree()
            foreach (XElement elee in xmlElement.Elements("inquiry")) //ReadSubtree("inquiry"))
            {



                InquiryDataSet.ReadXml(elee.CreateReader());

                foreach (DataTable table in InquiryDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.String));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                        dr["BorrowId"] = BorrowerId;
                        dr["LeadId"] = leadid;
                    }


                }


            }

            using (SqlConnection connection = new SqlConnection(ConString2))
            {

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    connection.Open();
                    DataTable tableInq = InquiryDataSet.Tables["inquiry"];
                    bulkCopy.DestinationTableName =
                    "dbo.Test_CL_Inq";

                    try
                    {
                        foreach (DataColumn col in tableInq.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(tableInq);
                        Console.WriteLine("success");
                    }
                    catch (Exception ex)
                    {

                     
                        Console.WriteLine(ex.Message);
                    }
                    connection.Close();
                  

                }
            
            }






            DataSet StabilitiesDataSet = new DataSet();


            foreach (XElement ele in xmlElement.Descendants("clear-fraud-insight").Descendants("stabilities"))
            {
                //cf stability
                StabilitiesDataSet.ReadXml(ele.CreateReader());//reader.ReadInnerXml());
                                                               // For each table in the DataSet, print the row values.




                foreach (DataTable table in StabilitiesDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.Int64));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.Int64));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                        dr["BorrowId"] = BorrowerId;
                        dr["LeadId"] = leadid;
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        connection.Open();
                        DataTable tableStabilities_CF = StabilitiesDataSet.Tables["stability"];
                        bulkCopy.DestinationTableName = "dbo.Stability_CF";



                        try
                        {
                            foreach (DataColumn col in tableStabilities_CF.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableStabilities_CF);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        connection.Close();
                    }
                }



            }



            DataSet ClearFraudInsightDataSet = new DataSet();


            foreach (XElement ele in xmlElement.Descendants("clear-fraud-insight"))
            {

                ClearFraudInsightDataSet.ReadXml(ele.CreateReader());//reader.ReadInnerXml());
                                                                     // For each table in the DataSet, print the row values.
                                                                     //StabilitiesDataSet.ReadXml(ele.CreateReader());



                foreach (DataTable table in ClearFraudInsightDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                        dr["BorrowId"] = BorrowerId;
                        dr["LeadId"] = leadid;
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {

                        SqlBulkCopy bulkCopyRatio = new SqlBulkCopy(connection);
                        SqlBulkCopy bulkCopyCrossTab = new SqlBulkCopy(connection);
                        connection.Open();
                        DataTable tableClearFraudInsightDataSet_CF = ClearFraudInsightDataSet.Tables["clear-fraud-insight"];
                        DataTable tableCFRatios = ClearFraudInsightDataSet.Tables["ratio"];
                        DataTable tableCFCrosstab = ClearFraudInsightDataSet.Tables["crosstab"];
                        bulkCopy.DestinationTableName = "dbo.ClearFraudInsight";
                        bulkCopyCrossTab.DestinationTableName = "dbo.crosstab_CF";
                        bulkCopyRatio.DestinationTableName = "dbo.RatioCF";
                        //"dbo.Test_CL_Inq";

                        try
                        {
                            foreach (DataColumn col in tableClearFraudInsightDataSet_CF.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }


                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableClearFraudInsightDataSet_CF);

                            Console.WriteLine("success: Clear Fraud Insight");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }



                        try
                        {
                            foreach (DataColumn col in tableCFCrosstab.Columns)
                            {
                                bulkCopyCrossTab.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.

                            bulkCopyCrossTab.WriteToServer(tableCFCrosstab);

                            Console.WriteLine("success CF CrossTab");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        try
                        {
                            // Write from the source to the destination.
                            foreach (DataColumn col in tableCFRatios.Columns)
                            {
                                bulkCopyRatio.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            bulkCopyRatio.WriteToServer(tableCFRatios);
                            Console.WriteLine("success CF Ratios");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }





                        connection.Close();
                    }
                }



            }

            //credit risk stability 

            DataSet CRStabilities = new DataSet();

            foreach (XElement ele in xmlElement.Descendants("clear-credit-risk").Descendants("stabilities"))
            {

                CRStabilities.ReadXml(ele.CreateReader());//reader.ReadInnerXml());



                foreach (DataTable table in CRStabilities.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.String));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                        dr["BorrowId"] = BorrowerId;
                        dr["LeadId"] = leadid;
                    }
                }
                DataTable tableCRStabilities = CRStabilities.Tables["stability"];
                if (tableCRStabilities.Columns.Count == 16)
                {

                    using (SqlConnection connection = new SqlConnection(ConString2))
                    {
                        using (SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection))
                        {

                           // SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection);


                            connection.Open();
                            
                            bulkcopyCRStabilities.DestinationTableName = "dbo.StabilityCR";

                            try
                            {
                                foreach (DataColumn col in tableCRStabilities.Columns)
                                {
                                    bulkcopyCRStabilities.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                }
                                // Write from the source to the destination.
                                bulkcopyCRStabilities.WriteToServer(tableCRStabilities);
                                Console.WriteLine("success CR Stability");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }


                else
                {
                    using (SqlConnection connection = new SqlConnection(ConString2))
                    {
                        using (SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection))
                        {

                            //SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection);


                            connection.Open();

                            bulkcopyCRStabilities.DestinationTableName = "[dbo].[StabilityCR5Row]";

                            try
                            {

                                foreach (DataColumn col in tableCRStabilities.Columns)
                                {
                                    bulkcopyCRStabilities.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                }

                                // Write from the source to the destination.
                                bulkcopyCRStabilities.WriteToServer(tableCRStabilities);
                                Console.WriteLine("success CR Stability");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }



                        DataSet ClearCreditRisk = new DataSet();


            foreach (XElement ele in xmlElement.Elements("clear-credit-risk"))
            {

                ClearCreditRisk.ReadXml(ele.CreateReader());//reader.ReadInnerXml());



                foreach (DataTable table in ClearCreditRisk.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.Int64));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.Int64));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                        dr["BorrowId"] = BorrowerId;
                        dr["LeadId"] = leadid;
                    }
                }


                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {

                       
                        SqlBulkCopy bulkcopyCRInquiry = new SqlBulkCopy(connection);
                        SqlBulkCopy bulkcopyCRTradelines = new SqlBulkCopy(connection);
                       
                        connection.Open();
                        DataTable tableClearCreditRisk = ClearCreditRisk.Tables["clear-credit-risk"];
                        
                        DataTable tableCRInquiries = ClearCreditRisk.Tables["inquiry"];
                        DataTable tableCRTradelines = ClearCreditRisk.Tables["tradeline"];
                        bulkCopy.DestinationTableName = "dbo.ClearCreditRisk";
                        bulkcopyCRInquiry.DestinationTableName = "dbo.InquiryCR";
                     
                        bulkcopyCRTradelines.DestinationTableName = "dbo.TradelinesCR";
                        try
                        {
                            foreach (DataColumn col in tableClearCreditRisk.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableClearCreditRisk);
                            Console.WriteLine("success clear credit risk");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        //multi try catch


                        try
                        {

                            foreach (DataColumn col in tableCRTradelines.Columns)
                            {
                                bulkcopyCRTradelines.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.
                            bulkcopyCRTradelines.WriteToServer(tableCRTradelines);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        try
                        {
                            // Write from the source to the destination.
                            bulkcopyCRInquiry.WriteToServer(tableCRInquiries);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        


                        connection.Close();
                    }
                }



            }

        }






        //constructor 2



        public Parsexml(string CreditReportid, XElement xmlElement, string leadid)
        {

            string ConString2 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
          ;



            DataSet InquiryDataSet = new DataSet();

            //  XmlReader reader = XmlReader.ReadSubtree()
            foreach (XElement elee in xmlElement.Elements("inquiry")) //ReadSubtree("inquiry"))
            {



                InquiryDataSet.ReadXml(elee.CreateReader());

                foreach (DataTable table in InquiryDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.String));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                      
                        dr["LeadId"] = leadid;
                    }


                }


            }

            using (SqlConnection connection = new SqlConnection(ConString2))
            {

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    connection.Open();
                    DataTable tableInq = InquiryDataSet.Tables["inquiry"];
                    bulkCopy.DestinationTableName =
                    "dbo.Test_CL_Inq";

                    try
                    {
                        foreach (DataColumn col in tableInq.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(tableInq);
                        Console.WriteLine("success");
                    }
                    catch (Exception ex)
                    {


                        Console.WriteLine(ex.Message);
                    }
                    connection.Close();


                }

            }






            DataSet StabilitiesDataSet = new DataSet();


            foreach (XElement ele in xmlElement.Descendants("clear-fraud-insight").Descendants("stabilities"))
            {
                //cf stability
                StabilitiesDataSet.ReadXml(ele.CreateReader());//reader.ReadInnerXml());
                                                               // For each table in the DataSet, print the row values.




                foreach (DataTable table in StabilitiesDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.Int64));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.Int64));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                   
                        dr["LeadId"] = leadid;
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        connection.Open();
                        DataTable tableStabilities_CF = StabilitiesDataSet.Tables["stability"];
                        bulkCopy.DestinationTableName = "dbo.Stability_CF";



                        try
                        {
                            foreach (DataColumn col in tableStabilities_CF.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableStabilities_CF);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        connection.Close();
                    }
                }



            }



            DataSet ClearFraudInsightDataSet = new DataSet();


            foreach (XElement ele in xmlElement.Descendants("clear-fraud-insight"))
            {

                ClearFraudInsightDataSet.ReadXml(ele.CreateReader());//reader.ReadInnerXml());
                                                                     // For each table in the DataSet, print the row values.
                                                                     //StabilitiesDataSet.ReadXml(ele.CreateReader());



                foreach (DataTable table in ClearFraudInsightDataSet.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                       
                        dr["LeadId"] = leadid;
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {

                        SqlBulkCopy bulkCopyRatio = new SqlBulkCopy(connection);
                        SqlBulkCopy bulkCopyCrossTab = new SqlBulkCopy(connection);
                        connection.Open();
                        DataTable tableClearFraudInsightDataSet_CF = ClearFraudInsightDataSet.Tables["clear-fraud-insight"];
                        DataTable tableCFRatios = ClearFraudInsightDataSet.Tables["ratio"];
                        DataTable tableCFCrosstab = ClearFraudInsightDataSet.Tables["crosstab"];
                        bulkCopy.DestinationTableName = "dbo.ClearFraudInsight";
                        bulkCopyCrossTab.DestinationTableName = "dbo.crosstab_CF";
                        bulkCopyRatio.DestinationTableName = "dbo.RatioCF";
                        //"dbo.Test_CL_Inq";

                        try
                        {
                            foreach (DataColumn col in tableClearFraudInsightDataSet_CF.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }


                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableClearFraudInsightDataSet_CF);

                            Console.WriteLine("success: Clear Fraud Insight");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }



                        try
                        {
                            foreach (DataColumn col in tableCFCrosstab.Columns)
                            {
                                bulkCopyCrossTab.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.

                            bulkCopyCrossTab.WriteToServer(tableCFCrosstab);

                            Console.WriteLine("success CF CrossTab");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        try
                        {
                            // Write from the source to the destination.
                            foreach (DataColumn col in tableCFRatios.Columns)
                            {
                                bulkCopyRatio.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            bulkCopyRatio.WriteToServer(tableCFRatios);
                            Console.WriteLine("success CF Ratios");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }





                        connection.Close();
                    }
                }



            }

            //credit risk stability 

            DataSet CRStabilities = new DataSet();

            foreach (XElement ele in xmlElement.Descendants("clear-credit-risk").Descendants("stabilities"))
            {

                CRStabilities.ReadXml(ele.CreateReader());//reader.ReadInnerXml());



                foreach (DataTable table in CRStabilities.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.String));
                    table.Columns.Add("CreditReportId", typeof(System.String));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.String));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                       
                        dr["LeadId"] = leadid;
                    }
                }
                DataTable tableCRStabilities = CRStabilities.Tables["stability"];
                if (tableCRStabilities.Columns.Count == 16)
                {

                    using (SqlConnection connection = new SqlConnection(ConString2))
                    {
                        using (SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection))
                        {

                         //  SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection);


                            connection.Open();

                            bulkcopyCRStabilities.DestinationTableName = "dbo.StabilityCR";

                            try
                            {
                                foreach (DataColumn col in tableCRStabilities.Columns)
                                {
                                    bulkcopyCRStabilities.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                }
                                // Write from the source to the destination.
                                bulkcopyCRStabilities.WriteToServer(tableCRStabilities);
                                Console.WriteLine("success CR Stability");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }


                else
                {
                    using (SqlConnection connection = new SqlConnection(ConString2))
                    {
                        using (SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection))
                        {

                          //  SqlBulkCopy bulkcopyCRStabilities = new SqlBulkCopy(connection);


                            connection.Open();

                            bulkcopyCRStabilities.DestinationTableName = "[dbo].[StabilityCR5Row]";

                            try
                            {

                                // Write from the source to the destination.
                                bulkcopyCRStabilities.WriteToServer(tableCRStabilities);
                                Console.WriteLine("success CR Stability");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }



            DataSet ClearCreditRisk = new DataSet();


            foreach (XElement ele in xmlElement.Elements("clear-credit-risk"))
            {

                ClearCreditRisk.ReadXml(ele.CreateReader());//reader.ReadInnerXml());



                foreach (DataTable table in ClearCreditRisk.Tables)
                {
                    table.Columns.Add("BorrowId", typeof(System.Int64));
                    table.Columns.Add("CreditReportId", typeof(System.Int64));
                    table.Columns.Add("LoanId", typeof(System.String));
                    table.Columns.Add("LeadId", typeof(System.Int64));
                    foreach (DataRow dr in table.Rows)
                    {
                        dr["CreditReportId"] = CreditReportid;
                     
                        dr["LeadId"] = leadid;
                    }
                }


                using (SqlConnection connection = new SqlConnection(ConString2))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {


                        SqlBulkCopy bulkcopyCRInquiry = new SqlBulkCopy(connection);
                        SqlBulkCopy bulkcopyCRTradelines = new SqlBulkCopy(connection);

                        connection.Open();
                        DataTable tableClearCreditRisk = ClearCreditRisk.Tables["clear-credit-risk"];

                        DataTable tableCRInquiries = ClearCreditRisk.Tables["inquiry"];
                        DataTable tableCRTradelines = ClearCreditRisk.Tables["tradeline"];
                        bulkCopy.DestinationTableName = "dbo.ClearCreditRisk";
                        bulkcopyCRInquiry.DestinationTableName = "dbo.InquiryCR";

                        bulkcopyCRTradelines.DestinationTableName = "dbo.TradelinesCR";
                        try
                        {
                            foreach (DataColumn col in tableClearCreditRisk.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(tableClearCreditRisk);
                            Console.WriteLine("success clear credit risk");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        //multi try catch


                        try
                        {

                            foreach (DataColumn col in tableCRTradelines.Columns)
                            {
                                bulkcopyCRTradelines.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            // Write from the source to the destination.
                            bulkcopyCRTradelines.WriteToServer(tableCRTradelines);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        try
                        {
                            // Write from the source to the destination.
                            bulkcopyCRInquiry.WriteToServer(tableCRInquiries);
                            Console.WriteLine("success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }



                        connection.Close();
                    }
                }



            }
        }








    }




   




         
}



    


                    //  i++;
         


            /*
            DataSet ds2 = new DataSet();

            //if (xmlElement.Elements() == xmlElement.Element("clear-fraud-insight")){  //xmlElement.Descendants("clear-fraud-insight"))  
            foreach (XElement ele in xmlElement.Descendants("clear-fraud-insight").Descendants("stabilities"))
                {
                   // XmlReader reader = new XmlReader();
                    // XmlTextReader xmlReader = new XmlTextReader(xmlElement.Name);
                    //XmlReader xmlReader = new XmlReader();
                    // if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "clear-credit-risk")
                    //  {
                    //XmlReader inquiryNodes = xmlReader.ReadSubtree();
                    //   ds2.ReadXml(xmlElement);

                    // }
                    ds2.ReadXml(ele.CreateReader());//reader.ReadInnerXml());
                                                    // For each table in the DataSet, print the row values.
                        foreach (DataTable table in ds2.Tables)
                        {

                                Console.WriteLine("table name: " + table);

                                foreach (DataColumn column in table.Columns)
                                {
                                  Console.WriteLine("column name: " + column);
                                }

                        }

                }
          //  }*/
















