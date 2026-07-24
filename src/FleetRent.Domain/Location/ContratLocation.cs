namespace FleetRent.Domain.Locations;

using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

public sealed class ContratLocation : AggregateRoot
{
    public string NumeroContrat { get; private set; }
    public Guid ReservationId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid VehiculeId { get; private set; }
    public DateTimeOffset DateDebut { get; private set; }
    public DateTimeOffset DateRetourPrevue { get; private set; }
    public DateTimeOffset? DateRetourReelle { get; private set; }
    public int KilometrageInitial { get; private set; }
    public int? KilometrageFinal { get; private set; }
    public decimal NiveauCarburantInitial { get; private set; }
    public decimal? NiveauCarburantFinal { get; private set; }
    public decimal MontantEstime { get; private set; }
    public decimal? MontantFinal { get; private set; }
    public decimal MontantCaution { get; private set; }
    public decimal MontantFraisSupplementaires { get; private set; }
    public StatutContratLocation Statut { get; private set; }
    public Guid CreeParUtilisateurId { get; private set; }
    public Guid? ClotureParUtilisateurId { get; private set; }

    public ContratLocation(
        Guid id,
        string numeroContrat,
        Guid reservationId,
        Guid clientId,
        Guid vehiculeId,
        DateTimeOffset dateDebut,
        DateTimeOffset dateRetourPrevue,
        int kilometrageInitial,
        decimal niveauCarburantInitial,
        decimal montantEstime,
        decimal montantCaution,
        Guid creeParUtilisateurId)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(numeroContrat))
        {
            throw new ArgumentException("Le numéro de contrat est obligatoire.", nameof(numeroContrat));
        }

        if (reservationId == Guid.Empty)
        {
            throw new ArgumentException("La réservation est obligatoire.", nameof(reservationId));
        }

        if (clientId == Guid.Empty)
        {
            throw new ArgumentException("Le client est obligatoire.", nameof(clientId));
        }

        if (vehiculeId == Guid.Empty)
        {
            throw new ArgumentException("Le véhicule est obligatoire.", nameof(vehiculeId));
        }

        if (creeParUtilisateurId == Guid.Empty)
        {
            throw new ArgumentException("L'utilisateur créateur est obligatoire.", nameof(creeParUtilisateurId));
        }

        if (dateDebut >= dateRetourPrevue)
        {
            throw new ArgumentException("La date de début doit être antérieure à la date de retour prévue.", nameof(dateDebut));
        }

        if (kilometrageInitial < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometrageInitial), "Le kilométrage initial ne peut pas être négatif.");
        }

        if (niveauCarburantInitial is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(niveauCarburantInitial), "Le niveau de carburant doit être compris entre 0 et 1.");
        }

        if (montantEstime <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montantEstime), "Le montant estimé doit être positif.");
        }

        if (montantCaution <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montantCaution), "La caution est obligatoire et doit être positive.");
        }

        NumeroContrat = numeroContrat;
        ReservationId = reservationId;
        ClientId = clientId;
        VehiculeId = vehiculeId;
        DateDebut = dateDebut;
        DateRetourPrevue = dateRetourPrevue;
        KilometrageInitial = kilometrageInitial;
        NiveauCarburantInitial = niveauCarburantInitial;
        MontantEstime = montantEstime;
        MontantCaution = montantCaution;
        CreeParUtilisateurId = creeParUtilisateurId;
        Statut = StatutContratLocation.Prepare;
    }

    public void Demarrer()
    {
        if (Statut != StatutContratLocation.Prepare)
        {
            throw new DomainException($"Impossible de démarrer un contrat au statut {Statut}.");
        }

        Statut = StatutContratLocation.Actif;
    }

    public void Prolonger(DateTimeOffset nouvelleDateRetourPrevue)
    {
        if (Statut != StatutContratLocation.Actif)
        {
            throw new DomainException($"Impossible de prolonger un contrat au statut {Statut}.");
        }

        if (nouvelleDateRetourPrevue <= DateRetourPrevue)
        {
            throw new ArgumentException("La nouvelle date de retour prévue doit être postérieure à l'actuelle.", nameof(nouvelleDateRetourPrevue));
        }

        DateRetourPrevue = nouvelleDateRetourPrevue;
    }

    public void EnregistrerRetour(DateTimeOffset dateRetourReelle, int kilometrageFinal, decimal niveauCarburantFinal)
    {
        if (Statut is not (StatutContratLocation.Actif or StatutContratLocation.EnRetard))
        {
            throw new DomainException($"Impossible d'enregistrer un retour pour un contrat au statut {Statut}.");
        }

        if (kilometrageFinal < KilometrageInitial)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometrageFinal), "Le kilométrage final ne peut pas être inférieur au kilométrage initial.");
        }

        if (niveauCarburantFinal is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(niveauCarburantFinal), "Le niveau de carburant doit être compris entre 0 et 1.");
        }

        DateRetourReelle = dateRetourReelle;
        KilometrageFinal = kilometrageFinal;
        NiveauCarburantFinal = niveauCarburantFinal;
        Statut = StatutContratLocation.Retourne;
    }

    public int CalculerKilometresParcourus()
    {
        if (KilometrageFinal is null)
        {
            throw new DomainException("Le retour n'a pas encore été enregistré.");
        }

        return KilometrageFinal.Value - KilometrageInitial;
    }

    public TimeSpan CalculerDureeRetard()
    {
        var dateComparaison = DateRetourReelle ?? DateTimeOffset.UtcNow;
        var retard = dateComparaison - DateRetourPrevue;
        return retard > TimeSpan.Zero ? retard : TimeSpan.Zero;
    }

    public void AjouterFraisSupplementaire(decimal montant)
    {
        if (montant <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montant), "Le montant du frais supplémentaire doit être positif.");
        }

        MontantFraisSupplementaires += montant;
    }

    public void Cloturer(Guid clotureParUtilisateurId)
    {
        if (Statut != StatutContratLocation.Retourne)
        {
            throw new DomainException($"Impossible de clôturer un contrat au statut {Statut}.");
        }

        if (clotureParUtilisateurId == Guid.Empty)
        {
            throw new ArgumentException("L'utilisateur clôturant est obligatoire.", nameof(clotureParUtilisateurId));
        }

        MontantFinal = MontantEstime + MontantFraisSupplementaires;
        ClotureParUtilisateurId = clotureParUtilisateurId;
        Statut = StatutContratLocation.Cloture;
    }

    public void Annuler()
    {
        if (Statut != StatutContratLocation.Prepare)
        {
            throw new DomainException($"Impossible d'annuler un contrat au statut {Statut}.");
        }

        Statut = StatutContratLocation.Annule;
    }
}