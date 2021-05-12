using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using muxc = Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using Windows.UI.Popups;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mobile_Money_Records_Reconciliator
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //Required models
        private Core.Models.PDFStatement statement;
        //End
        public MainWindow()
        {
            this.InitializeComponent();
            statement = new Core.Models.PDFStatement();
        }

        private double NavViewCompactModeThresholdWidth { get { return NavView.CompactModeThresholdWidth; } }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            var extractor = new Core.Services.Files.PDFExtractor(statement);
            var result = extractor.ExtractRecordsAsync();

            //TODO set MessageBox to match app scheme
            ContentDialog fileLoadedDialog = new ContentDialog
            {
                Title = "File Loaded",
                Content = $"File '{statement.FileName}' has been successfully loaded!",
                CloseButtonText = "Ok"
            };

            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                fileLoadedDialog.XamlRoot = MainAppGrid.XamlRoot;
            }
            await fileLoadedDialog.ShowAsync();

            var passkey = await GetPasskey();

            if (!Docs.IsEnabled)
            {
                Docs.IsEnabled = true;
                Docs.Items.Clear();
            }
            Docs.Items.Add(statement.FileName);
        }

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        private async Task<int> GetPasskey()
        {


            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                GetPin.XamlRoot = MainAppGrid.XamlRoot;
            }
            var userResponse = await GetPin.ShowAsync();
            if (userResponse == ContentDialogResult.Primary)
            {
                return int.Parse(Passkey.Password.ToString());
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
                    fileLoadedDialog.XamlRoot = MainAppGrid.XamlRoot;
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
                        finalDialog.XamlRoot = MainAppGrid.XamlRoot;
                    await finalDialog.ShowAsync();
                }
            }
        }
    }
}
