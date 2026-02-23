using BestStore.Application.Interfaces.Utility;
using BestStore.Shared.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BestStore.Infrastructure.Utility
{
    public class ImageStorageService : FileStorageServiceBase, IImageStorageService
    {
        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        public ImageStorageService(IHostEnvironment env)
            : base(env)
        {
        }

        public async Task<Result<string>> SaveImageAsync(
     IFormFile file,
     string rootPath,
     params string[] folders)
        {
            if (file == null)
                return Result<string>.Failure(
                    Error.Failure("Null.File", "File can't be null"));

            var validation = ValidateImage(file.FileName);

            if (validation.IsFailure)
                return Result<string>.Failure(validation.Error);

            var finalFolders = new[] { "images" }
                .Concat(folders ?? Array.Empty<string>())
                .ToArray();

            return await SaveAsync(file, rootPath, finalFolders);
        }

        public Result DeleteImage(string imagePath, string rootPath)
        {

            return Delete(imagePath, rootPath);
        }

        private static Result ValidateImage(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(ext))
            {
                return Result.Failure(Error.Failure("NotAllowed", "Only image files are allowed."));
            }
            return Result.Success();
        }
    }
}
