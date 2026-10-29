using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;


public class ReparationService
{
    public record HistoriqueReparationRecord(
        int OrdinateurId,
        string NomOrdinateur,
        string Technicien,
        string TypeReparation,
        DateTime DateIntervention
    );

    private readonly ParcInformatiqueContext _context;

    private readonly SemaphoreSlim _verrou = new SemaphoreSlim(1, 1);

    private static readonly string[] _techniciens = { "Alice", "Bob", "Chloé", "David", "Emma" };

    private readonly JournalService _journal;

    public ReparationService(ParcInformatiqueContext context, JournalService journal)
    {
        _context = context;
        _journal = journal;
    }

    public async Task<Reparation> CreerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            UnOrdinateur = ordinateur,
            Technicien = _techniciens[ordinateur.Id % _techniciens.Length],
            TypeReparation = "Révision complète",
            DateDebut = DateTime.Now,
            Statut = "En cours"
        };

        await _verrou.WaitAsync();
        try
        {
            _context.Reparations.Add(reparation);
            await _context.SaveChangesAsync();
        }
        finally
        {
            _verrou.Release();
        }

        return reparation;
    }

     
    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        
        var reparation = await CreerReparationAsync(ordinateur);

        
        await Task.Delay(2000);

        await _verrou.WaitAsync();
        try
        {
            reparation.Statut = "Terminée";
            await _context.SaveChangesAsync();
        }
        finally
        {
            _verrou.Release();
        }

        Console.WriteLine($"{ordinateur.Nom} réparé");

        _journal.Ecrire($"{ordinateur.Nom} réparé");
    }

    
    public async Task<List<Reparation>> ListerReparationsAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }


    public async Task<List<HistoriqueReparationRecord>> ObtenirHistoriqueAsync()
    {
        return await _context.Reparations
            .OrderByDescending(r => r.DateDebut)
            .Select(r => new HistoriqueReparationRecord(
                r.UnOrdinateur.Id,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut))
            .ToListAsync();
    }
}