using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using NHibernate.Mapping;
using System.Xml.Linq;
using System.Xml;

namespace JsonParse
{
    class GetClarityReport
    {

        public GetClarityReport(string FromDate, string Todate)
        {
            Console.WriteLine("New Clarity Parse:");
            var date1 = Convert.ToDateTime(FromDate);
            var Todate1 = Convert.ToDateTime(Todate);
         
            


            //Gets the last record entered into credit reports table
            GetLastRecordClarity getLastRecord = new GetLastRecordClarity();
            int LastIdVal = getLastRecord.GetLastRecordCL();
            //Uses last record entered id to insert only the new records
            if (LastIdVal == 0) // assumes that Accelitas table has no records .. gets all available records from credit reports table within date range to be inserted into Accelitas table
            {
                int countrecords = 0;

                string ConString1 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
                using (SqlConnection dbconnection = new SqlConnection(ConString1))
                {
                    dbconnection.Open();
                    string qstring = "select borrowerid, creditreport, id, LeadId, loanid from CreditReport where Leadid in (94479) and ReportType = 'clarity'";
                    //"select borrowerid, creditreport, id, LeadId from CreditReport where  BorrowerId = 88888"; 

                    // CreatedDate>=@date1 and CreatedDate<@Todate1 and ReportType= 'clarity' and leadid !=0";

                    using (SqlCommand cmd = new SqlCommand(qstring))
                    {
                        cmd.Parameters.AddWithValue("@date1", SqlDbType.DateTime).Value = date1;
                        cmd.Parameters.AddWithValue("@Todate1", SqlDbType.DateTime).Value = Todate1;

                        SqlDataAdapter adp = new SqlDataAdapter(qstring, dbconnection);
                        cmd.Connection = dbconnection;
                        //sql data reader 
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                //loops until no more records available to be read
                                while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                    {

                                      //  XElement Temp = XElement.Load(reader.GetTextReader(1).ToString());


                                        XElement Temp = XElement.Parse(reader.GetString(1).ToString());


                                        string BorrowerId;
                                        string LeadId;
                                        string Id;
                                        string loanid;

                                        BorrowerId = reader.GetInt64(0).ToString();
                                        Id = reader.GetInt32(2).ToString();
                                        LeadId = reader.GetInt64(3).ToString();
                                        loanid = reader.GetInt64(4).ToString();
                                        Console.WriteLine("data gathered");

                                        Console.WriteLine(Temp);

                                        DataSet ds = new DataSet();
                                        
                                        LeapCreditParse.Parsexml XmlParseClarity = new LeapCreditParse.Parsexml(Id, Temp, BorrowerId, LeadId);

                                        countrecords++;
                                    }

                                    else
                                    {
                                        XElement Temp = XElement.Load(reader.GetString(1));

                                        //Console.WriteLine(Temp);

                                        string BorrowerId;
                                        string LeadId;
                                        string Id;
                                        string loanid;


                                        BorrowerId = reader.GetInt64(0).ToString();
                                        Id = reader.GetInt32(2).ToString();
                                        LeadId = reader.GetInt64(3).ToString();
                                        loanid = reader.GetInt64(4).ToString();
                                        Console.WriteLine("data gathered");
                                    


                                        LeapCreditParse.Parsexml XmlParseClarity = new LeapCreditParse.Parsexml(Id, Temp, LeadId);
                                       

                                    }
                                    //inserts records into Accelitas table
                                    //InsertData NewInsert = new InsertData(sjson, Id, LeadId);

                                }
                            }
                            Console.WriteLine(countrecords + " new records entered, press any key to continue");


                        }


                    }

                    dbconnection.Close();
                }
            }


            else //If  table has records... only gets records from credit reports table within date range and where id is greater than the laast inserted record. 
            {

                //Get new data where id value is greater than last id value 

                int countrecords_ = 0;
                string ConString1 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
                using (SqlConnection dbconnection = new SqlConnection(ConString1))
                {
                    dbconnection.Open();


                    string qstring = "select borrowerid, creditreport, id, LeadId, loanid from CreditReport where id = 50";
                    // "select borrowerid, creditreport, id, LeadId, loanid from CreditReport where Leadid in (94479) and ReportType = 'clarity'";
                    //"select borrowerid, creditreport, id, LeadId, loanid from CreditReport where id = 50";
                    // "select borrowerid, creditreport, id, LeadId, loanid from CreditReport where Leadid in (94479) and ReportType = 'clarity'";
                    //"select creditreport, id, LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate1 and ReportType= 'ailift' and id> " + LastIdVal + "";


                    using (SqlCommand cmd = new SqlCommand(qstring))
                    {
                        cmd.Parameters.AddWithValue("@date1", SqlDbType.DateTime).Value = date1;
                        cmd.Parameters.AddWithValue("@Todate1", SqlDbType.DateTime).Value = Todate1;


                        cmd.Connection = dbconnection;
                        //data reader
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                //loops until no more records available to be read
                                while (reader.Read() && reader != null)
                                {
                                    if (!reader.IsDBNull(0))
                                    {

                                        


                                        XElement Temp = XElement.Parse(reader.GetString(1).ToString());


                                        string BorrowerId;
                                        string LeadId;
                                        string Id;
                                        string loanid;

                                        BorrowerId = reader.GetInt64(0).ToString();
                                        Id = reader.GetInt32(2).ToString();
                                        LeadId = reader.GetInt64(3).ToString();
                                        loanid = reader.GetInt64(4).ToString();
                                        Console.WriteLine("data gathered");

                                     

                                        LeapCreditParse.Parsexml XmlParseClarity = new LeapCreditParse.Parsexml(Id, Temp, BorrowerId, LeadId);

                                        countrecords_++;
                                    }

                                    else
                                    {
                                        XElement Temp = XElement.Load(reader.GetTextReader(1));




                                        
                                        string LeadId;
                                        string Id;
                                        string loanid;

                                       
                                        Id = reader.GetInt32(2).ToString();
                                        LeadId = reader.GetInt64(3).ToString();
                                        loanid = reader.GetInt64(4).ToString();
                                        Console.WriteLine("data gathered");
                                     


                                       LeapCreditParse.Parsexml XmlParseClarity = new LeapCreditParse.Parsexml(Id, Temp,LeadId);

                                        countrecords_++;




                                    }
                                }
                                Console.WriteLine(countrecords_ + " new records entered, press any key to continue");
                                Console.ReadKey();

                            }


                        }

                        dbconnection.Close();
                    }
                }


            }

        }

    }
}
