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
using Mobile_Money_Records_Reconciliator.Pages;
using Windows.UI.Core;

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
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(MainWindow)),
            ("statements", typeof(MpesaStatements)),
        };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            //NavView_Navigate("home", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

            // Listen to the window directly so the app responds
            // to accelerator keys regardless of which element has focus.
            //Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
            //    CoreDispatcher_AcceleratorKeyActivated;

            //Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            //SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;

        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private void NavView_SelectionChanged(muxc.NavigationView sender,
                                      muxc.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(
    string navItemTag,
    Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                //_page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            // When Alt+Left are pressed navigate back
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == Windows.System.VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            // Handle mouse back button.
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            //if (ContentFrame.SourcePageType == typeof(SettingsPage))
            //{
            //    // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
            //    NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            //    NavView.Header = "Settings";
            //}
            //else if (ContentFrame.SourcePageType != null)
            //{
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                NavView.Header =
                    ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            //}
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
            statement.PassKey = await GetPasskey();
            var extractor = new Core.Services.Files.PDFExtractor(statement);
            var result = await extractor.ExtractRecordsAsync();

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
