using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.TransactionModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class TransactionController(ITransactionService _transactionService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] TransactionFilterQuery query)
    {
        return Ok(await _transactionService.GetAllPagingAsync(query));
    }
}