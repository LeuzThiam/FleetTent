namespace FleetRent.Domain.Tests.Clients;

using FleetRent.Domain.Clients;

public class PermisConduireTests
{
    private static PermisConduire CreerPermis(DateTimeOffset? dateExpiration = null) =>
        new(
            "P123456789",
            "Québec",
            "B",
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
            dateExpiration ?? new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));

    [Fact]
    public void Constructeur_AvecNumeroVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new PermisConduire(
            "", "Québec", "B", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5)));
    }

    [Fact]
    public void Constructeur_AvecDateExpirationAnterieureADateEmission_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new PermisConduire(
            "P123", "Québec", "B", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(-1)));
    }

    [Fact]
    public void EstValideLe_AvantExpiration_RetourneVrai()
    {
        var permis = CreerPermis(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.True(permis.EstValideLe(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void EstValideLe_ApresExpiration_RetourneFaux()
    {
        var permis = CreerPermis(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.False(permis.EstValideLe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void PermetCategorie_AvecMemeClasse_RetourneVrai()
    {
        var permis = CreerPermis();

        Assert.True(permis.PermetCategorie("B"));
    }

    [Fact]
    public void PermetCategorie_AvecClasseDifferente_RetourneFaux()
    {
        var permis = CreerPermis();

        Assert.False(permis.PermetCategorie("C"));
    }

    [Fact]
    public void Equals_AvecMemesComposants_RetourneVrai()
    {
        var date1 = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var date2 = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var permis1 = new PermisConduire("P123", "Québec", "B", date1, date2);
        var permis2 = new PermisConduire("P123", "Québec", "B", date1, date2);

        Assert.Equal(permis1, permis2);
    }
}