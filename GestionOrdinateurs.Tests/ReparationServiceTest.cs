using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Tests;

// tests unitaires du service des réparations
public class ReparationServiceTest
{
    [Fact]
    public async Task CreerReparation_OrdinateurExistant_AjouteLaReparation()
    {
        var context = CreerContexte();
        var ordinateur = new Ordinateur
        {
            Nom = "ORDITEST",
            Stock = 1,
            Prix = 800,
            Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Fixe, "HP", "Intel Core i5")
        };
        context.Ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();

        var service = new ReparationService(context, new JournalService());

        
        var reparation = await service.CreerReparationAsync(ordinateur);


        Assert.Equal(1, await context.Reparations.CountAsync());
        Assert.Equal("En cours", reparation.Statut);
        Assert.Equal(ordinateur.Id, reparation.UnOrdinateur.Id);
    }

    private ParcInformatiqueContext CreerContexte()
    {
        var options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ParcInformatiqueContext(options);
    }
}