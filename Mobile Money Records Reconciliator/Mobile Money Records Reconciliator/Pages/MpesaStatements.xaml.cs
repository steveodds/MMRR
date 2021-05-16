using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mobile_Money_Records_Reconciliator.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MpesaStatements : Page
    {
        private Dictionary<string, object> _pageData;
        public List<Core.Models.MpesaRecord> MpesaRecords { get; set; }

        private Frame MainFrame { get; set; }
        private NavigationView NavView { get; set; }

        public MpesaStatements()
        {
            this.InitializeComponent();
            MpesaRecords = new();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _pageData = (Dictionary<string, object>)e.Parameter;
            MainFrame = (Frame)_pageData["Frame"];
            NavView = (NavigationView)_pageData["NavView"];
            if (NavView is not null)
            {
                NavView.SelectedItem = NavView.MenuItems[3];
                NavView.Header = "Statements";
            }
            var pdfText = (App.Current as App).SharedDataDump;
            if (!string.IsNullOrWhiteSpace(pdfText))
            {
                var generateStatements = new Core.Services.Files.StatementsExtractor(pdfText);
                var fullStatements = generateStatements.GetMpesaRecords().Result;
                MpesaRecords.AddRange(fullStatements.MpesaRecords.Take(5));
                UpdatePageDetails(fullStatements);
            }
        }

        private void UpdatePageDetails(Core.Models.StatementsData fullStatements)
        {
            CustName.Text = fullStatements.CustomerName;
            Mobile.Text = fullStatements.MobileNumber;
            Email.Text = fullStatements.EmailAddress;
            StatementDate.Text = fullStatements.StatementDate.ToShortDateString();
            StatementPeriod.Text = fullStatements.StatementPeriod;

            Sent_Received.Text = fullStatements.TotalSent + " | " + fullStatements.TotalReceived;
            Dep_Withdrawn.Text = fullStatements.TotalDeposited + " | " + fullStatements.TotalWithdrawn;
            Paybill_BuyGoods.Text = fullStatements.PaybillTotal + " | " + fullStatements.BuyGoodsTotal;
            Others.Text = fullStatements.OthersIn + " | " + fullStatements.OthersOut;
            Total.Text = fullStatements.TotalIn + " | " + fullStatements.TotalOut;
        }

        private void dataGrid_AutoGeneratingColumn(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReceiptNo":
                    e.Column.Header = "Receipt No.";
                    break;
                case "CompletionTime":
                    e.Column.Header = "Completion Time";
                    break;
                case "RecordType":
                    e.Column.Header = "Record Type";
                    break;
                case "TotalAmount":
                    e.Column.Header = "Total Amount";
                    break;
                default:
                    break;
            }
        }
    }
}
