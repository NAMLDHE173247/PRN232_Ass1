using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using Microsoft.EntityFrameworkCore;

namespace ass01.DataAccess.DAOs;

public class AccountDAO : IAccountDAO
{
    private readonly FunewsManagementContext _context;

    public AccountDAO(FunewsManagementContext context)
    {
        _context = context;
    }
}
