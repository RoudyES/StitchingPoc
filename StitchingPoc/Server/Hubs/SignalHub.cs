using Microsoft.AspNetCore.SignalR;

namespace StitchingPoc.Server.Hubs
{
    public class SignalHub: Hub
    {
        public async Task SendSignal(int signal)
        {
            await Clients.All.SendAsync("ReceiveSignal", signal);
        }

        public async Task SendImage(byte[] img)
        {
            await Clients.All.SendAsync("ReceiveImage", img);
        }
    }
}
