using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Travel.BLL.DTOs.Contact;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContactService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContactMessageDto> SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken = default)
    {
        var message = _mapper.Map<ContactMessage>(dto);
        await _unitOfWork.ContactMessages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContactMessageDto>(message);
    }

    public async Task<IReadOnlyList<ContactMessageDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.ContactMessages.Query()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ContactMessageDto>>(messages);
    }

    public async Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await _unitOfWork.ContactMessages.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ContactMessage), id);

        message.IsRead = true;
        _unitOfWork.ContactMessages.Update(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
