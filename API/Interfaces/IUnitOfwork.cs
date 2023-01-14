using API.Extensions;

namespace API.Interfaces
{
	public interface IUnitOfwork
	{
		IUserRepository UserRepository { get; }
		IMessageRepository MessageRepository { get; }
		ILikesRepository LikesRepository { get; }
		Task<bool> Complete();
		bool HasChanges();
	}
}