using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logger.Models
{
    [Table("EventLog")]
    public class EventLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Value { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
