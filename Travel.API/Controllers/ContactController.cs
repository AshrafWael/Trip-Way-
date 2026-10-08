using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.BLL.DTOs.Common;
using Travel.BLL.DTOs.Contact;
using Travel.BLL.Interfaces;

namespace Travel.API.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> Submit(CreateContactMessageDto dto, CancellationToken cancellationToken)
    {
        var created = await _contactService.SubmitAsync(dto, cancellationToken);
        return Ok(ApiResponse<ContactMessageDto>.Ok(created, "Thanks -- we'll get back to you soon."));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactMessageDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var messages = await _contactService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ContactMessageDto>>.Ok(messages));
    }

    [HttpPut("{id:int}/read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        await _contactService.MarkAsReadAsync(id, cancellationToken);
        return NoContent();
    }
}
