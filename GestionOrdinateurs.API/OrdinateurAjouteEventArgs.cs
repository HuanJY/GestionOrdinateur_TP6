using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// données transmises aux abonnés lors de l'ajout d'un ordinateur
public class OrdinateurAjouteEventArgs : EventArgs
{
    // ordinateur qui vient d'être ajouté
    public required Ordinateur Ordinateur { get; set; }
}