using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;



[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    // service des réparations 
    private readonly ReparationService _reparService;

    // Service des ordinateurs : pour récupérer les ordinateurs à réparer
    private readonly IOrdinateurService _ordinateurService;

    private readonly StockService _stockService;

    private readonly ILogger<ReparationController> _logger;
    public ReparationController(ReparationService reparService, IOrdinateurService ordinateurService, StockService stockService, ILogger<ReparationController> logger)
    {
        _reparService = reparService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    //  répare 3 ordinateurs en parallèle
    [Authorize(Roles = "Admin")]
    [HttpPost("SimulerReparation")]
    public async Task<IActionResult> SimulerReparation()
    {
        // récup la liste d'ordinateurs
        var listOrdinateurs = await _ordinateurService.Lister();

        // simulation a besoin d'au moins 3 ordi
        if (listOrdinateurs.Count < 3)
            return BadRequest("Il faut au moins 3 ordi pour lancer la simulation.");

        // lance les 3 réparations en parallèle et attend qu'elles soient toutes terminées
        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2])
        );

        return Ok("3 réparations simulées");
    }

    [HttpGet]
    public async Task<IActionResult> ListerReparations()
    {
        return Ok(await _reparService.ListerReparationsAsync());
    }


    [HttpGet("GetStockSSD")]
    public IActionResult GetStockSSD()
    {
        //  10 SSD en stock, 10 conso et chaque tâche consomme 1 SSD
        Parallel.For(
            0,
            10,
            i => _stockService.ConsommerSSD());

        _logger.LogInformation($"Stock SSD restant : {_stockService.StockSSD}");
        return Ok(_stockService.StockSSD);
    }


    //Q10  TP4 histo des reparation
    [HttpGet("historique")]
    public async Task<IActionResult> ObtenirHistorique()
    {
        _logger.LogInformation("Historique des réparations demandé");
        return Ok(await _reparService.ObtenirHistoriqueAsync());
    }
}
