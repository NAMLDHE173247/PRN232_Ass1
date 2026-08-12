using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using ass01.DataAccess.DAOs;

namespace ass01.DataAccess.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ITagDAO _dao;

    public TagRepository(ITagDAO dao)
    {
        _dao = dao;
    }

    public Task<List<Tag>> GetTagsAsync() => _dao.GetTagsAsync();

    public Task<Tag?> GetTagByIdAsync(int id) => _dao.GetTagByIdAsync(id);

    public Task AddTagAsync(Tag tag) => _dao.AddTagAsync(tag);

    public Task UpdateTagAsync(Tag tag) => _dao.UpdateTagAsync(tag);

    public Task DeleteTagAsync(Tag tag) => _dao.DeleteTagAsync(tag);
}
