using LocalJsonModule.DTOs.Folders;
using LocalJsonModule.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalWebModule.Controllers;

[ApiController]
[Route("api/folders")]
public class FolderController : ControllerBase
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var folders = await _folderService.GetAllAsync();
            return Ok(folders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var folder = await _folderService.GetByIdAsync(id);

            if (folder == null)
            {
                return NotFound("Папка не найдена.");
            }

            return Ok(folder);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{ownerId:guid}/owner")]
    public async Task<IActionResult> GetByOwnerId(Guid ownerId)
    {
        try
        {
            var folders = await _folderService.GetByOwnerIdAsync(ownerId);
            return Ok(folders);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{ownerId:guid}/parent/{parentFolderId:guid}/owner")]
    public async Task<IActionResult> GetByParentFolderId(Guid ownerId, Guid parentFolderId)
    {
        try
        {
            var folders = await _folderService.GetByParentFolderIdAsync(ownerId, parentFolderId);
            return Ok(folders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{ownerId:guid}/owner/root")]
    public async Task<IActionResult> GetRootFolders(Guid ownerId)
    {
        try
        {
            var folders = await _folderService.GetByParentFolderIdAsync(ownerId, null);
            return Ok(folders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFolderRequest request)
    {
        try
        {
            var folder = await _folderService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = folder.Id }, folder);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFolderRequest request)
    {
        try
        {
            bool updated = await _folderService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound("Папка не найдена.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            bool deleted = await _folderService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Папка не найдена.");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{ownerId:guid}/owner/delete")]
    public async Task<IActionResult> DeleteByOwnerId(Guid ownerId)
    {
        try
        {
            bool deleted = await _folderService.DeleteByOwnerIdAsync(ownerId);

            if (!deleted)
            {
                return NotFound("У владельца нет папок.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
