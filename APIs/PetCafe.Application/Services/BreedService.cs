using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.BreedModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IBreedService
{
    Task<PetBreed> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<PetBreed>> GetAllPagingAsync(FilterQuery query);
    Task<PetBreed> CreateAsync(BreedCreateModel model);
    Task<PetBreed> UpdateAsync(Guid id, BreedUpdateModel model);
    Task DeleteAsync(Guid id);
}


public class BreedService(
    IUnitOfWork _unitOfWork
) : IBreedService
{
    public async Task<PetBreed> CreateAsync(BreedCreateModel model)
    {
        var breed = _unitOfWork.Mapper.Map<PetBreed>(model);
        await _unitOfWork.PetBreedRepository.AddAsync(breed);
        await _unitOfWork.SaveChangesAsync();
        return breed;
    }

    public async Task<PetBreed> UpdateAsync(Guid id, BreedUpdateModel model)
    {
        var breed = await _unitOfWork.PetBreedRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, breed);
        _unitOfWork.PetBreedRepository.Update(breed);
        await _unitOfWork.SaveChangesAsync();
        return breed;
    }

    public async Task DeleteAsync(Guid id)
    {
        var breed = await _unitOfWork.PetBreedRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.PetBreedRepository.SoftRemove(breed);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PetBreed> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.PetBreedRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<PetBreed>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.PetBreedRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                k => k.OrderColumn ?? "CreatedAt",
                v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
            ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Species)

        );

        return BasePagingResponseModel<PetBreed>.CreateInstance(Entities, Pagination); ;

    }
}