using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;
using static IdentityModel.OidcConstants;

namespace Rewind.One.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _accessToken = string.Empty;
        private static string _baseAuthAddress = "https://localhost:44355";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetTokenHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetDiscoveryDocumentAsync(address: _baseAuthAddress);
                    if (response.IsError)
                    {
                        MessageBox.Show("Failed when getting token endpoint");
                    }
                    else
                    {
                        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                        {
                            Address = response.TokenEndpoint,
                            ClientId = "rewind_crypto_api",
                            ClientSecret = "apisecret",
                            GrantType = GrantTypes.ClientCredentials,
                            Scope = "crypto_api"
                        });

                        if (tokenResponse.IsError)
                        {
                            MessageBox.Show("Failed when requesting access_token for the first time");
                        }
                        else
                        {
                            _accessToken = tokenResponse.AccessToken;
                            MessageBox.Show("Success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void GetDataHandler(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                MessageBox.Show("Get token first!");
            }
            else
            {
                string address = "https://localhost:44356/api/hello";
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(_accessToken);
                    var response = await client.GetAsync(address);
                    if (response.IsSuccessStatusCode)
                    {
                        textBlockResult.Text = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        textBlockResult.Text = response.StatusCode.ToString();
                    }
                }
            }
        }

    }
}
