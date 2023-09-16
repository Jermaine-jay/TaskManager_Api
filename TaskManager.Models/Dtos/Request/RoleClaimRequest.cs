using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Dtos.Request
{
    public class RoleClaimRequest
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }

    public class RoleClaimResponse
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }

    public class UpdateRoleClaimsDto
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
        public string NewClaim { get; set; }
    }
}
