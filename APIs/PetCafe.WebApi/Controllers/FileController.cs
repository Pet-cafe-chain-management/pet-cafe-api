using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Services.Commons;

namespace PetCafe.WebApi.Controllers;

public class FileController(IFileService fileService) : BaseController
{
    private readonly IFileService _fileService = fileService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileById(Guid id)
    {
        return Ok(await _fileService.GetFileAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetList(List<Guid> ids)
    {
        return Ok(await _fileService.GetListAsync(ids));
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile formFile)
    {
        return Ok(await _fileService.UploadFileAsync(formFile));
    }
}