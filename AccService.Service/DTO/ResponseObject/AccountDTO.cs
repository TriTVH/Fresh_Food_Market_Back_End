using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.DTO.ResponseObject
{
    public class AccountDTO
    {
        public int AccountId { get; set; }

        public string RoleName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Username { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public AccountDTO() { }

        public
            AccountDTO(int accountId, string roleName, string? email, string? password, string? phone, string username, bool isActive, string? avatarUrl, DateTime createdDate, DateTime updatedDate)
        {
            AccountId = accountId;
            RoleName = roleName;
            Email = email;
            Phone = phone;
            Username = username;
            IsActive = isActive;
            AvatarUrl = avatarUrl;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
        }
    }
}
