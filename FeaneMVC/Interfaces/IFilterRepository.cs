using FinalProject.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeaneMVC.Repositories.Interfaces
{
    public interface IFilterRepository
    {
        Task<List<string>> GetUserFiltersAsync(Guid userId);
        Task AddFiltersAsync(Guid userId, List<string> filters);
        Task AddSingleFilterAsync(Guid userId, string filter);
    }
}