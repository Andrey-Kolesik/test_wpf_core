using System;

namespace WebApiServer.Models
{
    public class Course
    {
        public string Cur_Abbreviation { get; set; }
        public decimal Cur_OfficialRate { get; set; }
        public int Cur_Scale { get; set; }
        public int Cur_ID { get; set; }
        public DateTime Cur_DateStart { get; set; }
        public DateTime Cur_DateEnd { get; set; }
        public DateTime Date { get; set; }
    }
}
