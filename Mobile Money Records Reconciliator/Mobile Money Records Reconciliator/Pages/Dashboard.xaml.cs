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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using muxc = Microsoft.UI.Xaml.Controls;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mobile_Money_Records_Reconciliator.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        //Required models
        private Core.Models.PDFStatement statement;
        private Dictionary<string, object> _pageData;

        //End
        private Frame MainFrame;
        private NavigationView NavView;
        public Dashboard()
        {
            this.InitializeComponent();
            statement = new Core.Models.PDFStatement();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _pageData = (Dictionary<string, object>)e.Parameter;
            MainFrame = (Frame)_pageData["Frame"];
            NavView = (NavigationView)_pageData["NavView"];
            if(NavView is not null)
            {
                NavView.SelectedItem = NavView.MenuItems[0];
                NavView.Header = "Dashboard";
            }
        }

        //Core Functionality
        private void SavePDFDetails(string pdfPath)
        {
            //TODO: Generate PDF model
        }

        private void SaveMultiplePDFs(List<string> pdfPaths)
        {
            //TODO: Generate Multiple PDF Models
        }

        private void PDFDetailsToDB(Core.Models.PDFStatement pdfStatement)
        {
            //TODO: Save to DB
        }

        private async void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Windows.Storage.Pickers.FileOpenPicker();

            //Make folder Picker work in Win32
            IntPtr windowHandle = (App.Current as App).WindowHandle;
            var initializeWithWindow = fileDialog.As<IInitializeWithWindow>();

            fileDialog.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            initializeWithWindow.Initialize(windowHandle);
            fileDialog.FileTypeFilter.Add(".pdf");
            var pickedFile = await fileDialog.PickSingleFileAsync();
            if (pickedFile is not null)
            {
                statement.FileName = pickedFile.Name;
                statement.StatementPath = pickedFile.Path;
                UpdateFiles(windowHandle);
            }
        }

        private async void UpdateFiles(IntPtr windowHandle)
        {
            statement.PassKey = await GetPasskey();
            var extractor = new Core.Services.Files.PDFExtractor(statement);
            var result = await extractor.ExtractRecordsAsync();

            (App.Current as App).SharedDataDump = result;


            if (!Docs.IsEnabled)
            {
                Docs.IsEnabled = true;
                Docs.Items.Clear();
            }
            Docs.Items.Add(statement.FileName);

            if (MainFrame is not null)
                MainFrame.Navigate(typeof(MpesaStatements), _pageData);

        }

        

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        private async Task<int> GetPasskey()
        {


            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                GetPin.XamlRoot = DashboardGrid.XamlRoot;
            }
            var userResponse = await GetPin.ShowAsync();
            if (userResponse == ContentDialogResult.Primary)
            {
                var pin = int.Parse(Passkey.Password.ToString());
                Passkey.Password = string.Empty;
                return pin;
            }

            return -1;
        }
        private async void ClearDocumentHistory_Click(object sender, RoutedEventArgs e)
        {
            if (Docs.IsEnabled)
            {
                ContentDialog fileLoadedDialog = new ContentDialog
                {
                    Title = "Clear Document History",
                    Content = $"Are you sure you want to clear your document history? This action is not reversible!",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    DefaultButton = ContentDialogButton.Close
                };

                if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
                {
                    fileLoadedDialog.XamlRoot = DashboardGrid.XamlRoot;
                }
                var userResponse = await fileLoadedDialog.ShowAsync();
                if (userResponse == ContentDialogResult.Primary)
                {
                    Docs.Items.Clear();
                    Docs.Items.Add("You have no previous documents...");
                    Docs.IsEnabled = false;
                    var finalDialog = new ContentDialog();
                    finalDialog.Content = "History cleared";
                    finalDialog.Title = fileLoadedDialog.Title;
                    finalDialog.CloseButtonText = "Close";
                    if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
                        finalDialog.XamlRoot = DashboardGrid.XamlRoot;
                    await finalDialog.ShowAsync();
                }
            }
        }
    }
}
