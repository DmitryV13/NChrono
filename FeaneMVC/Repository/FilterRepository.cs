
using FeaneMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using FinalProject.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeaneMVC.Models;

namespace FeaneMVC.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        private readonly ApplicationDbContext _context;

        public FilterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetUserFiltersAsync(Guid userId)
        {
            return await _context.UserFilters
                .Where(f => f.UserId == userId)
                .Select(f => f.Filter)
                .Distinct()
                .ToListAsync();
        }

        public async Task AddFiltersAsync(Guid userId, List<string> filters)
        {
            foreach (var filter in filters)
            {
                var userFilter = new UserFilter
                {
                    Id = Guid.NewGuid(),
                    Filter = filter,
                    UserId = userId
                };
                _context.UserFilters.Add(userFilter);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddSingleFilterAsync(Guid userId, string filter)
        {
            var userFilter = new UserFilter
            {
                Id = Guid.NewGuid(),
                Filter = filter,
                UserId = userId
            };
            _context.UserFilters.Add(userFilter);
            await _context.SaveChangesAsync();
        }
    }
}