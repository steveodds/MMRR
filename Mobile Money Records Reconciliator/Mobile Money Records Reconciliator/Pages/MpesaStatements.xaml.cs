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

        private Frame MainFrame { get; set; }
        private NavigationView NavView { get; set; }

        public MpesaStatements()
        {
            this.InitializeComponent();
            var statementDump = (App.Current as App).SharedDataDump;
            if (!string.IsNullOrEmpty(statementDump))
                StatementsDisplay.Text = statementDump;
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
    }
}
