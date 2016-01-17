using System;
using System.Collections.Generic;

namespace Dashboard.Models.Account
{
    public class UsersInRoleModel
    {
        public IEnumerable<Guid> RemovedUsers { get; set; }
        public IEnumerable<Guid> EnrolledUsers { get; set; }
        public Guid RoleId { get; set; }
    }
}
