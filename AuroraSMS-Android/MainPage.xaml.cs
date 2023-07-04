using AuroraSMS_Android.Platforms.Android;

namespace AuroraSMS_Android
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            LoadUserPreferences();
        }

        private void LoadUserPreferences()
        {
            EndpointUrl.Text = Preferences.Default.Get<string>("endpoint", "");
            ApiKey.Text = Preferences.Default.Get<string>("apikey", "");
        }

        private void OnSaveBtnClicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("endpoint", EndpointUrl.Text);
            Preferences.Default.Set("apikey", ApiKey.Text);
        }

        private void StartBtn_Clicked(object sender, EventArgs e)
        {
            Android.Content.Intent intent = new(Android.App.Application.Context, typeof(MessageService));
#if ANDROID
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }
    }
}