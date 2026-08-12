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
}
