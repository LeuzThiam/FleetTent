namespace FleetRent.Domain.Reservations;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public sealed class Reservation : AggregateRoot
{
    public string NumeroReservation { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid VehiculeId { get; private set; }
    public Guid AgenceDepartId { get; private set; }
    public Guid AgenceRetourId { get; private set; }
    public DateTimeOffset DateDebut { get; private set; }
    public DateTimeOffset DateFin { get; private set; }
    public decimal MontantEstime { get; private set; }
    public decimal CautionRequise { get; private set; }
    public StatutReservation Statut { get; private set; }
    public DateTimeOffset CreeLe { get; private set; }
    public DateTimeOffset? ConfirmeeLe { get; private set; }
    public DateTimeOffset? AnnuleeLe { get; private set; }
    public string? MotifAnnulation { get; private set; }

    public Reservation(
        Guid id,
        string numeroReservation,
        Guid clientId,
        Guid vehiculeId,
        Guid agenceDepartId,
        Guid agenceRetourId,
        DateTimeOffset dateDebut,
        DateTimeOffset dateFin,
        decimal montantEstime,
        decimal cautionRequise)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(numeroReservation))
        {
            throw new ArgumentException("Le numéro de réservation est obligatoire.", nameof(numeroReservation));
        }

        if (clientId == Guid.Empty)
        {
            throw new ArgumentException("Le client est obligatoire.", nameof(clientId));
        }

        if (vehiculeId == Guid.Empty)
        {
            throw new ArgumentException("Le véhicule est obligatoire.", nameof(vehiculeId));
        }

        if (agenceDepartId == Guid.Empty)
        {
            throw new ArgumentException("L'agence de départ est obligatoire.", nameof(agenceDepartId));
        }

        if (agenceRetourId == Guid.Empty)
        {
            throw new ArgumentException("L'agence de retour est obligatoire.", nameof(agenceRetourId));
        }

        if (dateDebut >= dateFin)
        {
            throw new ArgumentException("La date de début doit être antérieure à la date de fin.", nameof(dateDebut));
        }

        if (montantEstime <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montantEstime), "Le montant estimé doit être positif.");
        }

        if (cautionRequise < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cautionRequise), "La caution requise ne peut pas être négative.");
        }

        NumeroReservation = numeroReservation;
        ClientId = clientId;
        VehiculeId = vehiculeId;
        AgenceDepartId = agenceDepartId;
        AgenceRetourId = agenceRetourId;
        DateDebut = dateDebut;
        DateFin = dateFin;
        MontantEstime = montantEstime;
        CautionRequise = cautionRequise;
        Statut = StatutReservation.EnAttente;
        CreeLe = DateTimeOffset.UtcNow;
    }

    public void Confirmer()
    {
        if (Statut != StatutReservation.EnAttente)
        {
            throw new DomainException($"Impossible de confirmer une réservation au statut {Statut}.");
        }

        Statut = StatutReservation.Confirmee;
        ConfirmeeLe = DateTimeOffset.UtcNow;
    }

    public void ChangerPeriode(DateTimeOffset dateDebut, DateTimeOffset dateFin)
    {
        if (Statut != StatutReservation.EnAttente)
        {
            throw new DomainException($"Impossible de modifier la période d'une réservation au statut {Statut}.");
        }

        if (dateDebut >= dateFin)
        {
            throw new ArgumentException("La date de début doit être antérieure à la date de fin.", nameof(dateDebut));
        }

        DateDebut = dateDebut;
        DateFin = dateFin;
    }

    public void ChangerVehicule(Guid vehiculeId)
    {
        if (Statut != StatutReservation.EnAttente)
        {
            throw new DomainException($"Impossible de changer le véhicule d'une réservation au statut {Statut}.");
        }

        if (vehiculeId == Guid.Empty)
        {
            throw new ArgumentException("Le véhicule est obligatoire.", nameof(vehiculeId));
        }

        VehiculeId = vehiculeId;
    }

    public void Annuler(string motif)
    {
        if (Statut is not (StatutReservation.EnAttente or StatutReservation.Confirmee))
        {
            throw new DomainException($"Impossible d'annuler une réservation au statut {Statut}.");
        }

        Statut = StatutReservation.Annulee;
        AnnuleeLe = DateTimeOffset.UtcNow;
        MotifAnnulation = motif;
    }

    public void Expirer()
    {
        if (Statut != StatutReservation.EnAttente)
        {
            throw new DomainException($"Impossible d'expirer une réservation au statut {Statut}.");
        }

        Statut = StatutReservation.Expiree;
    }

    public void ConvertirEnLocation()
    {
        if (Statut != StatutReservation.Confirmee)
        {
            throw new DomainException($"Impossible de convertir en location une réservation au statut {Statut}.");
        }

        Statut = StatutReservation.ConvertieEnLocation;
    }

    public int CalculerDuree() => (DateFin - DateDebut).Days;

    public bool ChevaucheAvec(DateTimeOffset dateDebut, DateTimeOffset dateFin) =>
        DateDebut < dateFin && DateFin > dateDebut;
}