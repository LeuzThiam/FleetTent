using FleetRent.Domain.Common;
using FleetRent.Domain.Enums;
using FleetRent.Domain.Exceptions;

namespace FleetRent.Domain.Vehicules
{
    public sealed class Vehicule : AggregateRoot
    {
        public Guid AgenceId { get; private set; }
        public Guid CategorieId { get; private set; }
        public string Immatriculation { get; private set; }
        public string NIV { get; private set; }
        public string Marque { get; private set; }
        public string Modele { get; private set; }
        public int Annee { get; private set; }
        public string Couleur { get; private set; }
        public int Kilometrage { get; private set; }
        public decimal TarifJournalier { get; private set; }
        public StatutVehicule Statut { get; private set; }
        public string TypeCarburant { get; private set; }
        public string TypeTransmission { get; private set; }
        public int NombrePlaces { get; private set; }
        public DateTimeOffset DateAjout { get; private set; }
        public DateTimeOffset? DateRetrait { get; private set; }
        public byte[] RowVersion { get; private set; } = [];

        public Vehicule(
        Guid id,
        Guid agenceId,
        Guid categorieId,
        string immatriculation,
        string niv,
        string marque,
        string modele,
        int annee,
        string couleur,
        int kilometrage,
        decimal tarifJournalier,
        string typeCarburant,
        string typeTransmission,
        int nombrePlaces)
        : base(id)
        {
            if (agenceId == Guid.Empty)
            {
                throw new ArgumentException("L'agence est obligatoire. ", nameof(agenceId));
            }
            if (categorieId == Guid.Empty)
            {
                throw new ArgumentException("La catégorie est obligatoire.", nameof(categorieId));
            }

            if (string.IsNullOrWhiteSpace(immatriculation))
            {
                throw new ArgumentException("L'immatriculation est obligatoire.", nameof(immatriculation));
            }

            if (string.IsNullOrWhiteSpace(niv))
            {
                throw new ArgumentException("Le NIV est obligatoire.", nameof(niv));
            }

            if (string.IsNullOrWhiteSpace(marque))
            {
                throw new ArgumentException("La marque est obligatoire.", nameof(marque));
            }

            if (string.IsNullOrWhiteSpace(modele))
            {
                throw new ArgumentException("Le modèle est obligatoire.", nameof(modele));
            }

            if (annee <= 1900)
            {
                throw new ArgumentOutOfRangeException(nameof(annee), "L'année doit être valide.");
            }

            if (kilometrage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kilometrage), "Le kilométrage ne peut pas être négatif.");
            }

            if (tarifJournalier <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tarifJournalier), "Le tarif journalier doit être positif.");
            }

            if (nombrePlaces <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nombrePlaces), "Le nombre de places doit être positif.");
            }

            AgenceId = agenceId;
            CategorieId = categorieId;
            Immatriculation = immatriculation;
            NIV = niv;
            Marque = marque;
            Modele = modele;
            Annee = annee;
            Couleur = couleur;
            Kilometrage = kilometrage;
            TarifJournalier = tarifJournalier;
            TypeCarburant = typeCarburant;
            TypeTransmission = typeTransmission;
            NombrePlaces = nombrePlaces;
            Statut = StatutVehicule.Disponible;
            DateAjout = DateTimeOffset.UtcNow;
        }

        public void MettreAJourInformation(string marque, string modele, string couleur, string typeCarburant, string typeTransmission, int nombrePlaces)
        {
            if (string.IsNullOrEmpty(marque))
            {
                throw new ArgumentException("La marque est obligatoire.", nameof(marque));
            }
            if (string.IsNullOrWhiteSpace(modele))
            {
                throw new ArgumentException("Le modèle est obligatoire.", nameof(modele));
            }

            if (nombrePlaces <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nombrePlaces), "Le nombre de places doit être positif.");
            }

            Marque = marque;
            Modele = modele;
            Couleur = couleur;
            TypeCarburant = typeCarburant;
            TypeTransmission = typeTransmission;
            NombrePlaces = nombrePlaces;
        }

        public void ChangerTarifJournalier(decimal nouveauTarif)
        {
            if (nouveauTarif <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nouveauTarif), "Le tarif journalier doit etre positif.");
            }
            TarifJournalier = nouveauTarif;
        }

        public void ChangerAgence(Guid nouvelleAgenceId)
        {
            if (nouvelleAgenceId == Guid.Empty)
            {
                throw new ArgumentException("L'agence est obligatoire.", nameof(nouvelleAgenceId));
            }
            AgenceId = nouvelleAgenceId;
        }
        public void Reserver()
        {
            if (Statut != StatutVehicule.Disponible)
            {
                throw new DomainException($"Impossible de réserver un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Reserve;
        }
        public void LibererReservation()
        {
            if (Statut != StatutVehicule.Reserve)
            {
                throw new DomainException($"Impossible de libérer la réservation d'un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Disponible;
        }

        public void DemarrerLocation()
        {
            if (Statut is not (StatutVehicule.Reserve or StatutVehicule.EnPreparation))
            {
                throw new DomainException($"Impossible de démarrer une location pour un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Loue;
        }

        public void EnregistrerRetour()
        {
            if (Statut != StatutVehicule.Loue)
            {
                throw new DomainException($"Impossible d'enregistrer un retour pour un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Disponible;
        }

        public void EnvoyerEnEntretien()
        {
            if (Statut is not (StatutVehicule.Disponible or StatutVehicule.Loue or StatutVehicule.Endommage))
            {
                throw new DomainException($"Impossible d'envoyer en entretien un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.EnEntretien;
        }

        public void MarquerCommeEndommage()
        {
            if (Statut != StatutVehicule.Loue)
            {
                throw new DomainException($"Impossible de marquer comme endommagé un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Endommage;
        }

        public void RemettreEnService()
        {
            if (Statut is not (StatutVehicule.EnEntretien or StatutVehicule.Endommage))
            {
                throw new DomainException($"Impossible de remettre en service un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Disponible;
        }

        public void Retirer()
        {
            if (Statut != StatutVehicule.Disponible)
            {
                throw new DomainException($"Impossible de retirer un véhicule au statut {Statut}.");
            }

            Statut = StatutVehicule.Retire;
            DateRetrait = DateTimeOffset.UtcNow;
        }

        public void MettreAJourKilometrage(int nouveauKilometrage)
        {
            if (nouveauKilometrage < Kilometrage)
            {
                throw new ArgumentOutOfRangeException(nameof(nouveauKilometrage), "Le kilométrage ne peut pas diminuer.");
            }

            Kilometrage = nouveauKilometrage;
        }
    }
}



