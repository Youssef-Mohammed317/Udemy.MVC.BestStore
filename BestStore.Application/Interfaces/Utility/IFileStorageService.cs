using BestStore.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace BestStore.Application.Interfaces.Utility
{
    public interface IFileStorageService
    {
        Task<Result<string>> SaveAsync(IFormFile file, string rootPath, params string[] folders);
        Result Delete(string filePath, string rootPath);
    }

}
