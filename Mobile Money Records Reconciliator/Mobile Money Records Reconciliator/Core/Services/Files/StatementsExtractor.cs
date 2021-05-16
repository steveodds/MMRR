using Mobile_Money_Records_Reconciliator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Services.Files
{
    public class StatementsExtractor
    {
        private readonly string _pdfText;
        private List<Models.MpesaRecord> MpesaRecords { get; set; }
        public Models.StatementsData FullStatements { get; set; }
        public StatementsExtractor(string pdfText)
        {
            _pdfText = pdfText;
            MpesaRecords = new();
            FullStatements = new();
        }


        public List<Models.MpesaRecord> GetMpesaRecords()
        {
            if (string.IsNullOrEmpty(_pdfText))
                throw new ArgumentNullException("The provided text was empty. No statements can be extracted.");

            FullStatements = GenerateStatement();

            return null;
        }

        private StatementsData GenerateStatement()
        {
            return null;
        }
    }
}
