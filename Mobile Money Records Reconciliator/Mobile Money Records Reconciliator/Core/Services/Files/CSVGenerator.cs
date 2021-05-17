using Mobile_Money_Records_Reconciliator.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Services.Files
{
    class CSVGenerator
    {
        private readonly List<MpesaRecord> _records;
        private readonly string MPESA_EXPORT;

        public CSVGenerator(List<Models.MpesaRecord> records)
        {
            MPESA_EXPORT = GenerateExportPath();
            _records = records;
        }


        public async Task<Models.FinalFile> GetCSV()
        {
            if (_records is null)
                throw new ArgumentException("There are no records to export.");

            FinalFile finalFile = await GenerateCSV();

            return finalFile;
        }
        private string GenerateExportPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMRR\Exports";
            Directory.CreateDirectory(path);
            return path;
        }

        private async Task<FinalFile> GenerateCSV()
        {
            var finalFile = $@"{MPESA_EXPORT}\mpesa_records.csv";
            File.Create(finalFile);
            //Add headers
            using (StreamWriter writer = File.CreateText(finalFile))
            {
                writer.WriteLine("Receipt No., Completion Time, Description, Status, Transaction Type, Amount, Charges, Total Amount, Balance,");
            }

            //Add content
            using (StreamWriter writer = File.AppendText(finalFile))
            {
                foreach (var record in _records)
                {
                    await writer.WriteLineAsync(record.ToString());
                }
            }

            return new FinalFile() { FileName = "mpesa_records.csv", FinalFilePath = finalFile };
        }
    }
}
