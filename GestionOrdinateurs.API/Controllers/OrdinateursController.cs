using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GestionOrdinateurs.API.Controllers;



//api rest du parc d'ordinateurs
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    //CRUD (injection)
    private readonly IOrdinateurService _service;

    //logger afin de suivre l'activité de l'api
    private readonly ILogger<OrdinateursController> _logger;

    private readonly JournalService _journal;

    //accès aux param de apsettings.json
    private readonly IConfiguration _configuration;
    public OrdinateursController(IOrdinateurService service, ILogger<OrdinateursController> logger, IConfiguration configuration,  JournalService journal)
    {
        _service = service;
        _logger = logger;
        _configuration = configuration;
        _journal = journal;

        _service.OrdinateurAjoute += AbonneOrdinateur.AfficherMessageAjout;
        _service.OrdinateurMisAJour += AbonneOrdinateur.AfficherMessageMiseAJour;
        _service.OrdinateurSupprime += AbonneOrdinateur.AfficherMessageSuppression;

        _service.OrdinateurAjoute += (sender, e) => _journal.EcrireOrdinateur($"{e.Ordinateur.Nom} ajouté");
        _service.OrdinateurMisAJour += (sender, e) => _journal.EcrireOrdinateur($"{e.Ordinateur.Nom} modifié");
        _service.OrdinateurSupprime += (sender, e) => _journal.EcrireOrdinateur($"{e.Ordinateur.Nom} supprimé");
    }


    //get api
    [HttpGet]
    public async Task<IActionResult> ListerOrdinateurs()
    {
        _logger.LogInformation("Liste des ordinateurs demandée");
        return Ok(await _service.Lister());
    }

    //get id
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenirOrdinateurParId(int id)
    {
        var ordinateur = await _service.ObtenirParId(id);
        if (ordinateur == null)
        {
            _logger.LogWarning($"Ordinateur introuvable : Id {id}");
            return NotFound();
        }

        _logger.LogInformation($"Ordinateur consulté : Id {id}");
        return Ok(ordinateur);
    }

    //post api
    [HttpPost]
    public async Task<IActionResult> AjouterOrdinateur(Ordinateur ordinateur)
    {
        var cree = await _service.Ajouter(ordinateur);
        _logger.LogInformation($"Ordinateur ajouté : {cree.Nom} (Id {cree.Id})");

        return Created($"/api/ordinateurs/{cree.Id}", cree);
    }


    //put id
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> ModifierOrdinateur(int id, Ordinateur ordinateur)
    {
        if (!await _service.MettreAJour(id, ordinateur))
        {
            _logger.LogWarning($"Modification impossible, ordinateur introuvable : Id {id}");
            return NotFound();
        }

        _logger.LogInformation($"Ordinateur modifié : Id {id}");
        return Ok(await _service.ObtenirParId(id));
    }

    //delete id
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> SupprimerOrdinateur(int id)
    {
        if (!await _service.Supprimer(id))
        {
            _logger.LogWarning($"Suppression impossible, ordinateur introuvable : Id {id}");
            return NotFound();
        }

        _logger.LogInformation($"Ordinateur supprimé : Id {id}");
        return NoContent();
    }

    //Q7 TP3
    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> RechercherParMarque(string marque)
    {
        _logger.LogInformation($"Recherche par marque : {marque}");
        return Ok(await _service.RechercherParMarque(marque));
    }



//get api : Infos de l'application lues dans appsetings.json
    [HttpGet("info")]
    public IActionResult ObtenirInfos()
    {
        _logger.LogInformation("Informations de l'application demandées");
        var infos = new
        {
            Nom = _configuration["Application:Nom"],
            Version = _configuration["Application:Version"],
            Description = _configuration["Application:Description"],
            Auteur = _configuration["Application:Auteur"]
            };
            return Ok(infos);
            }


    [HttpGet("disponibles")]
    public async Task<IActionResult> ListerDisponibles()
    {
        _logger.LogInformation("Liste des ordinateurs disponibles demandée");
        return Ok(await _service.ListerDisponibles());
    }


    [HttpGet("parPrix")]
    public async Task<IActionResult> ListerParPrixDecroissant()
    {
        _logger.LogInformation("Liste des ordi triée par prix décroissant demandée");
        return Ok(await _service.ListerParPrixDecroissant());
    }


    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> Diagnostiquer(int id)
    {
        var ordinateur = await _service.ObtenirParId(id);
        if (ordinateur == null)
        {
            _logger.LogWarning($"Diagnostic impossible, ordinateur introuvable : Id {id}");
            return NotFound();
        }

        var diagnostic = _service.Diagnostiquer(ordinateur);
        _logger.LogInformation($"Diagnostic de l'ordinateur {id} : {diagnostic}");

        return Ok(diagnostic);
    }


    
    [HttpGet("statistiques")]
    public IActionResult ObtenirStatistiques()
    {
        var (total, enStock, valeurStock, nbReparations) = _service.CalculerStatistiques();

        _logger.LogInformation("Statistiques du parc demandées");

        return Ok(new
        {
            TotalOrdinateurs = total,
            OrdinateursDisponibles = enStock,
            ValeurStock = valeurStock,
            NbReparations = nbReparations
        });
    }


    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> Chercher(string nom)
    {
        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = await _service.Lister();

        var ordinateur = inventaire[nom];
        if (ordinateur == null)
        {
            _logger.LogWarning($"Inventaire : aucun ordinateur nommé {nom}");
            return NotFound();
        }

        _logger.LogInformation($"Inventaire : ordinateur trouvé pour le nom {nom}");
        return Ok(ordinateur);
    }
}