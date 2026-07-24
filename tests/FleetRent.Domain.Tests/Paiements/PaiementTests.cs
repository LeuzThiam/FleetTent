namespace FleetRent.Domain.Tests.Paiements;

using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;
using FleetRent.Domain.Paiements;

public class PaiementTests
{
    private static Paiement CreerPaiement() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            300.00m,
            TypePaiement.Caution,
            "Carte de crédit",
            "REF-0001",
            null);

    [Fact]
    public void Constructeur_ParDefaut_EstConfirme()
    {
        var paiement = CreerPaiement();

        Assert.Equal(StatutPaiement.Confirme, paiement.Statut);
    }

    [Fact]
    public void Constructeur_AvecMontantNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Paiement(
            Guid.NewGuid(), Guid.NewGuid(), 0m, TypePaiement.Caution, "Carte de crédit", null, null));
    }

    [Fact]
    public void Constructeur_AvecMethodePaiementVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Paiement(
            Guid.NewGuid(), Guid.NewGuid(), 100m, TypePaiement.Caution, "", null, null));
    }

    [Fact]
    public void Annuler_DepuisConfirme_ChangeStatutAAnnuleEtDefinitMotif()
    {
        var paiement = CreerPaiement();

        paiement.Annuler("Erreur de saisie");

        Assert.Equal(StatutPaiement.Annule, paiement.Statut);
        Assert.Equal("Erreur de saisie", paiement.MotifAnnulation);
    }

    [Fact]
    public void Annuler_DepuisAnnule_LeveDomainException()
    {
        var paiement = CreerPaiement();
        paiement.Annuler("Erreur de saisie");

        Assert.Throws<DomainException>(() => paiement.Annuler("Deuxième tentative"));
    }
}