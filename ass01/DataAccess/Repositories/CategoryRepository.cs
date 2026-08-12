using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using ass01.DataAccess.DAOs;

namespace ass01.DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ICategoryDAO _dao;

    public CategoryRepository(ICategoryDAO dao)
    {
        _dao = dao;
    }
}
