namespace GestionOrdinateurs.API;

public class JournalService
{
    private static readonly string _dossierReparations = Path.Combine("Logs", "Reparations_Logs");
    private static readonly string _dossierOrdinateurs = Path.Combine("Logs", "Ordinateurs");

    private static readonly object _verrouFichier = new object();

    public JournalService()
    {
        Directory.CreateDirectory(_dossierReparations);
        Directory.CreateDirectory(_dossierOrdinateurs);
    }

    public void Ecrire(string message)
    {
        EcrireDansFichier(Path.Combine(_dossierReparations, "reparations.log"), message);
    }

    public void EcrireOrdinateur(string message)
    {
        EcrireDansFichier(Path.Combine(_dossierOrdinateurs, "ordinateurs.log"), message);
    }

    private void EcrireDansFichier(string chemin, string message)
    {
        lock (_verrouFichier)
        {
            using var writer = new StreamWriter(chemin, append: true);
            writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} - {message}");
        }
    }
}