namespace FleetRent.Domain.Agences;

using FleetRent.Domain.Common;

public sealed class Agence : AggregateRoot
{
    public string Code { get; private set; }
    public string Nom { get; private set; }
    public string Adresse { get; private set; }
    public string Telephone { get; private set; }
    public string Email { get; private set; }
    public string HorairesOuverture { get; private set; }
    public bool EstActive { get; private set; }
    public DateTimeOffset CreeLe { get; private set; }
    public DateTimeOffset? ModifieLe { get; private set; }

    public Agence(
        Guid id,
        string code,
        string nom,
        string adresse,
        string telephone,
        string email,
        string horairesOuverture)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Le code de l'agence est obligatoire.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(nom))
        {
            throw new ArgumentException("Le nom de l'agence est obligatoire.", nameof(nom));
        }

        Code = code;
        Nom = nom;
        Adresse = adresse;
        Telephone = telephone;
        Email = email;
        HorairesOuverture = horairesOuverture;
        EstActive = true;
        CreeLe = DateTimeOffset.UtcNow;
    }

    public void Activer()
    {
        EstActive = true;
        ModifieLe = DateTimeOffset.UtcNow;
    }

    public void Desactiver()
    {
        EstActive = false;
        ModifieLe = DateTimeOffset.UtcNow;
    }

    public void MettreAJourCoordonnees(string adresse, string telephone, string email)
    {
        Adresse = adresse;
        Telephone = telephone;
        Email = email;
        ModifieLe = DateTimeOffset.UtcNow;
    }

    public void MettreAJourHoraires(string horairesOuverture)
    {
        HorairesOuverture = horairesOuverture;
        ModifieLe = DateTimeOffset.UtcNow;
    }
}
