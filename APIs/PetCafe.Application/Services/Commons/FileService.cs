using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services.Commons;

public interface IFileService
{
    Task<Storage> UploadFileAsync(IFormFile fileUpload, string folder = FolderConstants.IMAGES);

    Task<bool> RemoveFileAsync(Guid fileId, string folder);

    Task<Storage> GetFileAsync(Guid fileId);

    Task<List<Storage>> GetListAsync(List<Guid> fileIds);

}


public class FileService(AppSettings appSettings, IUnitOfWork unitOfWork) : IFileService
{
    private readonly AppSettings _appSettings = appSettings;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Storage> GetFileAsync(Guid fileId)
    {
        var file = await _unitOfWork.StorageRepository.GetByIdAsync(fileId) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        return file;
    }

    public async Task<List<Storage>> GetListAsync(List<Guid> fileIds)
    {
        if (fileIds == null || fileIds.Count == 0) throw new BadRequestException("Invalid parameter!");

        return await _unitOfWork.StorageRepository.WhereAsync(x => fileIds.Contains(x.Id));
    }

    public Task<bool> RemoveFileAsync(Guid fileId, string folder)
    {
        throw new Exception();
    }

    public async Task<Storage> UploadFileAsync(IFormFile fileUpload, string folder = FolderConstants.IMAGES)
    {
        if (fileUpload.Length > 0)
        {
            var fs = fileUpload.OpenReadStream();
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey: _appSettings.FirebaseSettings.ApiKeY));

            var a = await auth.SignInWithEmailAndPasswordAsync(email: _appSettings.FirebaseSettings.AuthEmail, password: _appSettings.FirebaseSettings.AuthPassword);

            var extension = Path.GetExtension(fileUpload.FileName);

            var newFileName = $"{Guid.NewGuid()}{extension}";

            var cancellation = new FirebaseStorage(
                _appSettings.FirebaseSettings.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("assets/" + folder)
                .Child(newFileName)
                .PutAsync(fs, CancellationToken.None);

            try
            {
                var url = await cancellation;
                var result = await _unitOfWork.StorageRepository.AddAsync(
                    new Storage
                    {
                        FileName = newFileName,
                        Url = url,
                        Extension = extension,
                        Size = fileUpload.Length,
                        ContentType = fileUpload.ContentType
                    });
                await _unitOfWork.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        else
        {
            throw new BadRequestException("File is not existed!");
        }
    }
}