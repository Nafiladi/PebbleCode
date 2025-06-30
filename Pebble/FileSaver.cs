using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace PebbleCode
{
    public class FileSaver
    {
        public async Task SaveFileAsync(object window, string content)
        {
            var savePicker = new FileSavePicker();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(savePicker, hwnd);
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Text File", new[] { ".txt" });
            savePicker.SuggestedFileName = "code";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, content);
            }
        }
    }
}
