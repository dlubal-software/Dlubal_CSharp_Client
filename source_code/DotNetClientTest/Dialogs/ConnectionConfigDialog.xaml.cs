using Dlubal.WS.Common.Tools;
using System;
using System.ServiceModel;
using System.Windows;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
#elif RSECTION
using Dlubal.WS.RSection1.Application;
using ApplicationClient = Dlubal.WS.RSection1.Application.RSectionApplicationClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectionConfigDialog.xaml
    /// </summary>
    public partial class ConnectionConfigDialog : Window
    {
        private bool buttonClicked;

        public string Address { get; private set; } = Session.DEFAULT_ADDRESS;

        public ConnectionConfigDialog()
        {
            InitializeComponent();
            UpdateAdresses();
        }

        private void UpdateAdresses()
        {
            addressComboBox.Items.Clear();

            string[] endpoints = Session.GetEndpoints();

            if (endpoints == null || endpoints.Length == 0)
            {
                addressComboBox.Items.Add(Session.DEFAULT_ADDRESS);
            }
            else
            {
                foreach (string endpoint in endpoints)
                {
                    addressComboBox.Items.Add(endpoint);
                }
            }

            addressComboBox.SelectedIndex = 0;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonClicked)
            {
                return;
            }

            Session.DataLogger.AddLogStart("Initialization of Application WS Client Proxy...");

            buttonClicked = true;

            if (!Uri.TryCreate(addressComboBox.Text, UriKind.Absolute, out Uri uri) || ((uri.Scheme != Uri.UriSchemeHttp) && (uri.Scheme != Uri.UriSchemeHttps)))
            {
                Session.DataLogger.ReportError($"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!");
                Session.DataLogger.AddSeparator();
                buttonClicked = false;
                return;
            }

            Address = addressComboBox.Text;
            Session.Address = new EndpointAddress(addressComboBox.Text);

            try
            {
                Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);
                application_information information = TestingMethods.SoapApplicationClient.get_information();
                Session.DataLogger.AddText($"{information.name}, {information.language_name}");
                Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            }
            catch (Exception exception)
            {
                if (Session.SoapApplicationClient != null)
                {
                    if (Session.SoapApplicationClient.State != CommunicationState.Faulted)
                    {
                        Session.SoapApplicationClient.Close();
                    }
                    else
                    {
                        Session.SoapApplicationClient.Abort();
                    }

                    Session.SoapApplicationClient = null;
                }

                Session.DataLogger.ReportError(exception);
                Session.DataLogger.AddSeparator();
                buttonClicked = false;
                return;
            }

            Session.DataLogger.AddSeparator();
            DialogResult = true;
            buttonClicked = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonClicked)
            {
                return;
            }

            buttonClicked = true;
            DialogResult = false;
            Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAdresses();
        }
    }
}
