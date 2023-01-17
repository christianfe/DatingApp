using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class PhotoRepository : IPhotoRepository
	{
		private readonly DataContext _context;

		public PhotoRepository(DataContext context)
		{
			_context = context;
		}


		public async Task<Photo> GetPhotoById(int id)
		{
			return await _context.Photos
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
		{
			return await _context.Photos
				.IgnoreQueryFilters()
				.Where(p => p.isApproved == false)
				.Select(p => new PhotoForApprovalDto
				{
					Id = p.Id,
					Url = p.Url,
					Username = p.AppUser.UserName,
					IsApproved = p.isApproved
				})
				.ToListAsync();
		}

		public async Task<AppUser> GetUserByPhotoId(int id)
		{
			return await _context.Users
				.Include(p => p.Photos)
				.IgnoreQueryFilters()
				.Where(p => p.Photos.Any(p => p.Id == id))
				.SingleOrDefaultAsync();
		}

		public void RemovePhoto(Photo photo)
		{
			_context.Photos.Remove(photo);
		}
	}
}