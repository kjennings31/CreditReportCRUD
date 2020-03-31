using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapCreditData;
using System.IO;
using System.Xml;
using System.Configuration;
using LeapCreditNotifications.Models;
//using LeapCreditLMS.Models;
using System.Data.SqlClient;
using System.Data;
using JsonParse;

namespace LeapCreditParse
{
    public class ParseReport
    {

        public static void Main(string[] args)
        {
            ParseReport obj = new ParseReport();
            //obj.ClarityParse();

            //obj.UpdateBankAccounts();



            //obj.UpdateTrackingNumber();

            //var Fromdate = DateTime.Now.AddDays(-3);
            //var Todate = DateTime.Now;

            //Parsexml parsexmll = new Parsexml(); 

            var MB_FromDate = "2019-11-12";
            var MB_Todate = "2019-11-13";
            obj.MicrobuiltParse(MB_FromDate, MB_Todate);



            var FT_FromDate = "2019-11-12";
            var FT_Todate = "2019-11-13";
            obj.FactorTrustParse(FT_FromDate, FT_Todate);



            var CL_FromDate = "2019-11-12";
            var CL_Todate = "2019-11-12";
            obj.ClarityParse(CL_FromDate, CL_Todate);

            Console.WriteLine(""); 

            var AI_FromDate = "2019-11-12"; 
            var AI_Todate = "2019-11-13";

            GetData gAILifParse = new GetData(AI_FromDate, AI_Todate);
            var Cla_FromDate = "2020-03-17";
            var Cla_Todate = "2020-03-19";
            JsonParse.GetClarityReport Getreport = new JsonParse.GetClarityReport(Cla_FromDate, Cla_Todate);

        }


        //public void ClarityParse(string FromDate, string Todate)
        //{
        //    try
        //    {

        //        Console.WriteLine("ClarityParse:");
        //        ClarityCreditReportParse.Clarity obj = new ClarityCreditReportParse.Clarity();
        //        var result=obj.ParseCreditReport(FromDate, Todate);
        //        Console.WriteLine(result);
        //        Console.WriteLine("End");
        //        Console.Read();

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("ex:" + e.Message);
        //        //Console.Read();
        //    }


        //}


