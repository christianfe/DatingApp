using API.DTOs;
using API.Entities;

namespace API.Extensions
{
	public interface IUserRepository
	{
		void Update(AppUser user);
		Task<bool> SaveAllAsync();
		Task<IEnumerable<AppUser>> GetUsersAsync();
		Task<AppUser> GetUserByIdAsync(int id);
		Task<AppUser> GetUSerByUsernameAsync(string username);
		Task<IEnumerable<MemberDto>> GetMembersAsync();
		Task<MemberDto> GetMembersAsync(string username);
	}
}