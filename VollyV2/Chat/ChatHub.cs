using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Services;

namespace VollyV2.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private const string Vollybot = "VollyBot";
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ChatHub(ApplicationDbContext context, 
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task SendMessage(string message)
        {
            string name = Context.User.Identity.Name;
            await Clients.Caller.SendAsync("ReceiveMessage", name, message);
            await ProcessMessage(message);
        }

        private async Task ProcessMessage(string message)
        {
            string lowerMessage = message.ToLower();
            List<Opportunity> opportunitiesList = await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context);
            List<Opportunity> opportunities = opportunitiesList
                .Where(o => lowerMessage.Contains(o.Category.Name.ToLower()) ||
                            lowerMessage.Contains(o.Organization.Cause.Name.ToLower()))
                .ToList();
            if (opportunities.Count != 0)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", Vollybot, "Here are the results:");
                await Clients.Caller.SendAsync("ListOpportunityLinks", opportunities);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", Vollybot, "No results found, try again please.");
            }
        }
    }
}
