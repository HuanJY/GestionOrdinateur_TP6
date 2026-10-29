using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

//table ordinateur
[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; private set; }

    public required string Nom { get; set; }
    
    public EtatOrdinateur Etat { get; set; }
    
    [DefaultValue(0)]
    public int Stock { get; set; }
    
    [DefaultValue(0)]
    public decimal Prix { get; set; }

    public required Caracteristiques Caracteristiques { get; set; }

    public List<Reparation> Reparations { get; set; } = new();

    [DefaultValue(false)]
    public bool Vendu { get; set; } = false;
}