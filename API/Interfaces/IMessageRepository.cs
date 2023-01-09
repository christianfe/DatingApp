using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
	public interface IMessageRepository
	{
		void AddMessage(Message message);
		void DeleteMessage(Message message);
		Task<Message> GetMessage(int Id);
		Task<PagedList<MessageDto>> GetMessagesForUser();
		Task<IEnumerable<MessageDto>> GetMessageThread(int currentUSerId, int recipientId);
		Task<bool> SaveAllAsync();
	}
}