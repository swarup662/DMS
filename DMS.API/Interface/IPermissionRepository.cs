// File: DMS.API/Repositories/Interfaces/IPermissionRepository.cs
using DMS.COMMON.Models;
using System.Collections.Generic;
using System.Data;

public interface IPermissionRepository
{
    List<MenuItem> GetUserMenu(int userID);
    DataTable GetModules();
    DataTable GetActions();
    int AssignRolePermission(RolePermission model);

    // NEW:
    List<ActionDto> GetUserAllowedActions(int userID);
    bool HasPermission(int userID, int menuModuleId, int actionId);
}
