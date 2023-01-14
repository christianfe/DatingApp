using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]

	public class UsersController : BaseApiController
	{
		private readonly IUnitOfwork _uow;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;

		public UsersController(IUnitOfwork uow, IMapper mapper, IPhotoService photoService)
		{
			_mapper = mapper;
			_uow = uow;
			_photoService = photoService;
		}

		//[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
		{
			var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
			userParams.CurrentUsername = User.GetUsername();

			if (string.IsNullOrEmpty(userParams.Gender))
				userParams.Gender = gender == "male" ? "female" : "male";

			var users = await _uow.UserRepository.GetMembersAsync(userParams);
			Response.AddPaginationsHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalPages, users.TotalCount));
			return Ok(users);
		}

		[HttpGet("{username}")]
		public async Task<ActionResult<MemberDto>> GetUser(string username)
		{
			return await _uow.UserRepository.GetMembersAsync(username);
		}

		[HttpPut]
		public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
		{
			var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			if (user == null) return NotFound();

			_mapper.Map(memberUpdateDto, user);

			if (await _uow.Complete()) return NoContent();

			return BadRequest("Failed to update user");
		}

		[HttpPost("add-photo")]
		public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
		{
			var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			if (user == null) return NotFound();
			var result = await _photoService.AddPhotoAsync(file);

			if (result.Error != null) return BadRequest(result.Error.Message);
			var photo = new Photo
			{
				Url = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId
			};
			if (user.Photos.Count == 0) photo.IsMain = true;
			user.Photos.Add(photo);
			if (await _uow.Complete())
			{
				return CreatedAtAction(
					nameof(GetUser),
					new { username = user.UserName },
					_mapper.Map<PhotoDto>(photo));
			};
			return BadRequest("Problem occures adding photo");
		}

		[HttpPut("set-main-photo/{photoId}")]
		public async Task<ActionResult> SetMainPhoto(int photoId)
		{
			var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			if (user == null) return NotFound();
			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
			if (photo == null) return NotFound();
			if (photo.IsMain) return BadRequest("This is already the main photo");
			var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
			if (currentMain != null) currentMain.IsMain = false;
			photo.IsMain = true;
			if (await _uow.Complete()) return NoContent();
			return BadRequest("Problems occured setting the main photo");
		}

		[HttpDelete("delete-photo/{photoId}")]
		public async Task<ActionResult> DeletePhoto(int photoId)
		{
			var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
			if (photo == null) return NotFound();
			if (photo.IsMain) return BadRequest("You cannot delete the main photo");
			if (photo.PublicId != null)
			{
				var result = await _photoService.DeletePhotoAsync(photo.PublicId);
				if (result.Error != null) return BadRequest(result.Error.Message);
			}
			user.Photos.Remove(photo);
			if (await _uow.Complete()) return NoContent();
			return BadRequest("A problem occured while deleting the photo");
		}

	}
}
