using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;


namespace GestionOrdinateurs.API;

 
public class OrdinateurService : IOrdinateurService
{
    // accès à la base de données
    private readonly ParcInformatiqueContext _context;

    // contexte enregistré dans Program.cs
    public OrdinateurService(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // événements émis : les abonnés sont notifiés à chaque opération
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // renvoie tous les ordinateurs de la base de donnees
    public async Task<List<Ordinateur>> Lister()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    // renvoi uniquement les ordin dispo avec stock > 0
    public async Task<List<Ordinateur>> ListerDisponibles()
    {
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    // Renvoie l'ordinateur correspondant passé en paramètre ou null s'il n'existe pas
    public async Task<Ordinateur?> ObtenirParId(int id)
    {
        return await _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }

    // ajoute l'ordinateur en base (
    public async Task<Ordinateur> Ajouter(Ordinateur ordinateur)
    {
        _context.Ordinateurs.Add(ordinateur);
        await  _context.SaveChangesAsync();
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs { Ordinateur = ordinateur });
        
        return ordinateur;
    }

    // met à jour les infos et false si l'ordinateur n'existe pas
    public async Task <bool> MettreAJour(int id, Ordinateur modifie)
    {
        var existant = await ObtenirParId(id);
        if (existant == null)
            return false;

        existant.Nom = modifie.Nom;
        existant.Etat = modifie.Etat;
        existant.Stock = modifie.Stock;     
        existant.Prix = modifie.Prix;
        existant.Caracteristiques = modifie.Caracteristiques;
        existant.Vendu = modifie.Vendu;
        await _context.SaveChangesAsync();
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs { Ordinateur = existant });
        return true;
    }

    // supprime l'ordinateur et renvoie false s'il n'existe pas
    public async Task<bool> Supprimer(int id)
    {
        var existant = await ObtenirParId(id);
        if (existant == null)
            return false;

        _context.Ordinateurs.Remove(existant);
        await _context.SaveChangesAsync();

        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs { Ordinateur = existant });
        return true;
    }

    // BONUS ; renvoie les ordinateurs d'une marque (q6 TP3 aussi)
    public async Task<List<Ordinateur>> RechercherParMarque(string marque)
    {
        return await _context.Ordinateurs
            .Where(o => o.Caracteristiques.Marque.ToLower() == marque.ToLower())
            .ToListAsync();
    }


    // reoturne les ordis triés du plus cher au moins cher
    public async Task<List<Ordinateur>> ListerParPrixDecroissant()
    {
        return await _context.Ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }


    // diagnostic 
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
            {
                {
                    Etat: EtatOrdinateur.HorsService
                }   => "Remplacement",
                {
                    Etat: EtatOrdinateur.Lent, 
                    Caracteristiques:
                    {
                        RamGo: < 8
                    }
                } 
                    => "Ajouter RAM", 
                _
                    => "Aucune action"
            };
    }


    //stats du parc
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques()
    {
        
        var ordinateurs = _context.Ordinateurs.ToList();

        int total = ordinateurs.Count;
        int disponibles = ordinateurs.Count(o => o.Stock > 0);
        decimal valeurStock = ordinateurs.Sum(o => o.Prix * o.Stock);
        int nbReparations = _context.Reparations.Count();

        return (total, disponibles, valeurStock, nbReparations);
    }
}