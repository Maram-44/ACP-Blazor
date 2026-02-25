using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using ACP.Models.Account;

namespace ACP.Services
{
    public class NotificationClientService : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;

        public List<Notification> Notifications { get; private set; } = new();
        public int UnreadCount => Notifications.Count(n => !n.IsRead);
        public event Action? OnNotificationsChanged;

        public NotificationClientService(HttpClient httpClient, HubConnection hubConnection)
        {
            _httpClient = httpClient;
            _hubConnection = hubConnection;

            // إعداد استقبال الرسائل من SignalR
            _hubConnection.On<Notification>("ReceiveNotification", (notification) =>
            {
                Notifications.Insert(0, notification); // إضافة الجديد في البداية
                OnNotificationsChanged?.Invoke(); // تحديث الواجهة
            });
        }

        public async Task InitializeAsync()
        {
            // 1. جلب الإشعارات القديمة من الـ Controller
            var savedNotifications = await _httpClient.GetFromJsonAsync<List<Notification>>("api/notifications");
            if (savedNotifications != null)
            {
                Notifications = savedNotifications;
            }

            // 2. تشغيل اتصال SignalR إذا لم يكن متصلاً
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }

            OnNotificationsChanged?.Invoke();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/notifications/{id}/mark-as-read", null);
            if (response.IsSuccessStatusCode)
            {
                var notif = Notifications.FirstOrDefault(n => n.Id == id);
                if (notif != null)
                {
                    notif.IsRead = true;
                    OnNotificationsChanged?.Invoke();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
