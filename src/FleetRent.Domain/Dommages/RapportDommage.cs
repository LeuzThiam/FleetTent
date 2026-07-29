namespace FleetRent.Domain.Dommages;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public sealed class RapportDommage : AggregateRoot
{
    public Guid VehiculeId { get; private set; }
    public Guid ContratLocationId { get; private set; }
    public string Description { get; private set; }
    public string Zone { get; private set; }
    public GraviteDommage Gravite { get; private set; }
    public decimal CoutEstime { get; private set; }
    public decimal? CoutReel { get; private set; }
    public StatutDommage Statut { get; private set; }
    public DateTimeOffset DateSignalement { get; private set; }
    public DateTimeOffset? DateResolution { get; private set; }

    public RapportDommage(
        Guid id,
        Guid vehiculeId,
        Guid contratLocationId,
        string description,
        string zone,
        GraviteDommage gravite,
        decimal coutEstime)
        : base(id)
    {
        if (vehiculeId == Guid.Empty)
        {
            throw new ArgumentException("Le véhicule est obligatoire.", nameof(vehiculeId));
        }

        if (contratLocationId == Guid.Empty)
        {
            throw new ArgumentException("Le contrat de location est obligatoire.", nameof(contratLocationId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La description est obligatoire.", nameof(description));
        }

        if (string.IsNullOrWhiteSpace(zone))
        {
            throw new ArgumentException("La zone endommagée est obligatoire.", nameof(zone));
        }

        if (coutEstime < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coutEstime), "Le coût estimé ne peut pas être négatif.");
        }

        VehiculeId = vehiculeId;
        ContratLocationId = contratLocationId;
        Description = description;
        Zone = zone;
        Gravite = gravite;
        CoutEstime = coutEstime;
        Statut = StatutDommage.Signale;
        DateSignalement = DateTimeOffset.UtcNow;
    }

    public void CommencerReparation()
    {
        if (Statut != StatutDommage.Signale)
        {
            throw new DomainException($"Impossible de commencer la réparation d'un dommage au statut {Statut}.");
        }

        Statut = StatutDommage.EnReparation;
    }

    public void Resoudre(decimal coutReel)
    {
        if (Statut != StatutDommage.EnReparation)
        {
            throw new DomainException($"Impossible de résoudre un dommage au statut {Statut}.");
        }

        if (coutReel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coutReel), "Le coût réel ne peut pas être négatif.");
        }

        CoutReel = coutReel;
        DateResolution = DateTimeOffset.UtcNow;
        Statut = StatutDommage.Resolu;
    }
}
