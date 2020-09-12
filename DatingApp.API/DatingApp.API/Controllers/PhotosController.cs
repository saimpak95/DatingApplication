using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Helpers;
using DatingApp.DomainModels;
using DatingApp.Repository;
using DatingApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/user/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryOption;
        private Cloudinary Cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryOption)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.cloudinaryOption = cloudinaryOption;

            Account account = new Account
            {
                Cloud = cloudinaryOption.Value.CloudName,
                ApiKey = cloudinaryOption.Value.ApiKey,
                ApiSecret = cloudinaryOption.Value.ApiSecret


            };
            Cloudinary = new Cloudinary(account);
        }

        [HttpGet("{Id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int Id)
         {
            var photoFromRepo = await repo.GetPhoto(Id);
            var photo = mapper.Map<PhotoForReturnViewModel>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreatingViewModel photoViewModel)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromrepo = await repo.GetUserByID(userId);
            var file = photoViewModel.File;
            var uploadsResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {

                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };
                    uploadsResult = Cloudinary.Upload(uploadParams);
                }
            }
            photoViewModel.Url = uploadsResult.Uri.ToString();
            photoViewModel.PublicID = uploadsResult.PublicId;
            var photo = mapper.Map<Photo>(photoViewModel);
            if (!userFromrepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromrepo.Photos.Add(photo);

            if (await repo.SaveAll())
            {
                var photoToReturn = mapper.Map<PhotoForReturnViewModel>(photo);
                var photoFromRepo = await repo.GetPhoto(photo.Id);

                var photoByID = mapper.Map<PhotoForReturnViewModel>(photoFromRepo);

                return Ok(photoByID);
            }
            return BadRequest("Could not add the photo");
        }
        [HttpPost("{photoId}/SetMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromrepo = await repo.GetUserByID(userId);
            if (!userFromrepo.Photos.Any(p => p.Id == photoId))
            {
                return Unauthorized();
            }
            var photoFromRepor = await repo.GetPhoto(photoId);
            if(photoFromRepor.IsMain)
            {
                return BadRequest("This is already the main photo");
            }
            var currentMainPhoto = await repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepor.IsMain = true;

            if (await repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");

        }
        [HttpDelete("{photoID}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoID)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromrepo = await repo.GetUserByID(userId);
            if (!userFromrepo.Photos.Any(p => p.Id == photoID))
            {
                return Unauthorized();
            }
            var photoFromRepor = await repo.GetPhoto(photoID);
            if (photoFromRepor.IsMain)
            {
                return BadRequest("You can't delete the main photo");
            }
            if(photoFromRepor.PublicID != null)
            {
                var deleteParams = new DeletionParams(photoFromRepor.PublicID);
                var result = Cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    repo.Delete(photoFromRepor);
                }
            }
            if (photoFromRepor == null)
            {
                repo.Delete(photoFromRepor);
            }
            if(await repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photo");
        }

    }
}
