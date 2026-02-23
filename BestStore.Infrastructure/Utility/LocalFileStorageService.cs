using BestStore.Application.Interfaces.Utility;
using BestStore.Domain.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BestStore.Infrastructure.Utility
{
    public abstract class FileStorageServiceBase : IFileStorageService
    {
        

        public async Task<Result<string>> SaveAsync(IFormFile file, string rootPath, params string[] folders)
        {
            if (file == null)
                return Result<string>.Failure(Error.Failure($"Null.{nameof(file)}", "This file can't be null"));

            if (string.IsNullOrWhiteSpace(rootPath))
                return Result<string>.Failure(Error.Failure("InvalidPath", "Root path is invalid"));

            var uploadPath = BuildUploadPath(rootPath, folders);
            EnsureDirectoryExists(uploadPath);

            var finalFileName = GenerateFileName(file.FileName);
            var fullPath = Path.Combine(uploadPath, finalFileName);

            try
            {
                using var fs = new FileStream(fullPath, FileMode.CreateNew);
                await file.CopyToAsync(fs);
            }
            catch (Exception)
            {
                return Result<string>.Failure(Error.Failure("IOError", "An error occurred while saving the file."));
            }

            return Result<string>.Success(BuildRelativePath(finalFileName, folders));
        }
        public Result Delete(string filePath, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Result.Failure(
                    Error.Failure("Null", "File path is null"));

            if (string.IsNullOrWhiteSpace(rootPath))
                return Result.Failure(
                    Error.Failure("InvalidRoot", "Root path is invalid"));

            var cleanPath = filePath.TrimStart('/');

            if (cleanPath.Contains(".."))
                return Result.Failure(
                    Error.Failure("InvalidPath", "Invalid file path"));

            var fullPath = Path.Combine(rootPath, cleanPath);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Result.Success();
                }

                return Result.Failure(
                    Error.Failure("NotFound", "File not found"));
            }
            catch (IOException)
            {
                return Result.Failure(
                    Error.Failure("IOError", "Error deleting file"));
            }
        }

        protected virtual string GenerateFileName(string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{ext}";
        }

        protected virtual string BuildUploadPath(string rootPath, params string[] folders)
        {
            return Path.Combine(
                new[] { rootPath, "uploads" }
                .Concat(folders ?? Array.Empty<string>())
                .ToArray());
        }

        protected virtual string BuildRelativePath(string fileName, params string[] folders)
        {
            var segments = new[] { "uploads" }
                .Concat(folders ?? Array.Empty<string>())
                .Append(fileName);

            return "/" + string.Join("/", segments);
        }

        protected static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

    }
}
