namespace GestionOrdinateurs.Shared;

// caracteristique d'un ordinateur 
public record Caracteristiques
{
    public int RamGo { get; set; }
    
    //se réferer à l'enum TypeOrdinateur
    public TypeOrdinateur TypeOrdi { get; set; }
    public int StockageGo { get; set; }
    public string Marque { get; set; }
    public string Processeur { get; set; }

    //constructeur
    public Caracteristiques(int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string marque, string processeur)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
        Marque = marque;
        Processeur = processeur;
    }
}