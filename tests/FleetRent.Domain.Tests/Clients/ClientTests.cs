namespace FleetRent.Domain.Tests.Clients;

using FleetRent.Domain.Clients;
using FleetRent.Domain.Enums;

public class ClientTests
{
    private static readonly DateTimeOffset DateReference = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static Client CreerClient(DateTimeOffset? dateNaissance = null) =>
        new(
            Guid.NewGuid(),
            "CL0001",
            "Jean",
            "Tremblay",
            dateNaissance ?? new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
            "jean.tremblay@example.com",
            "514-555-0100",
            "1 rue des Érables, Montréal, QC");

    private static PermisConduire CreerPermisValide() =>
        new(
            "P123456789",
            "Québec",
            "B",
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));

    [Fact]
    public void Constructeur_ParDefaut_EstActif()
    {
        var client = CreerClient();

        Assert.Equal(StatutClient.Actif, client.Statut);
    }

    [Fact]
    public void Constructeur_AvecPrenomVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Client(
            Guid.NewGuid(), "CL0001", "", "Tremblay", new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), "a@b.com", "514-555-0100", "Adresse"));
    }

    [Fact]
    public void Constructeur_AvecDateNaissanceDansLeFutur_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Client(
            Guid.NewGuid(), "CL0001", "Jean", "Tremblay", DateTimeOffset.UtcNow.AddYears(1), "a@b.com", "514-555-0100", "Adresse"));
    }

    [Fact]
    public void Suspendre_MetStatutASuspendu()
    {
        var client = CreerClient();

        client.Suspendre();

        Assert.Equal(StatutClient.Suspendu, client.Statut);
    }

    [Fact]
    public void Reactiver_MetStatutAActif()
    {
        var client = CreerClient();
        client.Suspendre();

        client.Reactiver();

        Assert.Equal(StatutClient.Actif, client.Statut);
    }

    [Fact]
    public void Bloquer_MetStatutABloque()
    {
        var client = CreerClient();

        client.Bloquer();

        Assert.Equal(StatutClient.Bloque, client.Statut);
    }

    [Fact]
    public void Archiver_MetStatutAArchive()
    {
        var client = CreerClient();

        client.Archiver();

        Assert.Equal(StatutClient.Archive, client.Statut);
    }

    [Fact]
    public void VerifierEligibilite_SansPermis_RetourneFaux()
    {
        var client = CreerClient();

        var eligible = client.VerifierEligibilite(DateReference, "B", 21);

        Assert.False(eligible);
    }

    [Fact]
    public void VerifierEligibilite_AvecPermisValideEtClientActif_RetourneVrai()
    {
        var client = CreerClient();
        client.MettreAJourPermis(CreerPermisValide());

        var eligible = client.VerifierEligibilite(DateReference, "B", 21);

        Assert.True(eligible);
    }

    [Fact]
    public void VerifierEligibilite_ClientSuspendu_RetourneFaux()
    {
        var client = CreerClient();
        client.MettreAJourPermis(CreerPermisValide());
        client.Suspendre();

        var eligible = client.VerifierEligibilite(DateReference, "B", 21);

        Assert.False(eligible);
    }

    [Fact]
    public void VerifierEligibilite_PermisExpire_RetourneFaux()
    {
        var client = CreerClient();
        var permisExpire = new PermisConduire(
            "P123", "Québec", "B",
            new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
        client.MettreAJourPermis(permisExpire);

        var eligible = client.VerifierEligibilite(DateReference, "B", 21);

        Assert.False(eligible);
    }

    [Fact]
    public void VerifierEligibilite_AgeInsuffisant_RetourneFaux()
    {
        var client = CreerClient(new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero));
        client.MettreAJourPermis(CreerPermisValide());

        var eligible = client.VerifierEligibilite(DateReference, "B", 21);

        Assert.False(eligible);
    }

    [Fact]
    public void VerifierEligibilite_ClassePermisIncompatible_RetourneFaux()
    {
        var client = CreerClient();
        client.MettreAJourPermis(CreerPermisValide());

        var eligible = client.VerifierEligibilite(DateReference, "C", 21);

        Assert.False(eligible);
    }
}