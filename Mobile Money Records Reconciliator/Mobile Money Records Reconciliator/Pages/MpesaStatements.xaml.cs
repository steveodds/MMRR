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
            MpesaRecords.AddRange(
                new List<Core.Models.MpesaRecord>
                {
                    new Core.Models.MpesaRecord { Amount = 3200, Balance = 0, Charges = 23, CompletionTime = DateTime.Now, Description = "Goods", ReceiptNo = "RB98675RG", RecordType = Core.Enums.TransactionType.Expense, Status = "Completed", TotalAmount = 3223 },
                    new Core.Models.MpesaRecord { Amount = 768, Balance = 100, Charges = 23, CompletionTime = DateTime.Now.AddDays(-1), Description = "Services", ReceiptNo = "UY98675RG", RecordType = Core.Enums.TransactionType.Expense, Status = "Completed", TotalAmount = 9876 }
                }
                );
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
