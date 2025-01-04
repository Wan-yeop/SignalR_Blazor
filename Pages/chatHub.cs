using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

namespace MySignalRCoreApp.Hubs
{
    public class ChatHub : Hub
    {
        // 접속자 정보
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

        // 클라이언트에서 호출할 메서드
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            Console.WriteLine($"*** User-Agent: {httpContext?.Request.Headers["User-Agent"]}");
            Console.WriteLine($"*** QueryString: {httpContext?.Request.QueryString}");
            Console.WriteLine($"*** Path: {httpContext?.Request.Path}");

           // await Clients.All.SendAsync("ReceiveLog", httpContext?.Request.Path);

            // 내부 SignalR 연결인지 확인
            if (httpContext != null && httpContext.Request.Headers["User-Agent"].ToString().Contains("Blazor"))
            {
                // 내부 Blazor 연결은 무시
                return;
            }

            // append Dictionary
            string connectionId = Context.ConnectionId;
            string UserName = $"ID_{connectionId.Substring(0, 5)}";
            string connectionTime = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");

            ConnectedUsers.TryAdd(connectionId, $"{UserName} ({connectionTime})");

            // SemdAsync
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                Console.WriteLine($"Disconnection error: {exception.Message}");
            }
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                Console.WriteLine($"Received: {user} - {message}");
                await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId.Substring(0, 5), message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }
    }
}
