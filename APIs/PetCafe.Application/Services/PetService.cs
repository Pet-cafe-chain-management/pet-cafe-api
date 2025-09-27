using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.PetModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IPetService
{
    Task<Pet> CreateAsync(PetCreateModel model);
    Task<Pet> UpdateAsync(Guid id, PetUpdateModel model);
    Task DeleteAsync(Guid id);
    Task<Pet> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Pet>> GetAllPagingAsync(FilterQuery query);
}

public class PetService(
    IUnitOfWork _unitOfWork
) : IPetService
{
    public async Task<Pet> CreateAsync(PetCreateModel model)
    {
        var pet = _unitOfWork.Mapper.Map<Pet>(model);
        await _unitOfWork.PetRepository.AddAsync(pet);
        await _unitOfWork.SaveChangesAsync();
        return pet;
    }

    public async Task<Pet> UpdateAsync(Guid id, PetUpdateModel model)
    {
        var pet = await _unitOfWork.PetRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, pet);
        _unitOfWork.PetRepository.Update(pet);
        await _unitOfWork.SaveChangesAsync();
        return pet;
    }

    public async Task DeleteAsync(Guid id)
    {
        var pet = await _unitOfWork.PetRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.PetRepository.SoftRemove(pet);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Pet> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.PetRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Pet>> GetAllPagingAsync(FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.PetRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["Gender", "Name", "Color", "SpecialNotes", "Breed", "Species"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Breed).Include(b => b.Species)
        );

        return BasePagingResponseModel<Pet>.CreateInstance(Entities, Pagination); ;

    }
}