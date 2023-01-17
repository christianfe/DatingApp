using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AdminController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IUnitOfwork _uow;
		private readonly IPhotoService _photoService;

		public AdminController(UserManager<AppUser> userManager, IUnitOfwork uow, IPhotoService photoService)
		{
			_uow = uow;
			_userManager = userManager;
			_photoService = photoService;
		}


		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("users-with-roles")]
		public async Task<ActionResult> GetUserWithRoles()
		{
			var users = await _userManager.Users
				.OrderBy(u => u.UserName)
				.Select(u => new
				{
					u.Id,
					Username = u.UserName,
					Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
				})
				.ToListAsync();

			return Ok(users);
		}


		[Authorize(Policy = "RequireAdminRole")]
		[HttpPost("edit-roles/{username}")]
		public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
		{
			if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

			var selectedRoles = roles.Split(",").ToArray();
			var user = await _userManager.FindByNameAsync(username);

			if (user == null) return NotFound();

			var userRoles = await _userManager.GetRolesAsync(user);
			var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
			if (!result.Succeeded) return BadRequest("Failed to add roles");

			result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
			if (!result.Succeeded) return BadRequest("Failed to remove from roles");

			return Ok(await _userManager.GetRolesAsync(user));
		}


		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpGet("photos-to-moderate")]
		public async Task<ActionResult> GetPhotoForModeration()
		{
			var p = await _uow.PhotoRepository.GetUnapprovedPhotos();
			return Ok(p);
		}

		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpPut("approve-photo/{photoId}")]
		public async Task<ActionResult> ApprovePhoto(int photoId)
		{
			var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
			photo.isApproved = true;

			var user = await _uow.PhotoRepository.GetUserByPhotoId(photoId);

			if (!user.Photos.Any(x => x.IsMain))
				photo.IsMain = true;

			await _uow.Complete();
			return NoContent();
		}

		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpDelete("reject-photo/{photoId}")]
		public async Task<ActionResult> RejectPhoto(int photoId)
		{
			var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
			if (photo == null)
				return BadRequest("Error deleting Photo");
			_uow.PhotoRepository.RemovePhoto(photo);
			if (photo.PublicId != null)
				_photoService.DeletePhotoAsync(photo.PublicId);

			return NoContent();
		}
	}
}