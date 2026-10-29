using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;


public interface IOrdinateurService
{
    // renvoie tous les ordinateurs
    Task <List<Ordinateur>> Lister();

    // renvoi l'ordinateur correspondant à l'Id passe en paramètre ou null
    Task <Ordinateur?> ObtenirParId(int id);

    // ajoute un ordinateur et le renvoie avec son Id
    Task <Ordinateur> Ajouter(Ordinateur ordinateur);

    // Met à jour un ordinateur ; sinon false
    Task <bool> MettreAJour(int id, Ordinateur modifie);

    // supprime un ordinateur ; sinon flase
    Task <bool> Supprimer(int id);

    // renvoie les ordinateurs de la marque en paramètre
    Task <List<Ordinateur>> RechercherParMarque(string marque);

    Task<List<Ordinateur>> ListerDisponibles();

    Task<List<Ordinateur>> ListerParPrixDecroissant();

    string Diagnostiquer(Ordinateur ordinateur);

    // événements émis lors des opérations sur le parc de pc
    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // reoturne les statistiques du parc info
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}