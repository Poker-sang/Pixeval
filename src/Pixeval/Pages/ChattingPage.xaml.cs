using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Mako.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;
using Pixeval.Util.IO.Caching;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Pixeval.Utilities;

namespace Pixeval.Pages;

public sealed partial class ChattingPage
{
    public ChattingPage() => InitializeComponent();

    private readonly Vm _vm = new Vm();

    public override async void OnPageActivated(NavigationEventArgs e, object? parameter)
    {
        await _vm.InitAsync();
    }

    private void DeleteReplyButton_OnClicked(object sender, RoutedEventArgs e)
    {
        _vm.CurrentChat().Remove(sender.To<FrameworkElement>().GetTag<Chat>());
    }

    private async void AutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(sender.Text))
            return;

        _vm.CurrentChat().Add(new Chat(await Vm.GetUserAsync(App.AppViewModel.PixivUid), sender.Text, DateTime.Now));
        sender.Text = "";
    }
}

public partial class Vm : ObservableObject
{
    public async Task InitAsync()
    {
        Chats[0].AddRange(
        [
            new Chat(await GetUserAsync(16081786), "你好", DateTime.Now - TimeSpan.FromHours(1.5)),
            new Chat(await GetUserAsync(33558705), "欢迎来到Pixeval", DateTime.Now - TimeSpan.FromHours(1.3)),
            new Chat(await GetUserAsync(1039353), "1+1=2", DateTime.Now - TimeSpan.FromHours(0.7))
        ]);
        Chats[1].AddRange(
        [
            new Chat(await GetUserAsync(212304), "你好", DateTime.Now - TimeSpan.FromHours(1)),
            new Chat(await GetUserAsync(222905), "这里是风景画区", DateTime.Now - TimeSpan.FromHours(0.3)),
        ]);
        Chats[2].AddRange(
        [
            new Chat(await GetUserAsync(420509), "你好", DateTime.Now - TimeSpan.FromHours(2)),
            new Chat(await GetUserAsync(2353373), "这里是人物肖像画区", DateTime.Now - TimeSpan.FromHours(1.4)),
        ]);
    }

    public Dictionary<int, ObservableCollection<Chat>> Chats { get; } = new()
    {
        [0] = [],
        [1] = [],
        [2] = []
    };

    [ObservableProperty]
    public partial int Index { get; set; } = 0;

    public ObservableCollection<Chat> CurrentChat(int index = -1)
    {
        if (index is -1)
            index = Index;
        return Chats[index];
    }

    public static Task<UserEntity> GetUserAsync(long id) => App.AppViewModel.GetFromJsonAsync<UserEntity>("get/user", id, ("followedUserId", id.ToString()));
}

public partial class Chat : ObservableObject
{
    public Chat(UserEntity user, string comment, DateTime date)
    {
        User = user;
        Comment = comment;
        Date = date;

        Init();
    }

    [MemberNotNull(nameof(AvatarSource))]
    private async void Init()
    {
        AvatarSource = await CacheHelper.GetSourceFromCacheAsync(User.ProfileImageUrls.Medium);
    }

    public bool IsMe => User.Id == App.AppViewModel.MakoClient.Me.Id;

    [ObservableProperty]
    public partial ImageSource AvatarSource { get; private set; }

    public string UserName => User.Name;

    private UserEntity User { get; }

    public string Comment { get; }

    public DateTime Date { get; }
}
