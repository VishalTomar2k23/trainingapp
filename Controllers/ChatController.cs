﻿using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ChatController : ControllerBase
    {

        private readonly IChatService chatService;
        private readonly IProfileService profileService;

        public ChatController(IChatService chatService, IProfileService profileService)
        {
            this.chatService = chatService;
            this.profileService = profileService;
        }

        [HttpGet("fetchFriends")]
        [Authorize]
        public IActionResult FetchFriends(string searchTerm)
        {
            //IEnumerable<FriendProfileModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            //Console.WriteLine("inside FetchFriends of chat controller");
            //foreach(var profile in en)
            //{

            //    Console.WriteLine(profile.FirstName);
            //}

            Console.WriteLine(searchTerm);
            IEnumerable<FriendProfileModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            foreach(FriendProfileModel profile in en)
            {
                Console.WriteLine(profile.FirstName);
            }
            return Ok(new { message = en });
        }

        [HttpPost("addMessage")]
        [Authorize]
        public IActionResult AddMessage([FromBody] MessageModelToSendMessage message)
        {
            MessageModel messageModel = new()
            {
                Message = message.Message,
                RecieverID = FetchIdFromUserName(message.Reciever),
                SenderID = FetchIdFromUserName(message.Sender),
            };
            var data = this.chatService.AddMessage(messageModel);
            return Ok(new { data = data });
        }

        [HttpGet("fetchMessages")]
        [Authorize]
        public IActionResult FetchMessages(string loggedInUserName, string friendUserName )
        {
            int loggedInUserId = FetchIdFromUserName(loggedInUserName);
            int friendId = FetchIdFromUserName(friendUserName);
            IEnumerable<MessageEntity> messages = this.chatService.FetchMessages(loggedInUserId, friendId).OrderBy(m => m.CreatedAt);
            IEnumerable messagesToBeSent = messages.Select(
                m => new
                {
                    message = m.Message,
                    sender = FetchUserNameFromId(m.SenderID),
                    reciever = FetchUserNameFromId(m.RecieverID),
                    createdAt = m.CreatedAt,
                    isSeen = m.IsSeen
                }
                );
            return Ok(new {messages = messagesToBeSent });
        }

        int FetchIdFromUserName(string userName)
        {
            return this.profileService.FetchIdFromUserName(userName);
        }

        string FetchUserNameFromId(int id)
        {
            return this.profileService.FetchUserNameFromId(id);
        }
    }
    
}
