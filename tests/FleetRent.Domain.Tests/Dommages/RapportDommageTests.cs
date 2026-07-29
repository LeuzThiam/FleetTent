namespace FleetRent.Domain.Tests.Dommages;

using FleetRent.Domain.Dommages;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public class RapportDommageTests
{
    private static RapportDommage CreerRapport() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Rayure profonde sur la portière avant droite",
            "Portière avant droite",
            GraviteDommage.Moderee,
            250.00m);

    [Fact]
    public void Constructeur_ParDefaut_EstSignale()
    {
        var rapport = CreerRapport();

        Assert.Equal(StatutDommage.Signale, rapport.Statut);
    }

    [Fact]
    public void Constructeur_AvecCoutEstimeNegatif_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RapportDommage(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Description", "Zone", GraviteDommage.Legere, -1m));
    }

    [Fact]
    public void Constructeur_AvecZoneVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RapportDommage(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Description", "", GraviteDommage.Legere, 100m));
    }

    [Fact]
    public void CommencerReparation_DepuisSignale_ChangeStatutAEnReparation()
    {
        var rapport = CreerRapport();

        rapport.CommencerReparation();

        Assert.Equal(StatutDommage.EnReparation, rapport.Statut);
    }

    [Fact]
    public void CommencerReparation_DepuisStatutNonSignale_LeveDomainException()
    {
        var rapport = CreerRapport();
        rapport.CommencerReparation();

        Assert.Throws<DomainException>(() => rapport.CommencerReparation());
    }

    [Fact]
    public void Resoudre_DepuisEnReparation_ChangeStatutAResoluEtEnregistreLeCoutReel()
    {
        var rapport = CreerRapport();
        rapport.CommencerReparation();

        rapport.Resoudre(230.00m);

        Assert.Equal(StatutDommage.Resolu, rapport.Statut);
        Assert.Equal(230.00m, rapport.CoutReel);
        Assert.NotNull(rapport.DateResolution);
    }

    [Fact]
    public void Resoudre_DepuisSignale_LeveDomainException()
    {
        var rapport = CreerRapport();

        Assert.Throws<DomainException>(() => rapport.Resoudre(200.00m));
    }

    [Fact]
    public void Resoudre_AvecCoutReelNegatif_LeveArgumentOutOfRangeException()
    {
        var rapport = CreerRapport();
        rapport.CommencerReparation();

        Assert.Throws<ArgumentOutOfRangeException>(() => rapport.Resoudre(-5m));
    }
}