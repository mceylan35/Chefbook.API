using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.SignalR.Abstract;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;


namespace Chefbook.API.SignalR.Concrete
{
    [Authorize]
    //[HubName("emergyHub")]
    public class NotificationHub : Hub
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        private IFollowService _followService;
        private IPostService _postService;
        private IImageService _imageService;
        private IStarService _starService;

        public NotificationHub(IUserService userService, IConfiguration configuration, IFollowService followService,
            IPostService postService, IImageService imageService, IStarService starService)
        {
            _userService = userService;
            _configuration = configuration;
            _followService = followService;
            _postService = postService;
            _imageService = imageService;
            _starService = starService;
        }

        public override async Task OnConnectedAsync()
        {
            //  var currentUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            using (var context = new ChefContext())
            {
                var connection =
                    context.Connection.FirstOrDefault(i =>
                        i.UserId == Guid.Parse("4bcbbcbf-d75f-4c0f-821c-2a833f800ff4"));
                if (connection != null)
                {
                    connection.ConnectionId = Context.ConnectionId;
                    connection.Connected = true;
                }
                else
                {
                    context.Connection.Add(new Connection
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.Parse("4bcbbcbf-d75f-4c0f-821c-2a833f800ff4"),
                        ConnectionId = Context.ConnectionId
                    });
                }

                context.SaveChanges();


                await Clients.Caller.SendAsync("GetConnectionId", this.Context.ConnectionId);


                await base.OnConnectedAsync();
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            using (var context = new ChefContext())
            {
                var connection = context.Connection.FirstOrDefault(i => i.ConnectionId == Context.ConnectionId);
           
                    connection.Connected = false;
                    context.SaveChanges();
             
                
            }

            return base.OnDisconnectedAsync(exception);
        }

        //  [AllowAnonymous]
        public Task SendNotification(Guid whoId, Guid sendId, string message)
        {
            using (var context = new ChefContext())
            {
                var who = context.Connection.FirstOrDefault(i => i.UserId == whoId && i.Connected == true);
                if (who != null)
                {
                    return Clients.User(who.ConnectionId).SendAsync("NotificationGuncelle");
                }
                else
                {
                    return Clients.All.SendAsync("NotificationGuncelle");
                }
            }
        }


        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}