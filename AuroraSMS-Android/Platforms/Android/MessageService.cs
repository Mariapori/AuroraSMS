using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using AndroidX.Core.App;
using System.ComponentModel;
using System.Net.Http.Json;

namespace AuroraSMS_Android.Platforms.Android
{
    [Service]
    public class MessageService : Service
    {
        private string NOTIFICATION_CHANNEL_ID = "1000";
        private int NOTIFICATION_ID = 1;
        private string NOTIFICATION_CHANNEL_NAME = "notification";
        private const string GetUnsentMessages = "/api/GetUnsentMessages";
        private const string ChangeMessageStatus = "/api/ChangeMessageStatus";

        public static EventHandler<DateTime> LatestFetchUpdate;
        

        private void startForegroundService()
        {
            var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                createNotificationChannel(notifcationManager);
            }

            var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
            notification.SetAutoCancel(false);
            notification.SetOngoing(true);
            notification.SetSmallIcon(Resource.Mipmap.appicon);
            notification.SetContentTitle("AuroraSMS Service");
            notification.SetContentText("AuroraSMS Service is running");
            StartForeground(NOTIFICATION_ID, notification.Build());

            Task.Run(async() =>
            {
                while (true)
                {
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        var endpoint = Preferences.Default.Get<string>("endpoint", "");
                        var apikey = Preferences.Default.Get<string>("apikey", "");
                        Uri uri = new Uri(endpoint);
                        httpClient.BaseAddress = uri;
                        httpClient.DefaultRequestHeaders.Add("X-API-Key", apikey);

                        var response = await httpClient.GetAsync(uri + GetUnsentMessages);
                        LatestFetchUpdate(null,DateTime.Now);
                        if (response.IsSuccessStatusCode)
                        {
                            var messages = await response.Content.ReadFromJsonAsync<List<AuroraSmsMessage>>();

                            foreach (var message in messages)
                            {
                                try
                                {
                                    if (Sms.Default.IsComposeSupported)
                                    {
                                        SmsManager smsM = SmsManager.Default;
                                        smsM.SendTextMessage(message.To, null, message.Message, null, null);
                                        await httpClient.PostAsync(uri + ChangeMessageStatus + $"?id={message.Id}&newStatus=1", null);
                                    }
                                }
                                catch (FeatureNotSupportedException ex)
                                {

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                    // Minute
                    Thread.Sleep(1000 * 60);
                }
            });
        }

        private void createNotificationChannel(NotificationManager notificationMnaManager)
        {
            var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME,
            NotificationImportance.Low);
            notificationMnaManager.CreateNotificationChannel(channel);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            startForegroundService();
            return StartCommandResult.NotSticky;
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }

    }
}
