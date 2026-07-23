namespace FleetRent.Domain.Vehicules;

using FleetRent.Domain.Common;

public sealed class CategorieVehicule : AggregateRoot
{
    public string Code { get; private set; }
    public string Nom { get; private set; }
    public string Description { get; private set; }
    public int AgeMinimumConducteur { get; private set; }
    public string ClassePermisRequise { get; private set; }
    public decimal TarifJournalierDefaut { get; private set; }
    public decimal CautionDefaut { get; private set; }
    public int KilometresInclusParJour { get; private set; }
    public bool EstActive { get; private set; }

    public CategorieVehicule(
        Guid id,
        string code,
        string nom,
        string description,
        int ageMinimumConducteur,
        string classePermisRequise,
        decimal tarifJournalierDefaut,
        decimal cautionDefaut,
        int kilometresInclusParJour)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Le code de la catégorie est obligatoire.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(nom))
        {
            throw new ArgumentException("Le nom de la catégorie est obligatoire.", nameof(nom));
        }

        if (string.IsNullOrWhiteSpace(classePermisRequise))
        {
            throw new ArgumentException("La classe de permis requise est obligatoire.", nameof(classePermisRequise));
        }

        if (ageMinimumConducteur <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ageMinimumConducteur), "L'âge minimum du conducteur doit être positif.");
        }

        if (tarifJournalierDefaut <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tarifJournalierDefaut), "Le tarif journalier par défaut doit être positif.");
        }

        if (cautionDefaut < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cautionDefaut), "La caution par défaut ne peut pas être négative.");
        }

        if (kilometresInclusParJour < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometresInclusParJour), "Le kilométrage inclus ne peut pas être négatif.");
        }

        Code = code;
        Nom = nom;
        Description = description;
        AgeMinimumConducteur = ageMinimumConducteur;
        ClassePermisRequise = classePermisRequise;
        TarifJournalierDefaut = tarifJournalierDefaut;
        CautionDefaut = cautionDefaut;
        KilometresInclusParJour = kilometresInclusParJour;
        EstActive = true;
    }

    public void MettreAJourTarification(decimal tarifJournalierDefaut, decimal cautionDefaut, int kilometresInclusParJour)
    {
        if (tarifJournalierDefaut <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tarifJournalierDefaut), "Le tarif journalier par défaut doit être positif.");
        }

        if (cautionDefaut < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cautionDefaut), "La caution par défaut ne peut pas être négative.");
        }

        if (kilometresInclusParJour < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometresInclusParJour), "Le kilométrage inclus ne peut pas être négatif.");
        }

        TarifJournalierDefaut = tarifJournalierDefaut;
        CautionDefaut = cautionDefaut;
        KilometresInclusParJour = kilometresInclusParJour;
    }

    public void MettreAJourDescription(string description)
    {
        Description = description;
    }

    public void Activer()
    {
        EstActive = true;
    }

    public void Desactiver()
    {
        EstActive = false;
    }
}
