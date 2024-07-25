using Microsoft.AspNetCore.SignalR;
using tupencauywebapi.SignalR.Hubs;

namespace tupencauywebapi.SignalR.Servicios
{
    public class ServicioChat
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ServicioChat(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        public async Task OnUserConnected(string connectionId, string sitioId, string pencaId)
        {
            await _chatHubContext.Clients.Client(connectionId).SendAsync("JoinPencaGroup", sitioId, pencaId);
        }
    }
}
