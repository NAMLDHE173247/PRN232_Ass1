using System.Collections.Generic;
using System.Threading.Tasks;
using ass01.Models;
using ass01.DataAccess.DAOs;

namespace ass01.DataAccess.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IAccountDAO _dao;

    public AccountRepository(IAccountDAO dao)
    {
        _dao = dao;
    }
}
