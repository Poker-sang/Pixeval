using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Pixeval.Controls;
using SixLabors.ImageSharp;
using WinUI3Utilities;

namespace Pixeval.Pages;

public sealed partial class PostPage : EnhancedPage
{
    public PostPage() => InitializeComponent();

    private async void LoadNewFileOnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (await Window.PickSingleFileAsync() is { } file)
        {
            PathTextBlock.Text = file.Path;
            FileThumbnailImage.Source = new BitmapImage(new Uri(file.Path));
            UploadButton.Visibility = Visibility.Collapsed;
            PostButton.IsEnabled = true;
        }
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var identify = await Image.IdentifyAsync(PathTextBlock.Text);
            await App.AppViewModel.HttpClient.PostAsync(
                $"post" +
                $"?userId={App.AppViewModel.PixivUid}" +
                $"&title={TitleTextBox.Text}" +
                $"&desc={DescriptionTextBox.Text}" +
                $"&path={PathTextBlock.Text}" +
                $"&width={identify.Width}" +
                $"&height={identify.Height}", null);
            this.SuccessGrowl("投稿成功！");
            PostButton.IsEnabled = false;
        }
        catch (Exception exception)
        {
            this.ErrorGrowl("投稿失败！", exception.Message);
        }
    }
}
