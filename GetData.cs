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


namespace JsonParse
{
    class GetData
    {

        public GetData(string FromDate, string Todate)
        {
            Console.WriteLine("Accelitas Parse:");
            var date1 = Convert.ToDateTime(FromDate);
            var Todate1 = Convert.ToDateTime(Todate);
            
           
            //Gets the last record entered into credit reports table
            GetLastRecord getLastRecord= new GetLastRecord();
            int LastIdVal = getLastRecord.GetLast();
            //Uses last record entered id to insert only the new records
            if (LastIdVal == 0) // assumes that Accelitas table has no records .. gets all available records from credit reports table within date range to be inserted into Accelitas table
            {
                int countrecords = 0;

                string ConString1 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
                using (SqlConnection dbconnection = new SqlConnection(ConString1))
                {
                    dbconnection.Open();
                    string qstring = "select creditreport, id, LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate1 and ReportType= 'ailift' and leadid !=0"; 
                   
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
                                while (reader.Read() && reader != null)
                                {
                                    string Temp;
                                    long LeadId;
                                    long Id;
                                    Temp = reader.GetString(0);
                                    Id = reader.GetInt32(1);
                                    LeadId = reader.GetInt64(2);


                                    //json deserializer 
                                    displayJson sjson = JsonConvert.DeserializeObject<displayJson>(Temp);


                                    countrecords++;

                                    //inserts records into Accelitas table
                                    InsertData NewInsert = new InsertData(sjson, Id, LeadId);

                                }
                            }
                            Console.WriteLine(countrecords + " new records entered, press any key to continue");
                            

                        }


                    }

                    dbconnection.Close();
                }
            }


            else //If Accelitas table has records... only gets records from credit reports table within date range and where id is greater than the laast inserted record. 
            {

                //Get new data where id value is greater than last id value 
               
                int countrecords_ = 0;
                string ConString1 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
                using (SqlConnection dbconnection = new SqlConnection(ConString1))
                {
                    dbconnection.Open();
                 

                    string qstring = "select creditreport, id, LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate1 and ReportType= 'ailift' and id> " + LastIdVal + "";
                    
               
                    using (SqlCommand cmd = new SqlCommand(qstring))
                    {
                        cmd.Parameters.AddWithValue("@date1", SqlDbType.DateTime).Value=date1;
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
                                    string Temp;
                                    long LeadId;
                                    long Id;
                                    Temp = reader.GetString(0);
                                    Id = reader.GetInt32(1);
                                    LeadId = reader.GetInt64(2);


                                    //json deserializer
                                    displayJson sjson = JsonConvert.DeserializeObject<displayJson>(Temp);

                                    

                                    //inserts records into Accelitas table
                                    InsertData NewInsert = new InsertData(sjson, Id, LeadId);
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


  

       
    



    

