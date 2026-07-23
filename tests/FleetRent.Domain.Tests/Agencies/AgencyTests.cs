

using FleetRent.Domain.Agencies;

namespace FleetRent.Domain.Tests.Agencies
{
    public class AgencyTests
    {
        private static Agency CreerAgence() =>
            new(
                Guid.NewGuid(),
                "MTL01",
                "Agence Montréal Central",
                "123 Rue Principale, Montréal, QC",
                "514-123-4567",
                "montreal@fleetrent.com",
                "Lun-Ven: 9h-18h, Sam: 10h");

        [Fact]
        public void Constructeur_AvecCodeVide_LeveArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Agency(
                Guid.NewGuid(), "", "Nom", "Adresse", "514-555-0100", "a@b.com", "Lun-Ven"));
        }

        [Fact]
        public void Constructeur_AvecNomVide_LeveArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Agency(
                Guid.NewGuid(), "MTL01", "", "Adresse", "514-555-0100", "a@b.com", "Lun-Ven"));
        }

        [Fact]
        public void Constructeur_ParDefaut_EstActive()
        {
            var agency = CreerAgence();
            Assert.True(agency.IsActive);
        }

        [Fact]
        public void Deactivate_MetIsActiveAFaux()
        {
            var agency = CreerAgence();
            agency.Deactivate();
            Assert.False(agency.IsActive);
        }

        [Fact]
        public void Activate_MetIsActiveAVrai()
        {
            var agency = CreerAgence();
            agency.Deactivate();

            agency.Activate();

            Assert.True(agency.IsActive);
        }

        [Fact]
        public void UpdateContactInformation_MetAJourLesChamps()
        {
            var agency = CreerAgence();

            agency.UpdateContactInformation("456 rue Nouvelle", "514-555-0200", "nouveau@fleetrent.com");

            Assert.Equal("456 rue Nouvelle", agency.Address);
            Assert.Equal("514-555-0200", agency.PhoneNumber);
            Assert.Equal("nouveau@fleetrent.com", agency.Email);
        }

        [Fact]
        public void UpdateOpeningHours_MetAJourLHoraire()
        {
            var agency = CreerAgence();

            agency.UpdateOpeningHours("Lun-Dim 7h-20h");

            Assert.Equal("Lun-Dim 7h-20h", agency.OpeningHours);
        }
    }
}
