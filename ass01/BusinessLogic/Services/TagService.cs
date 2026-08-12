using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.Tag;
using ass01.DataAccess.Repositories;
using ass01.Models;

namespace ass01.BusinessLogic.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TagDto>> GetTagsAsync()
    {
        var tags = await _repository.GetTagsAsync();
        return tags.Select(t => new TagDto
        {
            TagId = t.TagId,
            TagName = t.TagName,
            Note = t.Note
        }).ToList();
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var t = await _repository.GetTagByIdAsync(id);
        if (t == null) return null;

        return new TagDto
        {
            TagId = t.TagId,
            TagName = t.TagName,
            Note = t.Note
        };
    }

    public async Task<TagDto> CreateTagAsync(CreateTagRequest request)
    {
        var tag = new Tag
        {
            TagName = request.TagName,
            Note = request.Note
        };

        await _repository.AddTagAsync(tag);

        return new TagDto
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Note = tag.Note
        };
    }

    public async Task UpdateTagAsync(int id, UpdateTagRequest request)
    {
        var tag = await _repository.GetTagByIdAsync(id);
        if (tag == null)
            throw new KeyNotFoundException("Tag not found.");

        tag.TagName = request.TagName;
        tag.Note = request.Note;

        await _repository.UpdateTagAsync(tag);
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _repository.GetTagByIdAsync(id);
        if (tag == null)
            throw new KeyNotFoundException("Tag not found.");

        await _repository.DeleteTagAsync(tag);
    }
}
