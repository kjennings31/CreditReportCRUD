using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace JsonParse
{
    class displayJson
    {

        public string temp { get; set; }
        public string Deserializedjson { get; set; }
        public string serializedjson { get; set; }
        public long signal { get; set; } //create multi. signals to handle longer json. If sjson.signal == -1
        public long signal1 { get; set; }
        public long signal2 { get; set; }

        public long signal3 { get; set; }
        public string negativeIndicator1 { get; set; }
        public string negativeIndicator2 { get; set; }
        public string negativeIndicator3 { get; set; }
        public string negativeIndicator4 { get; set; }
        public string negativeIndicator5 { get; set; }
        public string positiveIndicator1 { get; set; }
        public string positiveIndicator2 { get; set; }
        public string positiveIndicator3 { get; set; }
        public string positiveIndicator4 { get; set; }
        public string positiveIndicator5 { get; set; }
        public string callerTransactionCode { get; set; }
        public string dragnetTransactionId { get; set; }
        public string statusMessage { get; set; }
        






    }
}

