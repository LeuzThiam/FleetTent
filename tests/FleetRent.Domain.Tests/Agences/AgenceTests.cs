using FleetRent.Domain.Agences;

namespace FleetRent.Domain.Tests.Agences;

public class AgenceTests
{
    private static Agence CreerAgence() =>
        new(
            Guid.NewGuid(),
            "MTL01",
            "Agence Montréal Central",
            "123 Rue Principale, Montréal, QC",
            "514-123-4567",
            "montreal@fleetrent.com",
            "Lun-Ven: 9h-18h, Sam: 10h");

    [Fact]
    public void Constructeur_AvecCodeVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Agence(
            Guid.NewGuid(), "", "Nom", "Adresse", "514-555-0100", "a@b.com", "Lun-Ven"));
    }

    [Fact]
    public void Constructeur_AvecNomVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Agence(
            Guid.NewGuid(), "MTL01", "", "Adresse", "514-555-0100", "a@b.com", "Lun-Ven"));
    }

    [Fact]
    public void Constructeur_ParDefaut_EstActive()
    {
        var agence = CreerAgence();
        Assert.True(agence.EstActive);
    }

    [Fact]
    public void Desactiver_MetEstActiveAFaux()
    {
        var agence = CreerAgence();
        agence.Desactiver();
        Assert.False(agence.EstActive);
    }

    [Fact]
    public void Activer_MetEstActiveAVrai()
    {
        var agence = CreerAgence();
        agence.Desactiver();

        agence.Activer();

        Assert.True(agence.EstActive);
    }

    [Fact]
    public void MettreAJourCoordonnees_MetAJourLesChamps()
    {
        var agence = CreerAgence();

        agence.MettreAJourCoordonnees("456 rue Nouvelle", "514-555-0200", "nouveau@fleetrent.com");

        Assert.Equal("456 rue Nouvelle", agence.Adresse);
        Assert.Equal("514-555-0200", agence.Telephone);
        Assert.Equal("nouveau@fleetrent.com", agence.Email);
    }

    [Fact]
    public void MettreAJourHoraires_MetAJourLHoraire()
    {
        var agence = CreerAgence();

        agence.MettreAJourHoraires("Lun-Dim 7h-20h");

        Assert.Equal("Lun-Dim 7h-20h", agence.HorairesOuverture);
    }
}
