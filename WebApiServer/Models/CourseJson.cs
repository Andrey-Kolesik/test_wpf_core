using System;
namespace WebApiServer.Models
{
    public class CourseJson
    {
        public string Currency { get; set; }
        public decimal Value { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
