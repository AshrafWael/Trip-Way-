using Travel.BLL.DTOs.Contact;

namespace Travel.BLL.Interfaces;

public interface IContactService
{
    Task<ContactMessageDto> SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken = default);

    // Admin
    Task<IReadOnlyList<ContactMessageDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
}
