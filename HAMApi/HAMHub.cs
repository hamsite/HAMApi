using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HAMApi
{
    public class HAMHub : Hub
    {
        private static List<SwitchClients> SwitchClients = new List<SwitchClients>();

        public override Task OnDisconnectedAsync(Exception ex)
        {
            var User = SwitchClients.SingleOrDefault(c => c.ConnectionID == Context.ConnectionId);
            if (User != null)
            {
                SwitchClients.Remove(User);
                Clients.All.SendAsync("NotifyDisconnection", User);
            }

            return base.OnDisconnectedAsync(ex);
        }

        public override Task OnConnectedAsync()
        {
            if (!SwitchClients.Exists(s => s.ConnectionID == Context.ConnectionId))
            {

            }

            return base.OnConnectedAsync();
        }

        public void ConnectToServer(Machine machine)
        {
            if (!SwitchClients.Exists(s => s.ConnectionID == Context.ConnectionId))
            {
                //Check the smart contract if the address is saved
                var newUser = new SwitchClients {Name = machine.Name, ConnectionID = Context.ConnectionId, MachineID = machine.MachineID };
                SwitchClients.Add(newUser);
                Clients.All.SendAsync("NotifyConnection", newUser);
            }
        }

        public void TurnOn()
        {
            if (SwitchClients.Exists(s => s.ConnectionID == Context.ConnectionId))
            {
                var User = SwitchClients.FirstOrDefault(c => c.ConnectionID == Context.ConnectionId);
                User.Status = true;
                Clients.All.SendAsync("UpdateStatus", User);
            }
        }

        public void TurnOff()
        {
            if (SwitchClients.Exists(s => s.ConnectionID == Context.ConnectionId))
            {
                var User = SwitchClients.FirstOrDefault(c => c.ConnectionID == Context.ConnectionId);
                User.Status = false;
                Clients.All.SendAsync("UpdateStatus", User);
            }
        }

        public List<SwitchClients> GetPeers()
        {
            return SwitchClients;
        }

        public bool GetStatus(string connectingPeer)
        {
            var User = SwitchClients.SingleOrDefault(c => c.ConnectionID == Context.ConnectionId);
            if (User != null)
            {
                if (User.Status)
                {
                    var ConnectingPeer = SwitchClients.SingleOrDefault(c => c.Name == connectingPeer);
                    if (ConnectingPeer != null)
                    {
                        if (ConnectingPeer.Status)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }

}
