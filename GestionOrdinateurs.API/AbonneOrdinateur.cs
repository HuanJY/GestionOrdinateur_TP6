namespace GestionOrdinateurs.API;

// affichent un message dans la console pour les abonnees
public static class AbonneOrdinateur
{
    // reagit pour l'ajout d'un ordinateur
    public static void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine($"[ÉVÉNEMENT] Ordinateur ajouté : {e.Ordinateur.Nom} (Id {e.Ordinateur.Id})");
    }

    // réagit à la mise à jour d'un ordinateur
    public static void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine($"[ÉVÉNEMENT] Ordinateur mis à jour : {e.Ordinateur.Nom} (Id {e.Ordinateur.Id})");
    }

    // réagit lors de la suppression d'un ordinateur
    public static void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine($"[ÉVÉNEMENT] Ordinateur supprimé : {e.Ordinateur.Nom} (Id {e.Ordinateur.Id})");
    }
}