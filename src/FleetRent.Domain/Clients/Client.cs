namespace FleetRent.Domain.Clients;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;

public sealed class Client : AggregateRoot
{
    public string NumeroClient { get; private set; }
    public string Prenom { get; private set; }
    public string Nom { get; private set; }
    public DateTimeOffset DateNaissance { get; private set; }
    public string Email { get; private set; }
    public string Telephone { get; private set; }
    public string Adresse { get; private set; }
    public StatutClient Statut { get; private set; }
    public DateTimeOffset DateInscription { get; private set; }
    public string? Notes { get; private set; }
    public PermisConduire? Permis { get; private set; }

    public Client(
        Guid id,
        string numeroClient,
        string prenom,
        string nom,
        DateTimeOffset dateNaissance,
        string email,
        string telephone,
        string adresse)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(numeroClient))
        {
            throw new ArgumentException("Le numéro de client est obligatoire.", nameof(numeroClient));
        }

        if (string.IsNullOrWhiteSpace(prenom))
        {
            throw new ArgumentException("Le prénom est obligatoire.", nameof(prenom));
        }

        if (string.IsNullOrWhiteSpace(nom))
        {
            throw new ArgumentException("Le nom est obligatoire.", nameof(nom));
        }

        if (dateNaissance >= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("La date de naissance doit être dans le passé.", nameof(dateNaissance));
        }

        NumeroClient = numeroClient;
        Prenom = prenom;
        Nom = nom;
        DateNaissance = dateNaissance;
        Email = email;
        Telephone = telephone;
        Adresse = adresse;
        Statut = StatutClient.Actif;
        DateInscription = DateTimeOffset.UtcNow;
    }

    public void MettreAJourCoordonnees(string email, string telephone)
    {
        Email = email;
        Telephone = telephone;
    }

    public void MettreAJourAdresse(string adresse)
    {
        Adresse = adresse;
    }

    public void MettreAJourNotes(string? notes)
    {
        Notes = notes;
    }

    public void MettreAJourPermis(PermisConduire permis)
    {
        Permis = permis;
    }

    public void Suspendre()
    {
        Statut = StatutClient.Suspendu;
    }

    public void Reactiver()
    {
        Statut = StatutClient.Actif;
    }

    public void Bloquer()
    {
        Statut = StatutClient.Bloque;
    }

    public void Archiver()
    {
        Statut = StatutClient.Archive;
    }

    public bool VerifierEligibilite(DateTimeOffset date, string classePermisRequise, int ageMinimum)
    {
        if (Statut != StatutClient.Actif)
        {
            return false;
        }

        if (Permis is null || !Permis.EstValideLe(date) || !Permis.PermetCategorie(classePermisRequise))
        {
            return false;
        }

        return CalculerAge(date) >= ageMinimum;
    }

    private int CalculerAge(DateTimeOffset dateReference)
    {
        var age = dateReference.Year - DateNaissance.Year;
        if (dateReference.Month < DateNaissance.Month ||
            (dateReference.Month == DateNaissance.Month && dateReference.Day < DateNaissance.Day))
        {
            age--;
        }

        return age;
    }
}