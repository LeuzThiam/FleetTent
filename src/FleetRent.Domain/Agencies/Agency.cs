namespace FleetRent.Domain.Agencies;

using FleetRent.Domain.Common;

public sealed class Agency : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public string OpeningHours { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public Agency(
        Guid id,
        string code,
        string name,
        string address,
        string phoneNumber,
        string email,
        string openingHours)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Le code de l'agence est obligatoire.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Le nom de l'agence est obligatoire.", nameof(name));
        }

        Code = code;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        OpeningHours = openingHours;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateContactInformation(string address, string phoneNumber, string email)
    {
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateOpeningHours(string openingHours)
    {
        OpeningHours = openingHours;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}