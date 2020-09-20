using AutoMapper;
using DatingApp.API.Helpers;
using DatingApp.DomainModels;
using DatingApp.Repository;
using DatingApp.Repository.Helpers;
using DatingApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public MessageController(IDatingRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet("{MessageId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int MessageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await repo.GetMessage(MessageId);
            if (messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageParams.UserId = userId;
            var messagesFromRepo = await repo.GetMessagesForUser(messageParams);
            var messages = mapper.Map<IEnumerable<MessageToReturnViewModel>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromrepo = await repo.GetMessageThread(userId, recipientId);
            var messageThread = mapper.Map<IEnumerable<MessageToReturnViewModel>>(messageFromrepo);
            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody] MessageForCreationViewModel messageForCreationViewModel)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageForCreationViewModel.SenderId = userId;
            var user = await repo.GetUserByID(userId);
            var reciptientId = await repo.GetUserByID(messageForCreationViewModel.RecipientId);
            if (reciptientId == null)
                return BadRequest("Could not find user");
            var message = mapper.Map<Message>(messageForCreationViewModel);
            repo.Add(message);
            if (await repo.SaveAll())
            {
                var messageToReturn = mapper.Map<MessageToReturnViewModel>(message);

                return CreatedAtRoute("GetMessage", new { userId = userId, MessageId = message.Id }, messageToReturn);
            }
            throw new Exception("Creating the message failed on save");
        }

        [HttpPost("{MessageId}")]
        public async Task<IActionResult> DeleteMessage(int MessageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await repo.GetMessage(MessageId);
            if (messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;
            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                repo.Delete(messageFromRepo);
            if (await repo.SaveAll())
                return NoContent();
            throw new Exception("Error deleting the message");
        }

        [HttpPost("{MessageId}/read")]
        public async Task<IActionResult> MarkasRead(int MessageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await repo.GetMessage(MessageId);

            if (messageFromRepo.RecipientId != userId)
                return Unauthorized();
            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            if (await repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");
        }
    }
}