using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Models
{
    [NotMapped]
    public class StatementsData
    {
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime StatementDate { get; set; }
        public string StatementPeriod { get; set; }

        public decimal TotalSent { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal PaybillTotal { get; set; }
        public decimal BuyGoodsTotal { get; set; }
        public decimal OthersIn { get; set; }
        public decimal OthersOut { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }


        public List<MpesaRecord> MpesaRecords { get; private set; }

        public StatementsData()
        {
            MpesaRecords = new();
        }
    }
}
