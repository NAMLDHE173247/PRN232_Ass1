using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using ass01.DataAccess.DAOs;

namespace ass01.DataAccess.Repositories;

public class NewsArticleRepository : INewsArticleRepository
{
    private readonly INewsArticleDAO _dao;

    public NewsArticleRepository(INewsArticleDAO dao)
    {
        _dao = dao;
    }
}
