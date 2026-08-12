using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using Microsoft.EntityFrameworkCore;

namespace ass01.DataAccess.DAOs;

public class CategoryDAO : ICategoryDAO
{
    private readonly FunewsManagementContext _context;

    public CategoryDAO(FunewsManagementContext context)
    {
        _context = context;
    }
}
