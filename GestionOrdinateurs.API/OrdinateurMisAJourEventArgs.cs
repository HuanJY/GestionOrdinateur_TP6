using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// données transmises aux abonnés lors de la mise à jour d'un ordinateur
public class OrdinateurMisAJourEventArgs : EventArgs
{
    
    public required Ordinateur Ordinateur { get; set; }
}