namespace FleetRent.Domain.Enums;

public enum ReservationStatus
{
    Draft = 1,
    Pending = 2,
    Confirmed = 3,
    ConvertedToRental = 4,
    Cancelled = 5,
    Expired = 6
}