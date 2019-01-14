using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUT14_Logging_AuditTrails.Models
{
    public class HomeHontrollerViewModel
    {
        public string UserId { get; internal set; }
        public string LogId { get; internal set; }
        public string LogCreatedOn { get; internal set; }
    }
}