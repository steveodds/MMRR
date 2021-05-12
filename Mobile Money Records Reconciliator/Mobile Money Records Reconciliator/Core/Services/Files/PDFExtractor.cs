using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Mobile_Money_Records_Reconciliator.Core.Services.Files
{
    class PDFExtractor
    {
        //TODO: Flesh out
        private Models.PDFStatement pdf;
        public List<Models.MpesaRecord> MpesaRecords { get; set; }

        public PDFExtractor(Models.PDFStatement pdf)
        {
            this.pdf = pdf;
        }

        public async Task<Models.MpesaRecord> ExtractRecordsAsync()
        {
            var results = await GetAllPDFTextAsync(pdf.StatementPath);
            return null;
        }

        public Models.MpesaRecord ExtractSeveralRecords(List<Models.PDFStatement> pDFStatements)
        {
            return null;
        }


        private async Task<StringBuilder> GetAllPDFTextAsync(string pdfPath)
        {
            PdfReader pdfReader;
            await Task.Yield();
            var pageText = new StringBuilder();
            if (pdf.PassKey != 0)
            {
                var readerProperties = new ReaderProperties();
                readerProperties.SetPassword(Encoding.UTF8.GetBytes(pdf.PassKey.ToString()));
                pdfReader = new PdfReader(pdfPath, readerProperties);
            }
            else
            {
                pdfReader = new PdfReader(pdfPath);
            }
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {

                var pageNumbers = pdfDocument.GetNumberOfPages();
                for (int i = 1; i <= pageNumbers; i++)
                {
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(pdfDocument.GetFirstPage());
                    pageText.Append(strategy.GetResultantText());
                }
            }

            return pageText;
        }

    }
}
