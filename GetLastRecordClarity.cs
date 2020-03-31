using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;
using System.Timers;
using System.Threading.Tasks;
using System.IO;

namespace JsonParse
{


    
    class GetLastRecordClarity
    {
        private int Id { get; set; }
        //string path = @"C:\Users\KurtJennings\Desktop\Credit Reports Parse\ProcessEvents.txt";
        public int GetLastRecordCL()
        {


            
            string ConString3 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
            using (SqlConnection dbconnection = new SqlConnection(ConString3))
            {
                dbconnection.Open();
                string qstring4 = "SELECT TOP 1 id FROM creditreport where reporttype = 'clarity' ORDER BY ID DESC";
                using (SqlCommand cmd = new SqlCommand(qstring4))
                {

                    
                    cmd.Connection = dbconnection;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            
                                reader.Read();
                                Id = reader.GetInt32(0);
                                reader.Close();
                               Console.Write("Last record entered Id:" + Id + "\nAll new records will be inserted into Clarity Tables.");
                               //string Txt2file2 = "Last record entered record Id:" + Id + "\nAll new records will be inserted into Accelitas Table.";
                               // File.AppendAllText(path, Txt2file2 + Environment.NewLine);
                                dbconnection.Close();

                            return Id;


                        }
                        else
                        {
                            Console.WriteLine("Clarity Table currently has no records. Starting record insertion process");
                          //  string Txt2file = "Accelitas Table currently has no records. Starting record insertion process.";
                          //  File.AppendAllText(path, Txt2file + Environment.NewLine);
                            
                            return 0;

                        } 

                    }
                    
                }
            }
           
        }
    }
}
