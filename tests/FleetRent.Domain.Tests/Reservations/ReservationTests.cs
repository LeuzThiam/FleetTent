namespace FleetRent.Domain.Tests.Reservations;

using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;
using FleetRent.Domain.Reservations;

public class ReservationTests
{
    private static readonly DateTimeOffset Debut = new(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Fin = new(2026, 8, 5, 0, 0, 0, TimeSpan.Zero);

    private static Reservation CreerReservation() =>
        new(
            Guid.NewGuid(),
            "RES0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Debut,
            Fin,
            250.00m,
            300.00m);

    [Fact]
    public void Constructeur_ParDefaut_EstEnAttente()
    {
        var reservation = CreerReservation();

        Assert.Equal(StatutReservation.EnAttente, reservation.Statut);
    }

    [Fact]
    public void Constructeur_AvecDateDebutPosterieureOuEgaleADateFin_LeveArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Reservation(
            Guid.NewGuid(), "RES0001", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Fin, Debut, 250m, 300m));
    }

    [Fact]
    public void Constructeur_AvecMontantNegatifOuNul_LeveArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Reservation(
            Guid.NewGuid(), "RES0001", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Debut, Fin, 0m, 300m));
    }

    [Fact]
    public void Confirmer_DepuisEnAttente_ChangeStatutAConfirmee()
    {
        var reservation = CreerReservation();

        reservation.Confirmer();

        Assert.Equal(StatutReservation.Confirmee, reservation.Statut);
        Assert.NotNull(reservation.ConfirmeeLe);
    }

    [Fact]
    public void Confirmer_DepuisStatutNonEnAttente_LeveDomainException()
    {
        var reservation = CreerReservation();
        reservation.Confirmer();

        Assert.Throws<DomainException>(() => reservation.Confirmer());
    }

    [Fact]
    public void ChangerPeriode_DepuisEnAttente_MetAJourLesDates()
    {
        var reservation = CreerReservation();
        var nouveauDebut = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero);
        var nouvelleFin = new DateTimeOffset(2026, 9, 5, 0, 0, 0, TimeSpan.Zero);

        reservation.ChangerPeriode(nouveauDebut, nouvelleFin);

        Assert.Equal(nouveauDebut, reservation.DateDebut);
        Assert.Equal(nouvelleFin, reservation.DateFin);
    }

    [Fact]
    public void ChangerPeriode_ApresConfirmation_LeveDomainException()
    {
        var reservation = CreerReservation();
        reservation.Confirmer();

        Assert.Throws<DomainException>(() => reservation.ChangerPeriode(Debut, Fin));
    }

    [Fact]
    public void ChangerVehicule_DepuisEnAttente_MetAJourVehiculeId()
    {
        var reservation = CreerReservation();
        var nouveauVehiculeId = Guid.NewGuid();

        reservation.ChangerVehicule(nouveauVehiculeId);

        Assert.Equal(nouveauVehiculeId, reservation.VehiculeId);
    }

    [Fact]
    public void Annuler_DepuisEnAttente_ChangeStatutAAnnuleeEtDefinitMotif()
    {
        var reservation = CreerReservation();

        reservation.Annuler("Changement de plan du client");

        Assert.Equal(StatutReservation.Annulee, reservation.Statut);
        Assert.Equal("Changement de plan du client", reservation.MotifAnnulation);
        Assert.NotNull(reservation.AnnuleeLe);
    }

    [Fact]
    public void Annuler_DepuisConfirmee_ChangeStatutAAnnulee()
    {
        var reservation = CreerReservation();
        reservation.Confirmer();

        reservation.Annuler("Véhicule indisponible");

        Assert.Equal(StatutReservation.Annulee, reservation.Statut);
    }

    [Fact]
    public void Expirer_DepuisEnAttente_ChangeStatutAExpiree()
    {
        var reservation = CreerReservation();

        reservation.Expirer();

        Assert.Equal(StatutReservation.Expiree, reservation.Statut);
    }

    [Fact]
    public void Expirer_DepuisConfirmee_LeveDomainException()
    {
        var reservation = CreerReservation();
        reservation.Confirmer();

        Assert.Throws<DomainException>(() => reservation.Expirer());
    }

    [Fact]
    public void ConvertirEnLocation_DepuisConfirmee_ChangeStatutAConvertieEnLocation()
    {
        var reservation = CreerReservation();
        reservation.Confirmer();

        reservation.ConvertirEnLocation();

        Assert.Equal(StatutReservation.ConvertieEnLocation, reservation.Statut);
    }

    [Fact]
    public void ConvertirEnLocation_DepuisEnAttente_LeveDomainException()
    {
        var reservation = CreerReservation();

        Assert.Throws<DomainException>(() => reservation.ConvertirEnLocation());
    }

    [Fact]
    public void CalculerDuree_RetourneLeNombreDeJours()
    {
        var reservation = CreerReservation();

        Assert.Equal(4, reservation.CalculerDuree());
    }

    [Fact]
    public void ChevaucheAvec_PeriodesQuiSeChevauchent_RetourneVrai()
    {
        var reservation = CreerReservation();

        var chevauche = reservation.ChevaucheAvec(
            new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 7, 0, 0, 0, TimeSpan.Zero));

        Assert.True(chevauche);
    }

    [Fact]
    public void ChevaucheAvec_PeriodesQuiNeSeChevauchentPas_RetourneFaux()
    {
        var reservation = CreerReservation();

        var chevauche = reservation.ChevaucheAvec(
            new DateTimeOffset(2026, 8, 10, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 15, 0, 0, 0, TimeSpan.Zero));

        Assert.False(chevauche);
    }
}