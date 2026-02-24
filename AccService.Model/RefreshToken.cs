using System;
using System.Collections.Generic;

namespace AccService.Model;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public bool Revoked { get; set; }

    public virtual Account User { get; set; } = null!;
}
