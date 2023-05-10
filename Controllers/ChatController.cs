﻿using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ChatApp.Business.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {

        private readonly IChatService chatService;
        private readonly IProfileService profileService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IOnlineUserService onlineUserService;
        private readonly INotificationService notificationService;

        public ChatController(IChatService chatService, 
            IProfileService profileService, 
            IWebHostEnvironment webHostEnvironment, 
            IHubContext<ChatHub> hubContext,
            IOnlineUserService onlineUserService,
            INotificationService notificationService)
        {
            this.chatService = chatService;
            this.profileService = profileService;
            this.webHostEnvironment = webHostEnvironment;
            this.hubContext = hubContext;
            this.onlineUserService = onlineUserService;
            this.notificationService = notificationService;
        }


        #region endpoints
        //this is for searching the users starting their firstname with searchTerm
        [HttpGet("fetchFriends")]
        public IActionResult FetchFriends(string searchTerm)
        {

            
            IEnumerable<UserModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            
            return Ok(new { message = en });
        }

        [HttpGet("fetchMessages")]
        public IActionResult FetchMessages(string loggedInUserName, string friendUserName)
        {
            int loggedInUserId = FetchIdFromUserName(loggedInUserName);
            int friendId = FetchIdFromUserName(friendUserName);
            IEnumerable<MessageEntity> messages = this.chatService.FetchMessages(loggedInUserId, friendId).OrderBy(m => m.CreatedAt);
            
            // for marking each message as seen
            foreach(var message in messages)
            {
                if(message.SenderID == friendId && message.RecieverID == loggedInUserId)
                {
                    message.IsSeen = true;
                }
            }

            // method to update all the messages 
            this.chatService.MarkMsgsAsSeen(messages);

            IEnumerable messagesToBeSent = messages.Select(
                m => new MessageModel()
                {
                    Id = m.Id,
                    Message = m.Message,
                    SenderUserName = FetchUserNameFromId(m.SenderID),
                    RecieverUserName = FetchUserNameFromId(m.RecieverID),
                    CreatedAt = m.CreatedAt,
                    IsSeen = m.IsSeen,
                    RepliedToMsg = FetchMessageFromId(m.RepliedToMsg),
                    MessageType = m.MessageType
                }
                );

            var notificationModels = this.notificationService.GetMessagesNotifications(loggedInUserId, friendId);
            this.notificationService.DeleteNotifications(notificationModels);
            return Ok(new { messages = messagesToBeSent });
        }

        [HttpGet("fetchAll")]
        public IActionResult FetchAllUsers(string loggedInUsername)
        {
            if (string.IsNullOrEmpty(loggedInUsername))
            {
                return BadRequest("ënter username");
            }
            int id = FetchIdFromUserName(loggedInUsername);

            //list of friends that have interacted with logged in user before
            IEnumerable<UserModel> friends = this.profileService.FetchAllUsers(id);
            return Ok(friends);
        }

        [HttpPost("addFile")]
        public IActionResult AddFile([FromForm] FileMessageModel fileModel)
        {

            IFormFile file = fileModel.File;
            MessageModel messageModel = new()
            {
                SenderUserName = fileModel.SenderUserName,
                RecieverUserName = fileModel.RecieverUserName,
                RepliedToMsg = "-1",
                CreatedAt = DateTime.UtcNow
            };

            Notification notification = new()
            {
                CreatedAt = DateTime.UtcNow
            };

            string path = webHostEnvironment.WebRootPath + @"/SharedFiles/";

            if (file.ContentType.StartsWith("image"))
            {
                messageModel.MessageType = MessageType.Image;
                messageModel.Message = FileManagement.CreateUniqueFile(path + "Images", file);
                notification.Type = 2;
            }
            else if (file.ContentType.StartsWith("video"))
            {
                messageModel.MessageType = MessageType.Video;
                messageModel.Message = FileManagement.CreateUniqueFile(path + "Videos", file);
                notification.Type = 3;
            }
            else if (file.ContentType.StartsWith("audio"))
            {
                messageModel.MessageType = MessageType.Audio;
                messageModel.Message = FileManagement.CreateUniqueFile(path + "Audios", file);
                notification.Type = 4;
            }
            else
            {
                return BadRequest(new { message = "only images, videos and audios can be shared" });
            }

            MessageEntity messageEntity = this.chatService.AddMessage(messageModel);

            messageModel = ConvertToMessageModel(messageEntity);

            notification.RaisedBy = messageEntity.SenderID;
            notification.RaisedFor = messageEntity.RecieverID;

            notification = this.notificationService.AddNotification(notification);

            string senderConnectionId = this.onlineUserService.FetchOnlineUser(messageModel.SenderUserName).ConnectionId;
            OnlineUserEntity reciever = this.onlineUserService.FetchOnlineUser(messageModel.RecieverUserName);
            if (reciever != null)
            {
                this.hubContext.Clients.Clients(senderConnectionId, reciever.ConnectionId).SendAsync("AddMessageToTheList", messageModel);
                this.hubContext.Clients.Client(reciever.ConnectionId).SendAsync("AddNotification", EntityToModel.ConvertToNotificationModel(notification));
            }
            else
            {
                this.hubContext.Clients.Client(senderConnectionId).SendAsync("AddMessageToTheList", messageModel);
            }
            
            return Ok(new {message = "file added"});
        }

        [HttpGet("markAsRead")]
        public IActionResult MarkMsgs(string loggedInUserName, string friendUserName)
        {
            int loggedInUserId = FetchIdFromUserName(loggedInUserName);
            int friendId = FetchIdFromUserName(friendUserName);
            IEnumerable<MessageEntity> messages = this.chatService.FetchMessages(loggedInUserId, friendId).OrderBy(m => m.CreatedAt);

            // for marking each message as seen
            foreach (var message in messages)
            {
                if (message.SenderID == friendId && message.RecieverID == loggedInUserId)
                {
                    message.IsSeen = true;
                }
            }

            /*
                method to update all the messages
           
             */

            this.chatService.MarkMsgsAsSeen(messages);

            return Ok(new { messages= "message read"});
        }

       

        #endregion

        #region helpermethods
        int FetchIdFromUserName(string userName)
        {
            return this.profileService.FetchIdFromUserName(userName);
        }

        string FetchUserNameFromId(int id)
        {
            return this.profileService.FetchUserNameFromId(id);
        }

        string? FetchMessageFromId(int id)
        {
            return this.chatService.FetchMessageFromId(id);
        }        

        MessageModel ConvertToMessageModel(MessageEntity messageEntity)
        {
            MessageModel messageModel = new MessageModel()
            {
                Id = messageEntity.Id,
                MessageType = messageEntity.MessageType,
                Message = messageEntity.Message,
                CreatedAt = messageEntity.CreatedAt,
                IsSeen = messageEntity.IsSeen,
                RecieverUserName = FetchUserNameFromId(messageEntity.RecieverID),
                SenderUserName = FetchUserNameFromId(messageEntity.SenderID),
                RepliedToMsg = this.chatService.FetchMessageFromId(messageEntity.RepliedToMsg)
            };

            return messageModel;
        }
        #endregion

    }

}
