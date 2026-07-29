namespace FleetRent.Domain.Tests.Utilisateurs;

using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;
using FleetRent.Domain.Utilisateurs;

public class UtilisateurTests
{
    private static Utilisateur CreerUtilisateur() =>
        new(
            Guid.NewGuid(),
            "j.dupont",
            "hash-du-mot-de-passe",
            "Jean",
            "Dupont",
            "j.dupont@fleetrent.com",
            RoleUtilisateur.AgentLocation,
            Guid.NewGuid());

    [Fact]
    public void Constructeur_ParDefaut_EstActifEtSansTentativesEchouees()
    {
        var utilisateur = CreerUtilisateur();

        Assert.True(utilisateur.EstActif);
        Assert.Equal(0, utilisateur.TentativesConnexionEchouees);
    }

    [Fact]
    public void Constructeur_AvecNomUtilisateurVide_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Utilisateur(
            Guid.NewGuid(), "", "hash", "Jean", "Dupont", "j.dupont@fleetrent.com", RoleUtilisateur.AgentLocation, null));
    }

    [Fact]
    public void EnregistrerConnexionReussie_ReinitialiseLesTentativesEtDefinitLaDate()
    {
        var utilisateur = CreerUtilisateur();
        utilisateur.EnregistrerEchecConnexion(5, TimeSpan.FromMinutes(15));

        utilisateur.EnregistrerConnexionReussie();

        Assert.Equal(0, utilisateur.TentativesConnexionEchouees);
        Assert.NotNull(utilisateur.DerniereConnexionLe);
    }

    [Fact]
    public void EnregistrerEchecConnexion_AtteintLeMaximum_VerrouilleLeCompte()
    {
        var utilisateur = CreerUtilisateur();

        utilisateur.EnregistrerEchecConnexion(3, TimeSpan.FromMinutes(15));
        utilisateur.EnregistrerEchecConnexion(3, TimeSpan.FromMinutes(15));
        utilisateur.EnregistrerEchecConnexion(3, TimeSpan.FromMinutes(15));

        Assert.True(utilisateur.EstVerrouille(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void EnregistrerEchecConnexion_SousLeMaximum_NeVerrouillePasLeCompte()
    {
        var utilisateur = CreerUtilisateur();

        utilisateur.EnregistrerEchecConnexion(3, TimeSpan.FromMinutes(15));

        Assert.False(utilisateur.EstVerrouille(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ChangerMotDePasse_AvecValeurVide_LeveArgumentException()
    {
        var utilisateur = CreerUtilisateur();

        Assert.Throws<ArgumentException>(() => utilisateur.ChangerMotDePasse(""));
    }

    [Fact]
    public void Desactiver_DepuisActif_ChangeEstActifAFaux()
    {
        var utilisateur = CreerUtilisateur();

        utilisateur.Desactiver();

        Assert.False(utilisateur.EstActif);
    }

    [Fact]
    public void Desactiver_DepuisDejaDesactive_LeveDomainException()
    {
        var utilisateur = CreerUtilisateur();
        utilisateur.Desactiver();

        Assert.Throws<DomainException>(() => utilisateur.Desactiver());
    }

    [Fact]
    public void Reactiver_DepuisDesactive_ChangeEstActifAVrai()
    {
        var utilisateur = CreerUtilisateur();
        utilisateur.Desactiver();

        utilisateur.Reactiver();

        Assert.True(utilisateur.EstActif);
    }

    [Fact]
    public void Reactiver_DepuisDejaActif_LeveDomainException()
    {
        var utilisateur = CreerUtilisateur();

        Assert.Throws<DomainException>(() => utilisateur.Reactiver());
    }
}
