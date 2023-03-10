using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class UserRepository : IUserRepository
	{
		public readonly DataContext _context;
		public readonly IMapper _mapper;
		public UserRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
		{
			var query = _context.Users.AsQueryable();

			query = query.Where(u => u.UserName != userParams.CurrentUsername);
			query = query.Where(u => u.Gender == userParams.Gender);

			var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
			var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

			query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
			query = userParams.OrderBy switch
			{
				"created" => query.OrderByDescending(u => u.Created),
				_ => query.OrderByDescending(u => u.LastActive)
			};

			return await PagedList<MemberDto>.CreateAsync(
				query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
				userParams.PageNumber,
				userParams.PageSize);
		}

		public async Task<MemberDto> GetMembersAsync(string username, bool isCurrentUser)
		{
			var query = _context.Users
				.Where(x => x.UserName == username)
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.AsQueryable();

			if (isCurrentUser) query = query.IgnoreQueryFilters();

			return await query.FirstOrDefaultAsync();
		}

		public async Task<AppUser> GetUserByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task<AppUser> GetUserByUsernameAsync(string username, bool isCurrentUser = false)
		{

			var query = _context.Users
				.Include(p => p.Photos)
				.AsQueryable();

			if (isCurrentUser) query = query.IgnoreQueryFilters();

			return await query.SingleOrDefaultAsync(x => x.UserName == username); ;
		}

		public async Task<string> GetUserGender(string username)
		{
			return await _context.Users
				.Where(x => x.UserName == username)
				.Select(x => x.Gender)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<AppUser>> GetUsersAsync()
		{
			return await _context.Users
				.Include(p => p.Photos)
				.ToListAsync();
		}

		public void Update(AppUser user)
		{
			_context.Entry(user).State = EntityState.Modified;
		}
	}
}