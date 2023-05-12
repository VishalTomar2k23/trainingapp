﻿using ChatApp.Business.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationServices _notificationService;
		public NotificationController(INotificationServices notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpGet("{userName}")]
		public IActionResult GetAllNotifications(string userName)
		{
			if (userName != null)
			{
				return Ok(_notificationService.GetAllNotifications(userName));
			}
			return BadRequest();
		}

		[HttpPost("Clear")]
		public IActionResult ClearNotification([FromBody]string[] userName)
		{
			_notificationService.Clear(userName[0]);
			return Ok();
		}
	}
}
