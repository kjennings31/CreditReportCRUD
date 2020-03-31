using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;

namespace JsonParse
{
    class InsertData
    {

        public InsertData(displayJson sjson, long Id, long LeadId)
        {
            string ConString2 = System.Configuration.ConfigurationManager.ConnectionStrings["Connection1"].ConnectionString;
            using (SqlConnection dbconnection = new SqlConnection(ConString2))
            {
                if (sjson != null)
                {
                    //handles the different json formats 
                    if (sjson.signal != -1)
                    {
                        dbconnection.Open();
                        string qstring2 = "INSERT INTO ACCELITASPREPORTS(id,signal,negativeind1,negativeind2,negativeind3,negativeind4,negativeind5,Positiveind1,Positiveind2,Positiveind3,Positiveind4,Positiveind5,CallerTransCode, DragnetTransId,StatusMsg, LeadId)" +
                                "VALUES(" + Id + ", " + sjson.signal + ", '" + sjson.negativeIndicator1 + "','" + sjson.negativeIndicator2 + "','" + sjson.negativeIndicator3 + "','" + sjson.negativeIndicator4 + "','" + sjson.negativeIndicator5 + "','" + sjson.positiveIndicator1 + "','" + sjson.positiveIndicator2 + "','" + sjson.positiveIndicator3 + "','" + sjson.positiveIndicator4 + "','" + sjson.positiveIndicator5 + "','" + sjson.callerTransactionCode + "','" + sjson.dragnetTransactionId + "','" + sjson.statusMessage + "', " + LeadId + ")";
                        using (SqlCommand cmd2 = new SqlCommand(qstring2, dbconnection))
                        {

                            SqlDataAdapter adp = new SqlDataAdapter(qstring2, dbconnection);
                            cmd2.Connection = dbconnection;

                            cmd2.ExecuteNonQuery();


                        }
                    }

                    else
                    {
                        dbconnection.Open();

                        string qstring3 = "INSERT INTO ACCELITASPREPORTS(id,signal,signal1,signal2,signal3,negativeind1,negativeind2,negativeind3,negativeind4,negativeind5,Positiveind1,Positiveind2,Positiveind3,Positiveind4,Positiveind5" +
                            ",CallerTransCode, DragnetTransId,StatusMsg, LeadId)" +
                                "VALUES(" + Id + ", " + sjson.signal + ",  " + sjson.signal1 + "," + sjson.signal2 + "," + sjson.signal3 + ",'" + sjson.negativeIndicator1 + "','" + sjson.negativeIndicator2 + "','" + sjson.negativeIndicator3 + "','" + sjson.negativeIndicator4 + "','" + sjson.negativeIndicator5 + "','" + sjson.positiveIndicator1 + "','" + sjson.positiveIndicator2 + "','" + sjson.positiveIndicator3 + "','" + sjson.positiveIndicator4 + "','" + sjson.positiveIndicator5 + "','" + sjson.callerTransactionCode + "','" + sjson.dragnetTransactionId + "','" + sjson.statusMessage + "', " + LeadId + ")";
                        using (SqlCommand cmd2 = new SqlCommand(qstring3, dbconnection))
                        {
                            SqlDataAdapter adp = new SqlDataAdapter(qstring3, dbconnection);
                            cmd2.Connection = dbconnection;
                            cmd2.ExecuteNonQuery();


                        }

                    }
                }

                else
                {

                    Console.WriteLine("null data found on row where ID = "+ Id);
                    dbconnection.Open();

                    string qstring3 = "INSERT INTO ACCELITASPREPORTS(id)" +
                            "VALUES(" + Id + ")";
                    
                    using (SqlCommand cmd2 = new SqlCommand(qstring3, dbconnection))
                    {
                        SqlDataAdapter adp = new SqlDataAdapter(qstring3, dbconnection);
                        cmd2.Connection = dbconnection;
                        cmd2.ExecuteNonQuery();

                        Console.ReadKey();
                    }
                    dbconnection.Close();
                }
            }

            }
        }
    }



