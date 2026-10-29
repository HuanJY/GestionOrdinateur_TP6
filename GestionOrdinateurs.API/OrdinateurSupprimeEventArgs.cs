using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// données transmises aux abonnés à la suppression d'un ordinateur
public class OrdinateurSupprimeEventArgs : EventArgs
{
    public required Ordinateur Ordinateur { get; set; }
}