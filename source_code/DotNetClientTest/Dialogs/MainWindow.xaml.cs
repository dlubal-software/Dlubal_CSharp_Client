using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Dialogs;
using Dlubal.WS.Common.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Dlubal.WS.Clients.DotNetClientTest.Dialogs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataLogger dataLogger = DataLogger.DummyDataLogger;

        public MainWindow(DataLogger dataLogger)
        {
            InitializeComponent();

            Title = $"DotNetclientTest for {Program.ApplicationName}";

            this.dataLogger = dataLogger;
            dataLogger.LogBox = logTextBox;
            dataLogger.ProgressBar = actionProgressBar;
            dataLogger.ResetProgressBar();

            dataLogger.AddLogStart("Initialization of web service tests...");
            SetActionStatusBarText("Initialization of tests...");

            MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
            dataLogger.AddText($"{methodInfos.Length} methods found.");
            dataLogger.InitializeProgressBar(0, methodInfos.Length, 0);

            int countOfMethods = SetTreeView(methodInfos, $"{Program.ApplicationName} Web Service Tests");

            dataLogger.AddText($"{countOfMethods} methods parsed.");
            dataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            dataLogger.AddSeparator();
            dataLogger.ResetProgressBar();
            SetActionStatusBarText(string.Empty);
        }

        private int SetTreeView(MethodInfo[] methodInfos, string rootName)
        {
            TreeItemMethodInfo root = new TreeItemMethodInfo(rootName)
            {
                IsExpanded = true
            };

            int countOfMethods = 0;

            for (int methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
            {
                MethodInfo methodInfo = methodInfos[methodIndex];
                dataLogger.SetProgressBarValue(methodIndex + 1);

                string[] nameParts = methodInfo.Name.Replace('\u2040', ' ').Split('_');

                if (nameParts.Length < 2)
                {
                    continue;
                }

                if (nameParts[0].ToLower() != "test")
                {
                    continue;
                }

                TreeItemMethodInfo currentItem = root;

                for (int partIndex = 1; partIndex < nameParts.Length; partIndex++)
                {
                    if (currentItem.Items != null)
                    {
                        var existingItems = currentItem.Items.Where(x => x.Name == nameParts[partIndex]).ToList();

                        if (existingItems.Count > 0)
                        {
                            currentItem = existingItems[0] as TreeItemMethodInfo;
                            continue;
                        }
                    }
                    else
                    {
                        currentItem.Items = new ObservableCollection<CustomTreeItem<MethodInfo>>();
                    }

                    TreeItemMethodInfo newItem = new TreeItemMethodInfo(nameParts[partIndex])
                    {
                        Parent = currentItem
                    };

                    currentItem.Items.Add(newItem);
                    currentItem = newItem;

                    if (partIndex == 1)
                    {
                        currentItem.IsExpanded = true;
                    }
                    else if (partIndex == (nameParts.Length - 1))
                    {
                        currentItem.Content = methodInfo;
                        currentItem.IsThreeState = false;
                        countOfMethods++;
                    }
                }
            }

            treeViewControl.Items.Add(root);
            return countOfMethods;
        }

        private void SetActionStatusBarText(string text)
        {
            if (!actionTextBlock.Dispatcher.CheckAccess())
            {
                actionTextBlock.Dispatcher.Invoke(new Action<string>(SetActionStatusBarText), new object[] { text });
            }
            else
            {
                actionTextBlock.Text = text;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.SoapApplicationClient == null)
            {
                ConnectionConfigDialog connectionConfigDialog = new ConnectionConfigDialog
                {
                    Owner = this
                };

                SetActionStatusBarText("Connection to SOAP server...");

                bool? result = connectionConfigDialog.ShowDialog();

                if ((result == null) || (result == false))
                {
                    Session.DataLogger.AddWarning("Connection to SOAP server was canceled!");
                    SetActionStatusBarText(string.Empty);
                    return;
                }

                menuItemConnect.Header = "Disconnect";
                menuItemConnect.Icon = new Image
                {
                    Source = new BitmapImage(new Uri("/Icons/Connected.png", UriKind.Relative))
                };

                treeViewControl.IsEnabled = true;
                menuItemRunSelectedTest.IsEnabled = true;
                menuItemRunAllCheckedTests.IsEnabled = true;
                menuItemExportTestList.IsEnabled = true;
                SetActionStatusBarText(string.Empty);

                Title += " - " + connectionConfigDialog.Address;
            }
            else
            {
                Session.CloseModelClient();
                Session.CloseApplicationClient();
                DisableTests();
                int index = Title.IndexOf(" - ");
                if (index > 0)
                {
                    Title = Title.Substring(0, index);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Closing WS Client Proxy.
            Session.CloseSoapClients();

            // Closing Data Logger.
            Session.DataLogger.Dispose();
        }

        private void DisableTests()
        {
            treeViewControl.IsEnabled = false;
            menuItemRunSelectedTest.IsEnabled = false;
            menuItemRunAllCheckedTests.IsEnabled = false;
            SetActionStatusBarText(string.Empty);

            menuItemConnect.Header = "Connect";
            menuItemConnect.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/Icons/Disconnected.png", UriKind.Relative))
            };
        }

        private void MenuItemConnect_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        private void MenuItemRunSelectedTest_Click(object sender, RoutedEventArgs e)
        {
            dataLogger.AddLogStart("Passing through selected test...");

            if (Session.SoapApplicationClient == null)
            {
                dataLogger.ReportError($"Web Service Client Proxy is disconnected from server!{Environment.NewLine}Please try to reconnect it.");
                dataLogger.AddSeparator();
                DisableTests();
                return;
            }

            TreeItemMethodInfo item = treeViewControl.SelectedItem as TreeItemMethodInfo;

            if ((item != null) && (item.Items == null) && (item.Content != null))
            {
                SetActionStatusBarText("Test: " + item.Content.ConvertToString());
                Mouse.OverrideCursor = Cursors.Wait;

                bool result = (bool)item.Content.Invoke(null, null);
                dataLogger.AddLogEnd(result ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);

                if (!result && Session.SoapApplicationClient == null)
                {
                    DisableTests();
                }

                Mouse.OverrideCursor = Cursors.Arrow;
                SetActionStatusBarText(string.Empty);
            }
            else
            {
                dataLogger.AddWarning("Test is not selected!");
                dataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            }

            dataLogger.AddSeparator();
        }

        private void MenuItemRunAllCheckedTests_Click(object sender, RoutedEventArgs e)
        {
            TreeItemMethodInfo rootItem = treeViewControl.Items[0] as TreeItemMethodInfo;

            if (rootItem == null)
            {
                return;
            }

            // Gets the list of checked tests.
            dataLogger.AddLogStart("Reading checked tests...");

            List<MethodInfo> testList = new List<MethodInfo>();
            rootItem.GetContentOfCheckedItems(testList);

            Session.RunTests(testList, out string textResult, SetActionStatusBarText);
        }

        private void MenuItemExportTestList_Click(object sender, RoutedEventArgs e)
        {
            dataLogger.AddLogStart("Exporting selected tests...");

            // získat metody které jsou zaškrtnuté......
            TreeItemMethodInfo rootItem = treeViewControl.Items[0] as TreeItemMethodInfo;
            Debug.Assert(rootItem != null);

            List<MethodInfo> testList = new List<MethodInfo>();
            rootItem.GetContentOfCheckedItems(testList);

            bool succeeded = false;

            if (testList.Count == 0)
            {
                dataLogger.AddWarning("No test checked.");
            }
            else
            {
                try
                {
                    SaveFileDialog dialog = new SaveFileDialog
                    {
                        FileName = Properties.Settings.Default.TestMethodsFile
                    };

                    if (dialog.ShowDialog(this) == true)
                    {
                        using (StreamWriter writter = new StreamWriter(dialog.FileName))
                        {
                            foreach (MethodInfo methodInfo in testList)
                            {
                                writter.WriteLine(methodInfo.Name);
                            }

                            writter.Close();
                        }

                        succeeded = true;
                    }
                }
                catch (Exception ex)
                {
                    dataLogger.AddException(ex.ToString());
                }
            }

            dataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            dataLogger.AddSeparator();
        }
    }
}