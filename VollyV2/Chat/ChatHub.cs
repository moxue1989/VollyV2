using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using VollyV2.Data;
using VollyV2.Models;

namespace VollyV2.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            string name = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", name, message);
        }
    }
}
