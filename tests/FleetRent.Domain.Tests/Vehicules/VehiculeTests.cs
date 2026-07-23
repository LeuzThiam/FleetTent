namespace FleetRent.Domain.Tests.Vehicules;

using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;
using FleetRent.Domain.Vehicules;

public class VehiculeTests
{
    private static Vehicule CreerVehicule() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "ABC-123",
            "1HGCM82633A123456",
            "Toyota",
            "Corolla",
            2023,
            "Blanc",
            15000,
            45.00m,
            "Essence",
            "Automatique",
            5);

    [Fact]
    public void Constructeur_ParDefaut_EstDisponible()
    {
        var vehicule = CreerVehicule();
        Assert.Equal(StatutVehicule.Disponible, vehicule.Statut);
    }
    [Fact]
    public void Constructeur_AvecImmatriculationVide_LeverArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Vehicule(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "NIV123", "Toyota", "Corolla", 2023, "Blanc", 0, 45m, "Essence", "Automatique", 5));
    }
    [Fact]
    public void Constructeur_AvecKilometrageNegatif_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vehicule(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ABC-123", "NIV123", "Toyota", "Corolla", 2023, "Blanc", -1, 45m, "Essence", "Automatique", 5));
    }

    [Fact]
    public void Constructeur_AvecTarifNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vehicule(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ABC-123", "NIV123", "Toyota", "Corolla", 2023, "Blanc", 0, 0m, "Essence", "Automatique", 5));
    }

    [Fact]
    public void Reserver_DepuisDisponible_ChangeStatutAReserve()
    {
        var vehicule = CreerVehicule();

        vehicule.Reserver();

        Assert.Equal(StatutVehicule.Reserve, vehicule.Statut);
    }

    [Fact]
    public void Reserver_DepuisStatutNonDisponible_LeveDomainException()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();

        Assert.Throws<DomainException>(() => vehicule.Reserver());
    }

    [Fact]
    public void LibererReservation_DepuisReserve_ChangeStatutADisponible()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();

        vehicule.LibererReservation();

        Assert.Equal(StatutVehicule.Disponible, vehicule.Statut);
    }

    [Fact]
    public void LibererReservation_DepuisDisponible_LeveDomainException()
    {
        var vehicule = CreerVehicule();

        Assert.Throws<DomainException>(() => vehicule.LibererReservation());
    }

    [Fact]
    public void DemarrerLocation_DepuisReserve_ChangeStatutALoue()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();

        vehicule.DemarrerLocation();

        Assert.Equal(StatutVehicule.Loue, vehicule.Statut);
    }

    [Fact]
    public void DemarrerLocation_DepuisDisponible_LeveDomainException()
    {
        var vehicule = CreerVehicule();

        Assert.Throws<DomainException>(() => vehicule.DemarrerLocation());
    }

    [Fact]
    public void EnregistrerRetour_DepuisLoue_ChangeStatutADisponible()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();
        vehicule.DemarrerLocation();

        vehicule.EnregistrerRetour();

        Assert.Equal(StatutVehicule.Disponible, vehicule.Statut);
    }

    [Fact]
    public void MarquerCommeEndommage_DepuisLoue_ChangeStatutAEndommage()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();
        vehicule.DemarrerLocation();

        vehicule.MarquerCommeEndommage();

        Assert.Equal(StatutVehicule.Endommage, vehicule.Statut);
    }

    [Fact]
    public void MarquerCommeEndommage_DepuisDisponible_LeveDomainException()
    {
        var vehicule = CreerVehicule();

        Assert.Throws<DomainException>(() => vehicule.MarquerCommeEndommage());
    }

    [Fact]
    public void EnvoyerEnEntretien_DepuisDisponible_ChangeStatutAEnEntretien()
    {
        var vehicule = CreerVehicule();

        vehicule.EnvoyerEnEntretien();

        Assert.Equal(StatutVehicule.EnEntretien, vehicule.Statut);
    }

    [Fact]
    public void EnvoyerEnEntretien_DepuisReserve_LeveDomainException()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();

        Assert.Throws<DomainException>(() => vehicule.EnvoyerEnEntretien());
    }

    [Fact]
    public void RemettreEnService_DepuisEndommage_ChangeStatutADisponible()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();
        vehicule.DemarrerLocation();
        vehicule.MarquerCommeEndommage();

        vehicule.RemettreEnService();

        Assert.Equal(StatutVehicule.Disponible, vehicule.Statut);
    }

    [Fact]
    public void RemettreEnService_DepuisEnEntretien_ChangeStatutADisponible()
    {
        var vehicule = CreerVehicule();
        vehicule.EnvoyerEnEntretien();

        vehicule.RemettreEnService();

        Assert.Equal(StatutVehicule.Disponible, vehicule.Statut);
    }

    [Fact]
    public void Retirer_DepuisDisponible_ChangeStatutARetireEtDefinitDateRetrait()
    {
        var vehicule = CreerVehicule();

        vehicule.Retirer();

        Assert.Equal(StatutVehicule.Retire, vehicule.Statut);
        Assert.NotNull(vehicule.DateRetrait);
    }

    [Fact]
    public void Retirer_DepuisLoue_LeveDomainException()
    {
        var vehicule = CreerVehicule();
        vehicule.Reserver();
        vehicule.DemarrerLocation();

        Assert.Throws<DomainException>(() => vehicule.Retirer());
    }

    [Fact]
    public void MettreAJourKilometrage_AvecValeurSuperieure_MetAJour()
    {
        var vehicule = CreerVehicule();

        vehicule.MettreAJourKilometrage(20000);

        Assert.Equal(20000, vehicule.Kilometrage);
    }

    [Fact]
    public void MettreAJourKilometrage_AvecValeurInferieure_LeveArgumentOutOfRangeException()
    {
        var vehicule = CreerVehicule();

        Assert.Throws<ArgumentOutOfRangeException>(() => vehicule.MettreAJourKilometrage(1000));
    }

    [Fact]
    public void ChangerTarifJournalier_AvecValeurNegativeOuNulle_LeveArgumentOutOfRangeException()
    {
        var vehicule = CreerVehicule();

        Assert.Throws<ArgumentOutOfRangeException>(() => vehicule.ChangerTarifJournalier(0m));
    }
}
