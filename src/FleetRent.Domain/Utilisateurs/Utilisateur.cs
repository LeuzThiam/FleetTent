namespace FleetRent.Domain.Utilisateurs;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public sealed class Utilisateur : AggregateRoot
{
    public string NomUtilisateur { get; private set; }
    public string MotDePasseHash { get; private set; }
    public string Prenom { get; private set; }
    public string Nom { get; private set; }
    public string Email { get; private set; }
    public RoleUtilisateur Role { get; private set; }
    public Guid? AgenceId { get; private set; }
    public bool EstActif { get; private set; }
    public int TentativesConnexionEchouees { get; private set; }
    public DateTimeOffset? VerrouilleJusqua { get; private set; }
    public DateTimeOffset? DerniereConnexionLe { get; private set; }
    public DateTimeOffset CreeLe { get; private set; }

    public Utilisateur(
        Guid id,
        string nomUtilisateur,
        string motDePasseHash,
        string prenom,
        string nom,
        string email,
        RoleUtilisateur role,
        Guid? agenceId)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(nomUtilisateur))
        {
            throw new ArgumentException("Le nom d'utilisateur est obligatoire.", nameof(nomUtilisateur));
        }

        if (string.IsNullOrWhiteSpace(motDePasseHash))
        {
            throw new ArgumentException("Le mot de passe est obligatoire.", nameof(motDePasseHash));
        }

        if (string.IsNullOrWhiteSpace(prenom))
        {
            throw new ArgumentException("Le prénom est obligatoire.", nameof(prenom));
        }

        if (string.IsNullOrWhiteSpace(nom))
        {
            throw new ArgumentException("Le nom est obligatoire.", nameof(nom));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("L'email est obligatoire.", nameof(email));
        }

        NomUtilisateur = nomUtilisateur;
        MotDePasseHash = motDePasseHash;
        Prenom = prenom;
        Nom = nom;
        Email = email;
        Role = role;
        AgenceId = agenceId;
        EstActif = true;
        TentativesConnexionEchouees = 0;
        CreeLe = DateTimeOffset.UtcNow;
    }

    public void EnregistrerConnexionReussie()
    {
        DerniereConnexionLe = DateTimeOffset.UtcNow;
        TentativesConnexionEchouees = 0;
        VerrouilleJusqua = null;
    }

    public void EnregistrerEchecConnexion(int tentativesMaximales, TimeSpan dureeVerrouillage)
    {
        if (tentativesMaximales <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tentativesMaximales), "Le nombre maximal de tentatives doit être positif.");
        }

        TentativesConnexionEchouees++;

        if (TentativesConnexionEchouees >= tentativesMaximales)
        {
            VerrouilleJusqua = DateTimeOffset.UtcNow.Add(dureeVerrouillage);
        }
    }

    public bool EstVerrouille(DateTimeOffset dateReference)
    {
        return VerrouilleJusqua is not null && VerrouilleJusqua.Value > dateReference;
    }

    public void ChangerMotDePasse(string nouveauMotDePasseHash)
    {
        if (string.IsNullOrWhiteSpace(nouveauMotDePasseHash))
        {
            throw new ArgumentException("Le mot de passe est obligatoire.", nameof(nouveauMotDePasseHash));
        }

        MotDePasseHash = nouveauMotDePasseHash;
    }

    public void ChangerRole(RoleUtilisateur nouveauRole)
    {
        Role = nouveauRole;
    }

    public void AffecterAgence(Guid? agenceId)
    {
        AgenceId = agenceId;
    }

    public void Desactiver()
    {
        if (!EstActif)
        {
            throw new DomainException("Le compte est déjà désactivé.");
        }

        EstActif = false;
    }

    public void Reactiver()
    {
        if (EstActif)
        {
            throw new DomainException("Le compte est déjà actif.");
        }

        EstActif = true;
        TentativesConnexionEchouees = 0;
        VerrouilleJusqua = null;
    }
}
