using Microsoft.AspNetCore.SignalR.Client;

namespace VivChat.Components.Pages
{
    public partial class Home
    {
        private HubConnection? hubConnection;

        private readonly List<string> messages = [];

        private string? userInput;

        private string? messageInput;

        public Home()
        {
            userInput = string.Empty;
            messageInput = string.Empty;
        }

        /// <summary>
        /// (1) 허브 연결 초기화
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/chathub")).Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {DateTime.Now:t}\n{message}";
                messages.Add(encodedMsg);
                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        /// <summary>
        /// Message Sending...
        /// </summary>
        /// <returns></returns>
        private async Task Send()
        {
            if (hubConnection is not null)
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
                await hubConnection.DisposeAsync();
        }
    }
}
