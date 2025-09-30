using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.PetGroupModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IPetGroupService
{
    Task<PetGroup> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<PetGroup>> GetAllPagingAsync(FilterQuery query);
    Task<PetGroup> CreateAsync(PetGroupCreateModel model);
    Task<PetGroup> UpdateAsync(Guid id, PetGroupUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class PetGroupService(
    IUnitOfWork _unitOfWork
) : IPetGroupService
{
    public async Task<PetGroup> CreateAsync(PetGroupCreateModel model)
    {
        var petGroup = _unitOfWork.Mapper.Map<PetGroup>(model);
        await _unitOfWork.PetGroupRepository.AddAsync(petGroup);
        await _unitOfWork.SaveChangesAsync();
        return petGroup;
    }

    public async Task<PetGroup> UpdateAsync(Guid id, PetGroupUpdateModel model)
    {
        var petGroup = await _unitOfWork.PetGroupRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, petGroup);
        _unitOfWork.PetGroupRepository.Update(petGroup);
        await _unitOfWork.SaveChangesAsync();
        return petGroup;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var petGroup = await _unitOfWork.PetGroupRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.PetGroupRepository.SoftRemove(petGroup);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PetGroup> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.PetGroupRepository.GetByIdAsync(id,
          includeFunc: x => x
            .Include(x=>x.Pets.Where(x=>!x.IsDeleted))
            .Include(x => x.PetSpecies!)
            .Include(x => x.PetBreed!)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<PetGroup>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.PetGroupRepository.ToPagination(
           pageIndex: query.Page ?? 0,
          pageSize: query.Limit ?? 10,
          searchTerm: query.Q,
          searchFields: ["Name", "Description"],
          sortOrders: query.OrderBy?.ToDictionary(
                  k => k.OrderColumn ?? "CreatedAt",
                  v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
              ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
          includeFunc: x => x.Include(x => x.PetSpecies!).Include(x => x.PetBreed!)
      );
        return BasePagingResponseModel<PetGroup>.CreateInstance(Entities, Pagination);
    }
}