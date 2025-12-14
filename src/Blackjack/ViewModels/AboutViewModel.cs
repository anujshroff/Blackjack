using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    private const string GitHubUrl = "https://blackjack.anujshroff.com";

    [ObservableProperty]
    private string _versionDisplay = string.Empty;

    [ObservableProperty]
    private bool _isLicenseExpanded;

    [ObservableProperty]
    private bool _isPrivacyExpanded;

    [ObservableProperty]
    private bool _isCreditsExpanded;

    public string LicenseText { get; } = """
        MIT License

        Copyright (c) 2025 Anuj Shroff

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
        """;

    public string PrivacyPolicyText { get; } = """
        This application does NOT:
        • Collect personal information
        • Track your gameplay or behavior
        • Access your contacts, location, or files
        • Use advertising SDKs or analytics
        • Transmit any data to servers
        • Require an internet connection

        All game data is stored locally on your device.

        Distribution platforms (Google Play, Microsoft Store) may independently collect their own metrics.

        Full policy available at:
        privacy.blackjack.anujshroff.com
        """;

    public AboutViewModel()
    {
        var version = typeof(AboutViewModel).Assembly.GetName().Version;
        VersionDisplay = version is not null
            ? $"v{version.Major}.{version.Minor}.{version.Build}"
            : "v0.0.0";
    }

    [RelayCommand]
    private static async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void ToggleLicense()
    {
        IsLicenseExpanded = !IsLicenseExpanded;
        if (IsLicenseExpanded)
        {
            IsPrivacyExpanded = false;
            IsCreditsExpanded = false;
        }
    }

    [RelayCommand]
    private void TogglePrivacy()
    {
        IsPrivacyExpanded = !IsPrivacyExpanded;
        if (IsPrivacyExpanded)
        {
            IsLicenseExpanded = false;
            IsCreditsExpanded = false;
        }
    }

    [RelayCommand]
    private void ToggleCredits()
    {
        IsCreditsExpanded = !IsCreditsExpanded;
        if (IsCreditsExpanded)
        {
            IsLicenseExpanded = false;
            IsPrivacyExpanded = false;
        }
    }

    [RelayCommand]
    private static async Task OpenSourceCodeAsync()
    {
        try
        {
            await Browser.Default.OpenAsync(GitHubUrl, BrowserLaunchMode.External);
        }
        catch
        {
            // Unable to open browser - silently ignore
        }
    }
}
