using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.EmployeeModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IEmployeeService
{
    Task<Employee> CreateAsync(EmployeeCreateModel model);
    Task<Employee> UpdateAsync(Guid id, EmployeeUpdateModel model);
    Task DeleteAsync(Guid id);
    Task<Employee> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Employee>> GetAllPagingAsync(FilterQuery query);

}


public class EmployeeService(
    IUnitOfWork _unitOfWork,
    IHashService _hashService
) : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork = _unitOfWork;

    public async Task<Employee> CreateAsync(EmployeeCreateModel model)
    {
        var employee = _unitOfWork.Mapper.Map<Employee>(model);
        var account = new Account
        {
            Email = model.Email,
            Username = model.FullName,
            Role = Domain.Constants.RoleConstants.EMPLOYEE,
            PasswordHash = _hashService.HashPassword(model.Password ?? _hashService.GenerateRandomPassword())
        };

        employee.AccountId = account.Id;

        await _unitOfWork.AccountRepository.AddAsync(account);
        await _unitOfWork.EmployeeRepository.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Guid id, EmployeeUpdateModel model)
    {
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy nhân viên");
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(employee.AccountId) ?? throw new BadRequestException("Không tìm thấy tài khoản nhân viên");
        _unitOfWork.Mapper.Map(model, employee);

        if (!_hashService.VerifyPassword(model.Password ?? "", account.PasswordHash))
        {
            account.PasswordHash = _hashService.HashPassword(model.Password ?? _hashService.GenerateRandomPassword());
            _unitOfWork.AccountRepository.Update(account);
        }
        _unitOfWork.EmployeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy nhân viên");
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(employee.AccountId) ?? throw new BadRequestException("Không tìm thấy tài khoản nhân viên");
        _unitOfWork.AccountRepository.SoftRemove(account);
        _unitOfWork.EmployeeRepository.SoftRemove(employee);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Employee> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.EmployeeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy nhân viên");
    }

    public async Task<BasePagingResponseModel<Employee>> GetAllPagingAsync(FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.EmployeeRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["FullName", "Phone", "Address", "Email"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x
                    .Include(x => x.Account)

        );
        return BasePagingResponseModel<Employee>.CreateInstance(Entities, Pagination); ;
    }
}