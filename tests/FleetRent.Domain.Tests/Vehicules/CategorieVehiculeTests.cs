namespace FleetRent.Domain.Tests.Vehicules;

using FleetRent.Domain.Vehicules;

public class CategorieVehiculeTests
{
    private static CategorieVehicule CreerCategorie() =>
        new(
            Guid.NewGuid(),
            "ECO",
            "Économique",
            "Petite voiture économique",
            21,
            "B",
            45.00m,
            300.00m,
            200);

    [Fact]
    public void Constructeur_AvecCodeVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CategorieVehicule(
            Guid.NewGuid(), "", "Nom", "Desc", 21, "B", 45m, 300m, 200));
    }

    [Fact]
    public void Constructeur_AvecNomVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CategorieVehicule(
            Guid.NewGuid(), "ECO", "", "Desc", 21, "B", 45m, 300m, 200));
    }

    [Fact]
    public void Constructeur_AvecClasseDePermisVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CategorieVehicule(
            Guid.NewGuid(), "ECO", "Nom", "Desc", 21, "", 45m, 300m, 200));
    }

    [Fact]
    public void Constructeur_AvecAgeMinimumNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CategorieVehicule(
            Guid.NewGuid(), "ECO", "Nom", "Desc", 0, "B", 45m, 300m, 200));
    }

    [Fact]
    public void Constructeur_AvecTarifNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CategorieVehicule(
            Guid.NewGuid(), "ECO", "Nom", "Desc", 21, "B", 0m, 300m, 200));
    }

    [Fact]
    public void Constructeur_AvecCautionNegative_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CategorieVehicule(
            Guid.NewGuid(), "ECO", "Nom", "Desc", 21, "B", 45m, -1m, 200));
    }

    [Fact]
    public void Constructeur_AvecCautionNulle_NeLeveRienDuTout()
    {
        var categorie = new CategorieVehicule(
            Guid.NewGuid(), "ECO", "Nom", "Desc", 21, "B", 45m, 0m, 200);

        Assert.Equal(0m, categorie.CautionDefaut);
    }

    [Fact]
    public void Constructeur_ParDefaut_EstActive()
    {
        var categorie = CreerCategorie();

        Assert.True(categorie.EstActive);
    }

    [Fact]
    public void MettreAJourTarification_MetAJourLesTarifs()
    {
        var categorie = CreerCategorie();

        categorie.MettreAJourTarification(55.00m, 350.00m, 250);

        Assert.Equal(55.00m, categorie.TarifJournalierDefaut);
        Assert.Equal(350.00m, categorie.CautionDefaut);
        Assert.Equal(250, categorie.KilometresInclusParJour);
    }

    [Fact]
    public void MettreAJourTarification_AvecTarifNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        var categorie = CreerCategorie();

        Assert.Throws<ArgumentOutOfRangeException>(() => categorie.MettreAJourTarification(0m, 300m, 200));
    }

    [Fact]
    public void Desactiver_MetEstActiveAFaux()
    {
        var categorie = CreerCategorie();

        categorie.Desactiver();

        Assert.False(categorie.EstActive);
    }

    [Fact]
    public void Activer_MetEstActiveAVrai()
    {
        var categorie = CreerCategorie();
        categorie.Desactiver();

        categorie.Activer();

        Assert.True(categorie.EstActive);
    }
}
