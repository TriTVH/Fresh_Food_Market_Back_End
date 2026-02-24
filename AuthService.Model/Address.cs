using System;
using System.Collections.Generic;

namespace AuthService.Model;

public partial class Address
{
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string RecipientName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public int WardId { get; set; }

    public int DistrictId { get; set; }

    public int CityId { get; set; }

    public int ProvinceId { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Account Customer { get; set; } = null!;
}
