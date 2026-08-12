using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using Microsoft.EntityFrameworkCore;

namespace ass01.DataAccess.DAOs;

public class TagDAO : ITagDAO
{
    private readonly FunewsManagementContext _context;

    public TagDAO(FunewsManagementContext context)
    {
        _context = context;
    }
}
