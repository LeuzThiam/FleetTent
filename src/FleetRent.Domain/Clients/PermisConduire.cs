namespace FleetRent.Domain.Clients;

using FleetRent.Domain.Common;

public sealed class PermisConduire : ValueObject
{
    public string Numero { get; }
    public string ProvinceEmission { get; }
    public string ClassePermis { get; }
    public DateTimeOffset DateEmission { get; }
    public DateTimeOffset DateExpiration { get; }

    public PermisConduire(
        string numero,
        string provinceEmission,
        string classePermis,
        DateTimeOffset dateEmission,
        DateTimeOffset dateExpiration)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            throw new ArgumentException("Le numéro de permis est obligatoire.", nameof(numero));
        }

        if (string.IsNullOrWhiteSpace(provinceEmission))
        {
            throw new ArgumentException("La province d'émission est obligatoire.", nameof(provinceEmission));
        }

        if (string.IsNullOrWhiteSpace(classePermis))
        {
            throw new ArgumentException("La classe de permis est obligatoire.", nameof(classePermis));
        }

        if (dateExpiration <= dateEmission)
        {
            throw new ArgumentException("La date d'expiration doit être postérieure à la date d'émission.", nameof(dateExpiration));
        }

        Numero = numero;
        ProvinceEmission = provinceEmission;
        ClassePermis = classePermis;
        DateEmission = dateEmission;
        DateExpiration = dateExpiration;
    }

    public bool EstValideLe(DateTimeOffset date) => date <= DateExpiration;

    public bool PermetCategorie(string classePermisRequise) =>
        string.Equals(ClassePermis, classePermisRequise, StringComparison.OrdinalIgnoreCase);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Numero;
        yield return ProvinceEmission;
        yield return ClassePermis;
        yield return DateEmission;
        yield return DateExpiration;
    }
}