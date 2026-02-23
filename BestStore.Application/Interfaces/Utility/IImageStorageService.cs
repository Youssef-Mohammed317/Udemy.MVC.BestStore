using BestStore.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace BestStore.Application.Interfaces.Utility
{
    public interface IImageStorageService
    {
        Task<Result<string>> SaveImageAsync(IFormFile file, string rootPath, params string[] folders);
        Result DeleteImage(string imagePath, string rootPath);
    }

}