        public void ClarityParse(string FromDate, string Todate)
        {
            try
            {

                Console.WriteLine("ClarityParse:");
                using (var ctx = new LeapCreditLMSEntities())
                {
                    //var date = "2018-11-30";
                    //var Todate = "2018-12-04";
                    var date1 = Convert.ToDateTime(FromDate);
                    var Todate1 = Convert.ToDateTime(Todate);


                    //string GetCreditReport = "select LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate and ReportType='Clarity' and LeadId!=0 and Id>3311783";


                    string GetCreditReport = "select LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate and ReportType='Clarity' and LeadId!=0";
                    //"select * from CreditReport where Leadid in (53401,53400)";
                    //"select LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate and ReportType='Clarity' and LeadId!=0";
                    List<SqlParameter> lstParams1 = new List<SqlParameter>();
                    lstParams1.Add(SQLHelper.AddParameter("@date1", date1, System.Data.SqlDbType.DateTime));
                    lstParams1.Add(SQLHelper.AddParameter("@Todate", Todate1, System.Data.SqlDbType.DateTime));
                   

                    var GetCreditReportlst = SQLHelper.GetDataSet(GetCreditReport, lstParams1, false);
                    //Console.WriteLine("before:");

                    var CreditReports = (from rw in GetCreditReportlst.Tables[0].AsEnumerable()
                                         select new CreditReport()
                                         {
                                             LeadId = Convert.ToInt64(rw["LeadId"]),


                                         }).ToList();
                    

                    long LeadId = 0;
                    int count = 0;

                    foreach (var c in CreditReports)
                    {

                        try
                        {
                            //LeadId = 3456;
                            var loanid = ctx.Loans.Where(x => x.LMSLeadId == c.LeadId).Select(x => x.Id).FirstOrDefault();
                            var borrowerid = ctx.BorrowerDetails.Where(x => x.LoanId == loanid).Select(x => x.BorrowerId).FirstOrDefault();



                            if (LeadId != c.LeadId)
                            {
                                LeadId = c.LeadId ?? 0;
                                //Console.WriteLine("LeadId:" + LeadId);
                                //LeadId = 3456;                               
                                var isInquiry = ctx.CL_Inquiry.Where(x => x.CL_Inquiry_Leadid== LeadId).Any();
                                if (loanid > 0)
                                {
                                    isInquiry = ctx.CL_Inquiry.Where(x => x.CL_Inquiry_LoanId == loanid).Any();
                                }

                                if (!isInquiry)
                                {
                                    count++;
                                    
                                    ClarityCreditReportParse.Clarity obj = new ClarityCreditReportParse.Clarity();
                                    var result=obj.ParseCreditReport(loanid, borrowerid ?? 0, LeadId);
                                   //obj.ParseCreditReport(loanid, borrowerid ?? 0, LeadId);
                                    Console.WriteLine("CLLoanId:" + loanid);
                                    if(result!="")
                                    {
                                        Console.WriteLine("result:" + result);
                                    }
                                    
                                    Console.WriteLine("Completed:" + count);

                                }

                            }
                             //break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("LoanId:" + e.Message);
                            // Console.Read();
                        }






                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ex:" + e.Message);
                //Console.Read();
            }


        }

        public void UpdateBankAccounts()
        {
            try
            {
                using (var ctx = new LeapCreditLMSEntities())
                {
                    var date = "2018-03-25";
                    var Todate = "2018-03-26";
                    var date1 = Convert.ToDateTime(date);
                    var Todate1 = Convert.ToDateTime(Todate);
                    var creditreports = ctx.CreditReports.Where(x => x.CreatedDate >= date1 && x.CreatedDate < Todate1 && x.ReportType == "Clarity").ToList();

                    foreach (var c in creditreports)
                    {
                        var id = c.Id;
                        var loanid = c.LoanId ?? 0;

                        Class1 objdec = new Class1();

                        var report = ctx.CreditReports.Where(x => x.LoanId == 17495 && x.ReportType == "Clarity").FirstOrDefault();
                        var XmlResponse = objdec.Decrypt(report.CreditReport1);

                        var xmlReader = new XmlTextReader(new StringReader(XmlResponse));

                        while (xmlReader.Read())
                        {


                            try
                            {
                                if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "action")
                                {
                                    var x = xmlReader.ReadInnerXml();

                                    if (x == "Deny")
                                    {
                                        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "deny-codes")
                                        {
                                            var desc = xmlReader.ReadInnerXml();

                                        }
                                        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "deny-descriptions")
                                        {
                                            var desc = xmlReader.ReadInnerXml();
                                            //return desc;
                                        }

                                    }
                                    else if (x == "Exception")
                                    {
                                        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "deny-codes")
                                        {
                                            var desc = xmlReader.ReadInnerXml();

                                        }
                                        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "deny-descriptions")
                                        {
                                            var desc = xmlReader.ReadInnerXml();

                                        }
                                        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "exception-descriptions")
                                        {
                                            var desc = xmlReader.ReadInnerXml();
                                            //return desc;
                                        }

                                    }

                                }


                            }
                            catch (Exception ex)
                            {

                            }




                        }










                        var BBaccounts = ctx.CLBB_Accounts.Where(x => x.CLBB_Accounts_LoanId == loanid).FirstOrDefault();
                        // ReadBankAccounts(XmlResponse, BBaccounts);
                        try
                        {
                            xmlReader = new XmlTextReader(new StringReader(XmlResponse));

                            int i = 0;
                            while (xmlReader.Read())
                            {

                                if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "accounts")
                                {

                                    XmlReader ClearbankbehaviourNodes = xmlReader.ReadSubtree();
                                    while (ClearbankbehaviourNodes.Read())
                                    {

                                        if (ClearbankbehaviourNodes.Name == "account")
                                        {

                                            XmlReader ACCNodes = xmlReader.ReadSubtree();
                                            while (ACCNodes.Read())
                                            {
                                                if (i == 1)
                                                {
                                                    if (ACCNodes.Name == "bank-name")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc1_BankName = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                    if (ACCNodes.Name == "account-number")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc1_AccNo = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                }
                                                if (i == 2)
                                                {
                                                    if (ACCNodes.Name == "bank-name")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc2_BankName = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                    if (ACCNodes.Name == "account-number")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc2_AccNo = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                }
                                                if (i == 3)
                                                {
                                                    if (ACCNodes.Name == "bank-name")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc3_BankName = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                    if (ACCNodes.Name == "account-number")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc3_AccNo = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                }
                                                if (i == 4)
                                                {
                                                    if (ACCNodes.Name == "bank-name")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc4_BankName = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                    if (ACCNodes.Name == "account-number")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc4_AccNo = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                }
                                                if (i == 5)
                                                {
                                                    if (ACCNodes.Name == "bank-name")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc5_BankName = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                    if (ACCNodes.Name == "account-number")
                                                    {
                                                        BBaccounts.CLBB_Accounts_Acc5_AccNo = ClearbankbehaviourNodes.ReadInnerXml();
                                                    }
                                                }


                                            }

                                        }
                                        i++;


                                    }



                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        ctx.SaveChanges();
                    }

                    //ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }




        }


