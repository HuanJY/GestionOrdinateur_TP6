using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Tests;

// tests unitaires 
public class OrdinateurServiceTest
{
    // creation d'un service branché sur une base de test vide
    private OrdinateurService CreerService()
    {
        return new OrdinateurService(CreerContexte());
    }
    
    //  on creer un ordinateur de test
    private static Ordinateur CreerOrdinateur()
    {
        return new Ordinateur
        {
            Nom = "ORDITEST",
            Stock = 2,
            Prix = 999,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "Asus", "Intel Core i7")
        };
    }
    // on ajoute un ordinateur : le parc compte un ordinateur de plus
    [Fact]
    public async Task Ajouter_UnOrdinateur_AugmenteLeNombre()
    {
        // Arrange
        var service = CreerService();
        int nombreAvant = (await service.Lister()).Count;

        // Act
        await service.Ajouter(CreerOrdinateur());

        // Assert
        Assert.Equal(nombreAvant + 1, (await service.Lister()).Count);
    }

    // on delete un Id existant : true et donc un ordinateur en moins
    [Fact]
    public async Task Supprimer_OrdinateurExistant_RetourneTrueEtDiminueLeNombre()
    {
        // Arrange : la base est vide, on ajoute d'abord un ordinateur
        var service = CreerService();
        var ordinateur = await service.Ajouter(CreerOrdinateur());
        int nombreAvant = (await service.Lister()).Count;

        // Act
        bool resultat = await service.Supprimer(ordinateur.Id);

        // Assert
        Assert.True(resultat);
        Assert.Equal(nombreAvant - 1, (await service.Lister()).Count);
        Assert.Null(await service.ObtenirParId(ordinateur.Id));
    }

    // on delete un Id inexistant : false et donc le parc inchangé
    [Fact]
    public async Task Supprimer_OrdinateurInexistant_RetourneFalse()
    {
        // Arrange
        var service = CreerService();
        int nombreAvant = (await service.Lister()).Count;

        // Act
        bool resultat = await service.Supprimer(99);

        // Assert
        Assert.False(resultat);
        Assert.Equal(nombreAvant, (await service.Lister()).Count);
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