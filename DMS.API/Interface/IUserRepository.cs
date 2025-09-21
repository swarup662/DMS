using DMS.COMMON.Models;
using System.Collections.Generic;
using System.Data;

namespace DMS.API.Repositories
{
    public interface IUserRepository
    {
        int SaveUser(User model);
        int AssignPermissions(List<UserPermissionModel> permissions);
        DataTable GetAllUsers();
    }
}
