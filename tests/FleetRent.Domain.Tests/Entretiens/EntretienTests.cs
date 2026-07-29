namespace FleetRent.Domain.Tests.Entretiens;

using FleetRent.Domain.Entretiens;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public class EntretienTests
{
    private static readonly DateTimeOffset DateProgrammee = new(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);

    private static Entretien CreerEntretien() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TypeEntretien.Preventif,
            "Vidange et contrôle général",
            DateProgrammee,
            150.00m,
            "Garage Central");

    [Fact]
    public void Constructeur_ParDefaut_EstPlanifie()
    {
        var entretien = CreerEntretien();

        Assert.Equal(StatutEntretien.Planifie, entretien.Statut);
    }

    [Fact]
    public void Constructeur_AvecCoutEstimeNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Entretien(
            Guid.NewGuid(), Guid.NewGuid(), TypeEntretien.Preventif, "Vidange", DateProgrammee, 0m, "Garage Central"));
    }

    [Fact]
    public void Constructeur_AvecPrestataireVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Entretien(
            Guid.NewGuid(), Guid.NewGuid(), TypeEntretien.Preventif, "Vidange", DateProgrammee, 100m, ""));
    }

    [Fact]
    public void Demarrer_DepuisPlanifie_ChangeStatutAEnCoursEtEnregistreLeKilometrage()
    {
        var entretien = CreerEntretien();

        entretien.Demarrer(15000);

        Assert.Equal(StatutEntretien.EnCours, entretien.Statut);
        Assert.Equal(15000, entretien.Kilometrage);
        Assert.NotNull(entretien.DateDebut);
    }

    [Fact]
    public void Demarrer_DepuisStatutNonPlanifie_LeveDomainException()
    {
        var entretien = CreerEntretien();
        entretien.Demarrer(15000);

        Assert.Throws<DomainException>(() => entretien.Demarrer(15500));
    }

    [Fact]
    public void Demarrer_AvecKilometrageNegatif_LeveArgumentOutOfRangeException()
    {
        var entretien = CreerEntretien();

        Assert.Throws<ArgumentOutOfRangeException>(() => entretien.Demarrer(-1));
    }

    [Fact]
    public void Terminer_DepuisEnCours_ChangeStatutATermineEtEnregistreLeCoutReel()
    {
        var entretien = CreerEntretien();
        entretien.Demarrer(15000);

        entretien.Terminer(180.00m);

        Assert.Equal(StatutEntretien.Termine, entretien.Statut);
        Assert.Equal(180.00m, entretien.CoutReel);
        Assert.NotNull(entretien.DateFin);
    }

    [Fact]
    public void Terminer_DepuisStatutNonEnCours_LeveDomainException()
    {
        var entretien = CreerEntretien();

        Assert.Throws<DomainException>(() => entretien.Terminer(180.00m));
    }

    [Fact]
    public void Terminer_DepuisTermine_LeveDomainException()
    {
        var entretien = CreerEntretien();
        entretien.Demarrer(15000);
        entretien.Terminer(180.00m);

        Assert.Throws<DomainException>(() => entretien.Terminer(200.00m));
    }

    [Fact]
    public void Annuler_DepuisPlanifie_ChangeStatutAAnnule()
    {
        var entretien = CreerEntretien();

        entretien.Annuler();

        Assert.Equal(StatutEntretien.Annule, entretien.Statut);
    }

    [Fact]
    public void Annuler_DepuisEnCours_LeveDomainException()
    {
        var entretien = CreerEntretien();
        entretien.Demarrer(15000);

        Assert.Throws<DomainException>(() => entretien.Annuler());
    }
}