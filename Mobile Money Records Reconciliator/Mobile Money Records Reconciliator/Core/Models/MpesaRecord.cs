using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Models
{
    public class MpesaRecord
    {
        public string ReceiptNo { get; set; }
        public DateTime CompletionTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } //Failed or Succeeded
        public Enums.TransactionType RecordType { get; set; }
        public int Amount { get; set; }
        public int Charges { get; set; }
        public int TotalAmount { get; set; }
        public int Balance { get; set; }
    }
}
