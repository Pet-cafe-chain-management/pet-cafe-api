using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.PetSpeciesModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IPetSpeciesService
{
    // Define methods for managing pet species
    Task<PetSpecies> CreatePetSpeciesAsync(PetSpeciesCreateModel model);
    Task<PetSpecies> UpdatePetSpeciesAsync(Guid id, PetSpeciesUpdateModel model);
    Task DeletePetSpeciesAsync(Guid id);
    Task<PetSpecies?> GetPetSpeciesByIdAsync(Guid id);
    Task<BasePagingResponseModel<PetSpecies>> GetAllPetSpeciesAsync(FilterQuery query);
}


public class PetSpeciesService(IUnitOfWork _unitOfWork) : IPetSpeciesService
{

    public async Task<PetSpecies> CreatePetSpeciesAsync(PetSpeciesCreateModel model)
    {
        var petSpecies = new PetSpecies
        {
            Name = model.Name,
            Description = model.Description,
            IsActive = true
        };

        await _unitOfWork.PetSpeciesRepository.AddAsync(petSpecies);
        await _unitOfWork.SaveChangesAsync();
        return petSpecies;
    }

    public async Task<PetSpecies> UpdatePetSpeciesAsync(Guid id, PetSpeciesUpdateModel model)
    {
        var petSpecies = await _unitOfWork.PetSpeciesRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        _unitOfWork.Mapper.Map(model, petSpecies);

        _unitOfWork.PetSpeciesRepository.Update(petSpecies);
        await _unitOfWork.SaveChangesAsync();
        return petSpecies;
    }

    public async Task DeletePetSpeciesAsync(Guid id)
    {
        var petSpecies = await _unitOfWork.PetSpeciesRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.PetSpeciesRepository.SoftRemove(petSpecies);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PetSpecies?> GetPetSpeciesByIdAsync(Guid id)
    {
        return await _unitOfWork.PetSpeciesRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<PetSpecies>> GetAllPetSpeciesAsync(FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.PetSpeciesRepository.ToPagination(
              pageIndex: query.Page ?? 0,
             pageSize: query.Limit ?? 10,
             searchTerm: query.Q,
             searchFields: ["Name", "Description"],
             sortOrders: query.OrderBy?.ToDictionary(
                 k => k.OrderColumn ?? "CreatedAt",
                 v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
             ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }

         );

        return BasePagingResponseModel<PetSpecies>.CreateInstance(Entities, Pagination); ;
    }
}