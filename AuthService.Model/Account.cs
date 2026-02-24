using System;
using System.Collections.Generic;

namespace AuthService.Model;

public partial class Account
{
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int? SupplierId { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string Username { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Role Role { get; set; } = null!;

    public virtual Supplier? Supplier { get; set; }
}
