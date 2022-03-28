using Microsoft.AspNetCore.SignalR.Client;
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

namespace StitchingSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();
            StartSignalR();
        }
        private async void StartSignalR()
        {
            connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:7028/signalhub")
                            .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int>("ReceiveSignal", (message) =>
            {
                if (message == 0)
                    ConnectionStatus.Content = "Signal OFF";

                else
                    ConnectionStatus.Content = "Signal ON";
            });

            try
            {
                await connection.StartAsync();
                ConnectionStatus.Content = "Connected!";
            }
            catch (Exception ex)
            {

            }
        }
        private void StartStream_Click(object sender, RoutedEventArgs e)
        {
            connection?.SendAsync("SendSignal", 1).Wait();
        }

        private void StopStream_Click(object sender, RoutedEventArgs e)
        {
            connection?.SendAsync("SendSignal", 0).Wait();
        }
    }
}
