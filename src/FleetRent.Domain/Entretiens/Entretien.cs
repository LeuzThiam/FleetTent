using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

namespace FleetRent.Domain.Entretiens;

public sealed class Entretien : AggregateRoot
{
    public Guid VehiculeId {get; private set; }
    public TypeEntretien Type { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset DateProgrammee { get; private set; }
    public DateTimeOffset? DateDebut { get; private set; }
    public DateTimeOffset? DateFin { get; private set; }
    public int? Kilometrage { get; private set; }
    public decimal CoutEstime { get; private set; }
    public decimal? CoutReel { get; private set; }
    public string Prestataire { get; private set; }
    public StatutEntretien Statut { get; private set; }

    public Entretien(
        Guid id,
        Guid vehiculeId,
        TypeEntretien type,
        string description,
        DateTimeOffset dateProgrammee,
        decimal coutEstime,
        string prestataire)
        : base(id)
    {
        if (vehiculeId == Guid.Empty)
        {
            throw new ArgumentException("Le véhicule est obligatoire.", nameof(vehiculeId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La description est obligatoire.", nameof(description));
        }

        if (coutEstime <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coutEstime), "Le coût estimé doit être positif.");
        }

        if (string.IsNullOrWhiteSpace(prestataire))
        {
            throw new ArgumentException("Le prestataire est obligatoire.", nameof(prestataire));
        }

        VehiculeId = vehiculeId;
        Type = type;
        Description = description;
        DateProgrammee = dateProgrammee;
        CoutEstime = coutEstime;
        Prestataire = prestataire;
        Statut = StatutEntretien.Planifie;
    }

    public void Demarrer(int kilometrage)
    {
        if (Statut != StatutEntretien.Planifie)
        {
            throw new DomainException($"Impossible de démarrer un entretien au statut {Statut}.");
        }

        if (kilometrage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometrage), "Le kilométrage ne peut pas être négatif.");
        }

        Kilometrage = kilometrage;
        DateDebut = DateTimeOffset.UtcNow;
        Statut = StatutEntretien.EnCours;
    }

    public void Terminer(decimal coutReel)
    {
        if (Statut != StatutEntretien.EnCours)
        {
            throw new DomainException($"Impossible de terminer un entretien au statut {Statut}.");
        }

        if (coutReel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coutReel), "Le coût réel doit être positif.");
        }

        CoutReel = coutReel;
        DateFin = DateTimeOffset.UtcNow;
        Statut = StatutEntretien.Termine;
    }

    public void Annuler()
    {
        if (Statut != StatutEntretien.Planifie)
        {
            throw new DomainException($"Impossible d'annuler un entretien au statut {Statut}.");
        }

        Statut = StatutEntretien.Annule;
    }
}

