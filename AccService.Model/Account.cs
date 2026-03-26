using System;
using System.Collections.Generic;

namespace AccService.Model;

public partial class Account
{
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int? SupplierId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string Username { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

}