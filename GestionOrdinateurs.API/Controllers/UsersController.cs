using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controllers;

// api des users
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ef core injec
    private readonly ParcInformatiqueContext _context;

    
    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // get api user
    [HttpGet]
    public async Task<IActionResult> RecupererTous()
    {
        return Ok(await _context.Users.ToListAsync());
    }

    // get api users/{login} : 200 si trouvé sinon 404 sinon
    [HttpGet("{login}")]
    public async Task<IActionResult> ChercherParLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}