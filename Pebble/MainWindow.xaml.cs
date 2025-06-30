using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace PebbleCode
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var interpreter = new PebbleCodeInterpreter();
            interpreter.OnRequestInput += HandleInputRequest;
            var output = await interpreter.ExecuteAsync(CodeEditor.Text);
            OutputConsole.Text = output;
        }

        private async Task<string> HandleInputRequest(string prompt)
        {
            var inputDialog = new ContentDialog
            {
                Title = prompt,
                Content = new TextBox { PlaceholderText = "Enter your value" },
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await inputDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return (inputDialog.Content as TextBox).Text;
            }
            return "";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSaver = new FileSaver();
            await fileSaver.SaveFileAsync(this, CodeEditor.Text);
        }
    }
}