        public void UpdateTrackingNumber()
        {
            try
            {
                using (var ctx = new LeapCreditLMSEntities())
                {
                    var date = "2018-08-07";
                    var Todate = "2018-08-28";
                    var date1 = Convert.ToDateTime(date);
                    var Todate1 = Convert.ToDateTime(Todate);
                    var creditreports = ctx.CreditReports.Where(x => x.CreatedDate >= date1 && x.CreatedDate < Todate1 && x.ReportType == "Clarity" && x.Id >= 96663).OrderBy(x => x.Id).ToList();

                    //var creditreport = ctx.CreditReports.Where(x => x.Id==89428).ToList();

                    foreach (var c in creditreports)
                    {
                        var id = c.Id;
                        var loanid = c.LoanId ?? 0;

                        Class1 objdec = new Class1();

                        var report = ctx.CreditReports.Where(x => x.Id == id).FirstOrDefault();
                        var XmlResponse = objdec.Decrypt(report.CreditReport1);

                        var xmlReader = new XmlTextReader(new StringReader(XmlResponse));

                        var ClInquiry = ctx.CL_Inquiry.Where(x => x.CL_Inquiry_CreditReportId == id).FirstOrDefault();

                        if (ClInquiry != null)
                        {


                            while (xmlReader.Read())
                            {


                                try
                                {
                                    if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "tracking-number")
                                    {
                                        var trackingNo = xmlReader.ReadInnerXml();

                                        ClInquiry.CL_Inquiry_TrackingNumber = trackingNo;
                                        ctx.SaveChanges();
                                        break;

                                    }


                                }
                                catch (Exception ex)
                                {

                                    break;
                                }




                            }
                        }



                        //ctx.SaveChanges();
                    }

                    //ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }




        }



        public void MicrobuiltParse(string FromDate, string Todate)
        {
            try
            {
                Console.WriteLine("Microbiltparse");
                using (var ctx = new LeapCreditLMSEntities())
                {
                    //var date = "2018-07-01";
                    //var Todate = "2018-07-16";
                    //var date1 = Convert.ToDateTime(date);
                    //var Todate1 = Convert.ToDateTime(Todate);
                    var date1 = Convert.ToDateTime(FromDate);
                    var Todate1 = Convert.ToDateTime(Todate);


                    string GetCreditReport = "select LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate and ReportType='Microbilt' and LeadId!=0";
                    List<SqlParameter> lstParams1 = new List<SqlParameter>();
                    lstParams1.Add(SQLHelper.AddParameter("@date1", date1, System.Data.SqlDbType.DateTime));
                    lstParams1.Add(SQLHelper.AddParameter("@Todate", Todate1, System.Data.SqlDbType.DateTime));
                    var GetCreditReportlst = SQLHelper.GetDataSet(GetCreditReport, lstParams1, false);


                    var CreditReports = (from rw in GetCreditReportlst.Tables[0].AsEnumerable()
                                         select new CreditReport()
                                         {
                                             LeadId = Convert.ToInt64(rw["LeadId"]),

                                         }).ToList();






                    //var creditreports = ctx.CreditReports.Where(x => x.CreatedDate >= date1 && x.CreatedDate < Todate1 && x.ReportType == "Microbilt" /*&& x.WaterfallServiceId!=null*/).ToList();
                    //var creditreports = ctx.CreditReports.Where(x => x.LoanId == 166293 && x.ReportType == "Clarity").ToList();
                    long LeadId = 0;

                    foreach (var c in CreditReports)
                    {
                        try
                        {
                            LeadId = c.LeadId ?? 0;
                            if (LeadId != 0)
                            {
                                var loanid = ctx.Loans.Where(x => x.LMSLeadId == c.LeadId).Select(x => x.Id).FirstOrDefault();
                                var borrowerid = ctx.BorrowerDetails.Where(x => x.LoanId == loanid).Select(x => x.BorrowerId).FirstOrDefault();

                                //Console.WriteLine("MBLeadId: " + LeadId);
                                //var borrowerid = c.BorrowerId ?? 0;
                                //loanid = c.LoanId ?? 0;
                                var isMB = ctx.MB_Content.Where(x => x.Leadid == LeadId).Any();
                                if(loanid>0)
                                 isMB = ctx.MB_Content.Where(x => x.LoanId == loanid).Any();

                                if (!isMB)
                                {
                                    MicrobiltParse.Microbilt objMB = new MicrobiltParse.Microbilt();
                                    objMB.ParseMBCreditReport(loanid, borrowerid ?? 0, LeadId);
                                    Console.WriteLine("MBLoanid: " + loanid);
                                }
                            }

                        }
                        catch (Exception e)
                        {

                        }






                    }


                }
            }
            catch (Exception e)
            {

            }
        }

        public void FactorTrustParse(string FromDate, string Todate)
        {
            try
            {

                Console.WriteLine("FactorTrust");
                using (var ctx = new LeapCreditLMSEntities())
                {
                    //var date = "2018-07-01";
                    //var Todate = "2018-07-16";
                    //var date1 = Convert.ToDateTime(date);
                    //var Todate1 = Convert.ToDateTime(Todate);
                    var date1 = Convert.ToDateTime(FromDate);
                    var Todate1 = Convert.ToDateTime(Todate);

                    string GetCreditReport = "select LeadId from CreditReport where CreatedDate>=@date1 and CreatedDate<@Todate and ReportType='FactorTrust' and LeadId!=0";
                    List<SqlParameter> lstParams1 = new List<SqlParameter>();
                    lstParams1.Add(SQLHelper.AddParameter("@date1", date1, System.Data.SqlDbType.DateTime));
                    lstParams1.Add(SQLHelper.AddParameter("@Todate", Todate1, System.Data.SqlDbType.DateTime));
                    var GetCreditReportlst = SQLHelper.GetDataSet(GetCreditReport, lstParams1, false);


                    var CreditReports = (from rw in GetCreditReportlst.Tables[0].AsEnumerable()
                                         select new CreditReport()
                                         {
                                             LeadId = Convert.ToInt64(rw["LeadId"]),

                                         }).ToList();








                    //var creditreports = ctx.CreditReports.Where(x => x.CreatedDate >= date1 && x.CreatedDate < Todate1 && x.ReportType == "FactorTrust").ToList();
                    //var creditreports = ctx.CreditReports.Where(x => x.LoanId == 166293 && x.ReportType == "Clarity").ToList();
                    long LeadId = 0;
                    // LeadId = 1419242;
                    foreach (var c in CreditReports)
                    {
                        //Console.WriteLine("foreach");
                        try
                        {
                            LeadId = c.LeadId ?? 0;
                           // Console.WriteLine("LeadId:" +LeadId);
                            if(LeadId!=0)
                            {
                                var loanid = ctx.Loans.Where(x => x.LMSLeadId == c.LeadId).Select(x => x.Id).FirstOrDefault();
                                var borrowerid = ctx.BorrowerDetails.Where(x => x.LoanId == loanid).Select(x => x.BorrowerId).FirstOrDefault();
                                var isFT = ctx.FT_Login_TransactionInfo.Where(x => x.LoanId == loanid).Any();
                                if (!isFT)
                                {
                                    FatorTrustParse.FactorTrust objFT = new FatorTrustParse.FactorTrust();
                                    var result=objFT.ParseFTCreditReport(loanid, borrowerid ?? 0, LeadId);
                                    Console.WriteLine("FTLoanid : " + loanid);
                               // Console.WriteLine(result);

                                }
                            }
                           

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception :"+ e.Message);
                        }

                   }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Console.Read();
        }

        public void EncryptCreditReport()
        {
            try
            {
                Crypto Obj = new Crypto();
                var Key = System.Configuration.ConfigurationManager.AppSettings["SensitiveKey"].ToString();
                using (var ctx = new LeapCreditLMSEntities())
                {
                    Console.WriteLine("Checking Credit Reports for Encryption");
                    var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                    var Fromdate = "2018-12-07";
                    var Startdate = Convert.ToDateTime(Fromdate).Date;

                    while (Startdate <= Convert.ToDateTime(Fromdate).Date)
                    {

                        //List<SqlParameter> lstParams1 = new List<SqlParameter>();
                        //lstParams1.Add(SQLHelper.AddParameter("@yesterday", Startdate, System.Data.SqlDbType.DateTime));
                        //string GetCreditReport = "select top 1 CreditReport from CreditReport where cast(CreatedDate as date)=@yesterday";
                        //var GetCreditReport1st = SQLHelper.ExecuteQuery(GetCreditReport, lstParams1, false);


                        //List<SqlParameter> lstParams1 = new List<SqlParameter>();
                        //lstParams1.Add(SQLHelper.AddParameter("@yesterday", yesterday, System.Data.SqlDbType.DateTime));
                        //string GetCreditReport = "select top 1 CreditReport from CreditReport where cast(CreatedDate as date)=@yesterday";
                        //var GetCreditReport1st = SQLHelper.ExecuteQuery(GetCreditReport, lstParams1, false);

                        //if (GetCreditReport1st.Contains("<?xml"))
                        //{
                        Console.WriteLine("Encryption of Credit Report Started");
                        List<SqlParameter> lstParam = new List<SqlParameter>();
                        lstParam.Add(SQLHelper.AddParameter("@yesterday", Startdate, System.Data.SqlDbType.DateTime));
                        string GetCreditReportList = @"With CTE as
                            (
                            select Id, CreditReport,'' as EncryptedCreditReport, ROW_NUMBER() over(order by id) as RowNo 
                            from creditreport where cast(createddate as date) = @yesterday
                            )
                            Select Id, CreditReport,EncryptedCreditReport from CTE Where RowNo <= 100";
                        var CreditReportList = SQLHelper.GetDataSet(GetCreditReportList, lstParam, false);
                        int iCount = 1, StartNo = 0, EndNo = 0;
                        while (CreditReportList.Tables[0].Rows.Count != 0)
                        {
                            Console.WriteLine("Count " + CreditReportList.Tables[0].Rows.Count);
                            foreach (DataRow dr in CreditReportList.Tables[0].Rows)
                            {

                                dr["EncryptedCreditReport"] = dr["CreditReport"].ToString().Contains("<?xml") ? Obj.Encrypt(Convert.ToString(dr["CreditReport"]))
                                    : dr["CreditReport"].ToString();
                            }

                            CreditReportList.Tables[0].Columns.RemoveAt(1);

                            Console.WriteLine("After encryption...");
                            string Truncate = "TRUNCATE TABLE EncryptCreditReportTemp";
                            SQLHelper.ExecuteQuery(Truncate, new List<SqlParameter>(), false);

                            using (SqlConnection scon = SQLHelper.GetConnectionString(false))
                            {
                                scon.Open();

                                using (SqlBulkCopy sbc = new SqlBulkCopy(scon))
                                {
                                    sbc.DestinationTableName = "EncryptCreditReportTemp";
                                    sbc.BatchSize = 5000;
                                    sbc.WriteToServer(CreditReportList.Tables[0]);
                                    sbc.Close();
                                }
                            }

                            string EncryptUpdate = "Update CreditReport set CreditReport=t1.EncryptedCreditReport from CreditReport Inner Join EncryptCreditReportTemp t1 on CreditReport.Id=t1.CreditReportId";
                            SQLHelper.ExecuteQuery(EncryptUpdate, new List<SqlParameter>(), false);
                            Console.WriteLine("Encryption Completed");
                            StartNo = (iCount * 100) + 1;
                            EndNo = (StartNo + 99);
                            lstParam = new List<SqlParameter>();
                            lstParam.Add(SQLHelper.AddParameter("@yesterday", Startdate, System.Data.SqlDbType.DateTime));
                            GetCreditReportList = @"With CTE as
                            (
                            select Id, CreditReport,'' as EncryptedCreditReport, ROW_NUMBER() over(order by id) as RowNo 
                            from creditreport where cast(createddate as date) = @yesterday
                            )
                            Select Id, CreditReport,EncryptedCreditReport from CTE Where RowNo Between " + StartNo.ToString() + " AND " + EndNo.ToString();
                            CreditReportList = SQLHelper.GetDataSet(GetCreditReportList, lstParam, false);

                            iCount += 1;
                        }
                        //}
                        //else
                        //{
                        //    Console.WriteLine("These Credit Reports are already Encrypted");
                        //}



                        Startdate = Startdate.AddDays(1);
                       
                    }

                    Console.Read();






                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e.Message);
                Console.Read();
            }


        }



        public class CreditReportData
        {
            public long Id { get; set; }
            public string CreditReport { get; set; }
            public string EncryptedCreditReport { get; set; }

        }
    
    
    }
}
