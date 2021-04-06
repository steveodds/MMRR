using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Models
{
    class PDFStatement
    {
        public Guid FileID { get; set; }
        public string StatementPath { get; set; }
        public int PassKey { get; set; }
        public string FileName { get; set; }
    }
}
