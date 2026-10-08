using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travel.BLL.DTOs.Blog;
using Travel.BLL.Exceptions;
using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BlogPostDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.BlogPosts.Query()
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<BlogPostDto>>(posts);
    }

    public async Task<BlogPostDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.BlogPosts.Query()
            .Include(p => p.Author)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(BlogPost), id);

        return _mapper.Map<BlogPostDto>(post);
    }

    public async Task<IReadOnlyList<BlogPostDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.BlogPosts.Query()
            .Include(p => p.Author)
            .Include(p => p.Images)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<BlogPostDto>>(posts);
    }

    public async Task<BlogPostDto> CreateAsync(string authorId, CreateBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        var post = _mapper.Map<BlogPost>(dto);
        post.AuthorId = authorId;
        post.PublishedAt = dto.IsPublished ? DateTime.UtcNow : null;

        var order = 0;
        foreach (var url in dto.ImageUrls)
            post.Images.Add(new BlogPostImage { ImageUrl = url, DisplayOrder = order++ });

        await _unitOfWork.BlogPosts.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(post.Id, cancellationToken);
    }

    public async Task UpdateAsync(int id, UpdateBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.BlogPosts.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(BlogPost), id);

        var wasPublished = post.IsPublished;
        _mapper.Map(dto, post);

        if (dto.IsPublished && !wasPublished)
            post.PublishedAt = DateTime.UtcNow;

        _unitOfWork.BlogPosts.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.BlogPosts.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(BlogPost), id);

        _unitOfWork.BlogPosts.Remove(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateImagesAsync(int id, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.BlogPosts.Query(asNoTracking: false)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(BlogPost), id);

        foreach (var existingImage in post.Images.ToList())
            _unitOfWork.BlogPostImages.Remove(existingImage);

        var order = 0;
        foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            post.Images.Add(new BlogPostImage { ImageUrl = url.Trim(), DisplayOrder = order++ });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
