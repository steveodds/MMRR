using Mobile_Money_Records_Reconciliator.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Services.Files
{
    public class StatementsExtractor
    {
        private readonly string _pdfText;
        private List<Models.MpesaRecord> MpesaRecords { get; set; }
        private string _lastTransactionNo;
        public Models.StatementsData FullStatements { get; set; }
        public StatementsExtractor(string pdfText)
        {
            _pdfText = pdfText;
            MpesaRecords = new();
            FullStatements = new();
        }


        public async Task<StatementsData> GetMpesaRecords()
        {
            if (string.IsNullOrEmpty(_pdfText))
                throw new ArgumentNullException("The provided text was empty. No statements can be extracted.");

            FullStatements = await GenerateStatement();
            FullStatements.MpesaRecords.AddRange(MpesaRecords);
            return FullStatements;
        }

        private async Task<StatementsData> GenerateStatement()
        {
            var isSummarySet = false;
            var isStatementSet = false;
            var isStatementHeader = true;
            var text = _pdfText;
            var reader = new StringReader(text);
            var line = string.Empty;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.ToLower().Contains("summary") && !isSummarySet)
                {
                    isSummarySet = true;
                    continue;
                }
                if (line.ToLower().Contains("detailed statement") && !isStatementSet)
                {
                    isStatementSet = true;
                    continue;
                }

                if (!isSummarySet && !isStatementSet)
                {
                    //Get customer details
                    SetCustomerDetails(line);
                }
                else if (isSummarySet && !isStatementSet)
                {
                    //Get statement summary
                    SetSummaryDetails(line);
                }
                else if (isStatementSet)
                {
                    //Get detailed transactions
                    if(isStatementHeader)
                    {
                        var current = line.ToLower();
                        isStatementHeader = current.Contains("receipt no.") || current.Contains("status");
                        if (isStatementHeader)
                            continue;
                    }
                    SetStatementDetails(line);
                }
            }

            return FullStatements;
        }

        private void SetCustomerDetails(string line)
        {
            switch (line)
            {
                case string a when a.ToLower().Contains("customer"):
                    FullStatements.CustomerName = line.Remove(0, 13).Trim();
                    break;
                case string b when b.ToLower().Contains("mobile"):
                    FullStatements.MobileNumber = line.Remove(0, 13).Trim();
                    break;
                case string c when c.ToLower().Contains("email"):
                    FullStatements.EmailAddress = line.Remove(0, 13).Trim();
                    break;
                case string d when d.ToLower().Contains("date"):
                    FullStatements.StatementDate = DateTime.Parse(line.Remove(0, 17).Trim());
                    break;
                case string e when e.ToLower().Contains("period"):
                    FullStatements.StatementPeriod = line.Remove(0, 16).Trim();
                    break;
                default:
                    break;
            }
        }

        private void SetSummaryDetails(string line)
        {
            string temp;
            switch (line)
            {
                case string a when a.ToLower().Contains("send"):
                    {
                        temp = line.Remove(0, 11).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.TotalSent = decimal.Parse(amounts[1].Trim()) - decimal.Parse(amounts[0].Trim());
                        break;
                    }
                case string b when b.ToLower().Contains("received"):
                    {
                        temp = line.Remove(0, 15).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.TotalReceived = decimal.Parse(amounts[0].Trim()) - decimal.Parse(amounts[1].Trim());
                        break;
                    }
                case string c when c.ToLower().Contains("deposit"):
                    {
                        temp = line.Remove(0, 13).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.TotalDeposited = decimal.Parse(amounts[0].Trim()) - decimal.Parse(amounts[1].Trim());
                        break;
                    }
                case string d when d.ToLower().Contains("withdrawal"):
                    {
                        temp = line.Remove(0, 17).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.TotalWithdrawn = decimal.Parse(amounts[1].Trim()) - decimal.Parse(amounts[0].Trim());
                        break;
                    }
                case string e when e.ToLower().Contains("paybill"):
                    {
                        temp = line.Remove(0, 25).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.PaybillTotal = decimal.Parse(amounts[1].Trim()) - decimal.Parse(amounts[0].Trim());
                        break;
                    }
                case string f when f.ToLower().Contains("goods"):
                    {
                        temp = line.Remove(0, 27).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.BuyGoodsTotal = decimal.Parse(amounts[1].Trim()) - decimal.Parse(amounts[0].Trim());
                        break;
                    }
                case string g when g.ToLower().Contains("others"):
                    {
                        temp = line.Remove(0, 7).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.OthersIn = decimal.Parse(amounts[0].Trim());
                        FullStatements.OthersOut = decimal.Parse(amounts[1].Trim());
                        break;
                    }
                case string h when h.ToLower().Contains("total"):
                    {
                        temp = line.Remove(0, 5).Trim();
                        string[] amounts = temp.Split(' ');
                        FullStatements.TotalIn= decimal.Parse(amounts[0].Trim());
                        FullStatements.TotalOut = decimal.Parse(amounts[1].Trim());
                        break;
                    }
                default:
                    break;
            }
        }


        private void SetStatementDetails(string line)
        {
            var transaction = new MpesaRecord();
            string tempLine;
            if(!line.Any(char.IsDigit)|| line.Length < 10 || line.Substring(0, 11).Trim().Length != 10)
            {
                //Handle overflown transactions
                var lastTransaction = MpesaRecords.First(x => x.ReceiptNo == _lastTransactionNo);
                lastTransaction.Description = lastTransaction.Description + " " + line.Trim();
                return;
            }
            else
            {
                transaction.ReceiptNo = line.Substring(0, 10);
                _lastTransactionNo = transaction.ReceiptNo;
                tempLine = line.Remove(0, 10).Trim();
            }

            //Get date
            var dateString = tempLine.Substring(0, 19).Trim();
            transaction.CompletionTime = DateTime.Parse(dateString);
            tempLine = tempLine.Remove(0, 19).Trim();

            //Get Details
            if (tempLine.ToLower().Contains("completed"))
            {
                var details = tempLine.Substring(0, tempLine.ToLower().IndexOf("completed")).Trim();
                transaction.Description = details;
                transaction.Status = "Completed";
                tempLine = tempLine.Remove(0, tempLine.ToLower().IndexOf("completed") + 9).Trim();
            }
            else if (tempLine.ToLower().Contains("failed"))
            {
                var details = tempLine.Substring(0, tempLine.ToLower().IndexOf("failed")).Trim();
                transaction.Description = details;
                transaction.Status = "Failed";
                tempLine = tempLine.Remove(0, tempLine.ToLower().IndexOf("failed") + 6).Trim();
            }
            else
            {
                throw new ArgumentException($"Could not process transaction '{transaction.ReceiptNo}'. Please contact the developer.");
            }

            //Get transaction amounts
            var amounts = tempLine.Split(' ');
            if (amounts[0].Contains("-"))
            {
                transaction.Amount = decimal.Parse(amounts[0].Remove(0, 1));
                transaction.RecordType = Enums.TransactionType.Expense;
            }
            else
            {
                transaction.Amount = decimal.Parse(amounts[0]);
                transaction.RecordType = Enums.TransactionType.Expense;
            }
            transaction.Balance = decimal.Parse(amounts[1]);
            MpesaRecords.Add(transaction);
        }

    }
}
