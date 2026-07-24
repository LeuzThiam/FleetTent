namespace FleetRent.Domain.Tests.Locations;

using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;
using FleetRent.Domain.Locations;

public class ContratLocationTests
{
    private static readonly DateTimeOffset Debut = new(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset RetourPrevu = new(2026, 8, 5, 0, 0, 0, TimeSpan.Zero);

    private static ContratLocation CreerContrat() =>
        new(
            Guid.NewGuid(),
            "CTR0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Debut,
            RetourPrevu,
            10000,
            1.0m,
            200.00m,
            300.00m,
            Guid.NewGuid());

    [Fact]
    public void Constructeur_ParDefaut_EstPrepare()
    {
        var contrat = CreerContrat();

        Assert.Equal(StatutContratLocation.Prepare, contrat.Statut);
    }

    [Fact]
    public void Constructeur_AvecCautionNulleOuNegative_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ContratLocation(
            Guid.NewGuid(), "CTR0001", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Debut, RetourPrevu, 10000, 1.0m, 200m, 0m, Guid.NewGuid()));
    }

    [Fact]
    public void Constructeur_AvecDateDebutPosterieureOuEgaleADateRetourPrevue_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ContratLocation(
            Guid.NewGuid(), "CTR0001", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            RetourPrevu, Debut, 10000, 1.0m, 200m, 300m, Guid.NewGuid()));
    }

    [Fact]
    public void Demarrer_DepuisPrepare_ChangeStatutAActif()
    {
        var contrat = CreerContrat();

        contrat.Demarrer();

        Assert.Equal(StatutContratLocation.Actif, contrat.Statut);
    }

    [Fact]
    public void Demarrer_DepuisStatutNonPrepare_LeveDomainException()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();

        Assert.Throws<DomainException>(() => contrat.Demarrer());
    }

    [Fact]
    public void Prolonger_DepuisActif_MetAJourDateRetourPrevue()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        var nouvelleDate = RetourPrevu.AddDays(2);

        contrat.Prolonger(nouvelleDate);

        Assert.Equal(nouvelleDate, contrat.DateRetourPrevue);
    }

    [Fact]
    public void Prolonger_DepuisPrepare_LeveDomainException()
    {
        var contrat = CreerContrat();

        Assert.Throws<DomainException>(() => contrat.Prolonger(RetourPrevu.AddDays(2)));
    }

    [Fact]
    public void EnregistrerRetour_DepuisActif_ChangeStatutARetourneEtEnregistreLesDonnees()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        var dateRetour = RetourPrevu.AddDays(-1);

        contrat.EnregistrerRetour(dateRetour, 10200, 0.75m);

        Assert.Equal(StatutContratLocation.Retourne, contrat.Statut);
        Assert.Equal(dateRetour, contrat.DateRetourReelle);
        Assert.Equal(10200, contrat.KilometrageFinal);
        Assert.Equal(0.75m, contrat.NiveauCarburantFinal);
    }

    [Fact]
    public void EnregistrerRetour_AvecKilometrageFinalInferieur_LeveArgumentOutOfRangeException()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();

        Assert.Throws<ArgumentOutOfRangeException>(() => contrat.EnregistrerRetour(RetourPrevu, 5000, 1.0m));
    }

    [Fact]
    public void CalculerKilometresParcourus_ApresRetour_RetourneLaDifference()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        contrat.EnregistrerRetour(RetourPrevu, 10500, 1.0m);

        Assert.Equal(500, contrat.CalculerKilometresParcourus());
    }

    [Fact]
    public void CalculerKilometresParcourus_AvantRetour_LeveDomainException()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();

        Assert.Throws<DomainException>(() => contrat.CalculerKilometresParcourus());
    }

    [Fact]
    public void CalculerDureeRetard_RetourALHeure_RetourneZero()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        contrat.EnregistrerRetour(RetourPrevu.AddDays(-1), 10200, 1.0m);

        Assert.Equal(TimeSpan.Zero, contrat.CalculerDureeRetard());
    }

    [Fact]
    public void CalculerDureeRetard_RetourEnRetard_RetourneLaDuree()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        var dateRetour = RetourPrevu.AddHours(5);

        contrat.EnregistrerRetour(dateRetour, 10200, 1.0m);

        Assert.Equal(TimeSpan.FromHours(5), contrat.CalculerDureeRetard());
    }

    [Fact]
    public void AjouterFraisSupplementaire_AugmenteLeMontant()
    {
        var contrat = CreerContrat();

        contrat.AjouterFraisSupplementaire(50m);
        contrat.AjouterFraisSupplementaire(25m);

        Assert.Equal(75m, contrat.MontantFraisSupplementaires);
    }

    [Fact]
    public void AjouterFraisSupplementaire_AvecMontantNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        var contrat = CreerContrat();

        Assert.Throws<ArgumentOutOfRangeException>(() => contrat.AjouterFraisSupplementaire(0m));
    }

    [Fact]
    public void Cloturer_DepuisRetourne_ChangeStatutAClotureEtCalculeMontantFinal()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();
        contrat.EnregistrerRetour(RetourPrevu, 10200, 1.0m);
        contrat.AjouterFraisSupplementaire(50m);
        var utilisateurId = Guid.NewGuid();

        contrat.Cloturer(utilisateurId);

        Assert.Equal(StatutContratLocation.Cloture, contrat.Statut);
        Assert.Equal(250.00m, contrat.MontantFinal);
        Assert.Equal(utilisateurId, contrat.ClotureParUtilisateurId);
    }

    [Fact]
    public void Cloturer_DepuisStatutNonRetourne_LeveDomainException()
    {
        var contrat = CreerContrat();

        Assert.Throws<DomainException>(() => contrat.Cloturer(Guid.NewGuid()));
    }

    [Fact]
    public void Annuler_DepuisPrepare_ChangeStatutAAnnule()
    {
        var contrat = CreerContrat();

        contrat.Annuler();

        Assert.Equal(StatutContratLocation.Annule, contrat.Statut);
    }

    [Fact]
    public void Annuler_DepuisActif_LeveDomainException()
    {
        var contrat = CreerContrat();
        contrat.Demarrer();

        Assert.Throws<DomainException>(() => contrat.Annuler());
    }
}