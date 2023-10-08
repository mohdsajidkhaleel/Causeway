using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Exceptionlog
    {
        public int Id { get; set; }
        public string? FunctionName { get; set; }
        public string? ControllerName { get; set; }
        public string? ExceptionDetails { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
