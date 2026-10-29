namespace GestionOrdinateurs.Shared;


public class InventaireOrdinateurs
{
    
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    public Ordinateur? this[int id]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Id == id); }
    }

    public Ordinateur? this[string nom]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Nom.ToLower() == nom.ToLower()); }
    }
}