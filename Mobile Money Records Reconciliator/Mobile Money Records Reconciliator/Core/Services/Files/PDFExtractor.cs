using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Services.Files
{
    class PDFExtractor
    {
        private Models.PDFStatement pdf;
        public List<Models.MpesaRecord> MpesaRecords { get; set; }

        public PDFExtractor(Models.PDFStatement pdf)
        {
            this.pdf = pdf;
        }



    }
}
