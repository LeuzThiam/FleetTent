namespace FleetRent.Domain.Paiements;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public sealed class Paiement : AggregateRoot
{
    public Guid ContratLocationId { get; private set; }
    public decimal Montant { get; private set; }
    public TypePaiement Type { get; private set; }
    public string MethodePaiement { get; private set; }
    public StatutPaiement Statut { get; private set; }
    public DateTimeOffset DateTransaction { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public string? MotifAnnulation { get; private set; }

    public Paiement(
        Guid id,
        Guid contratLocationId,
        decimal montant,
        TypePaiement type,
        string methodePaiement,
        string? reference,
        string? notes)
        : base(id)
    {
        if (contratLocationId == Guid.Empty)
        {
            throw new ArgumentException("Le contrat de location est obligatoire.", nameof(contratLocationId));
        }

        if (montant <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montant), "Le montant doit être positif.");
        }

        if (string.IsNullOrWhiteSpace(methodePaiement))
        {
            throw new ArgumentException("La méthode de paiement est obligatoire.", nameof(methodePaiement));
        }

        ContratLocationId = contratLocationId;
        Montant = montant;
        Type = type;
        MethodePaiement = methodePaiement;
        Reference = reference;
        Notes = notes;
        Statut = StatutPaiement.Confirme;
        DateTransaction = DateTimeOffset.UtcNow;
    }

    public void Annuler(string motif)
    {
        if (Statut != StatutPaiement.Confirme)
        {
            throw new DomainException($"Impossible d'annuler un paiement au statut {Statut}.");
        }

        Statut = StatutPaiement.Annule;
        MotifAnnulation = motif;
    }
}