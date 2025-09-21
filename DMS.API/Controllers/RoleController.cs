﻿using DMS.API.Repositories.Interfaces;
using DMS.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DMS.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepo;

        public RoleController(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        [HttpPost("GetRoles")]
        public IActionResult GetRoles([FromBody] RolesRequest request)
        {
            if (request == null) return BadRequest("Request is required.");

            var dt = _roleRepo.GetRoles(
                request.PageSize,
                request.PageNumber,
                request.Search,
                request.SortColumn,
                request.SortDir,
                request.SearchCol
            );

            var roles = new List<Role>();
            foreach (DataRow row in dt.Rows)
            {
                roles.Add(new Role
                {
                    RoleID = Convert.ToInt32(row["RoleID"]),
                    RoleName = row["RoleName"]?.ToString() ?? string.Empty,
                    RoleDescription = row["RoleDescription"]?.ToString() ?? string.Empty,
                    CreatedBy = row["CreatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["CreatedBy"]),
                    CreatedDate = row["CreatedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["CreatedDate"]),
                    UpdatedBy = row["UpdatedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["UpdatedBy"]),
                    UpdatedDate = row["UpdatedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["UpdatedDate"]),
                    ActiveFlag = row["ActiveFlag"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["ActiveFlag"]),
                    
                });
            }

            int totalRecords = 0;
            if (dt.Rows.Count > 0 && dt.Columns.Contains("TotalRecords"))
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);

            var response = new RolesPagedResponse
            {
                Items = roles,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortColumn = request.SortColumn,
                SortDir = request.SortDir,
                Search = request.Search,
                SearchCol = request.SearchCol
            };

            return Ok(response);
        }

        [HttpPost("AddRole")]
        public IActionResult AddRole([FromBody] Role role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.RoleName))
                return BadRequest("Role name is required.");

            int newId = _roleRepo.SaveRole("INSERT", null, role.RoleName, role.RoleDescription, role.CreatedBy ?? 0);
            
            return Ok(new { RoleID = newId, Message = "Role created successfully." });
        }

        [HttpPost("UpdateRole")]
        public IActionResult UpdateRole([FromBody] Role role)
        {
            if (role == null || role.RoleID <= 0)
                return BadRequest("Valid Role is required.");

            int result = _roleRepo.SaveRole("UPDATE", role.RoleID, role.RoleName, role.RoleDescription, role.CreatedBy ?? 0);

            if (result > 0)
                return Ok(new { RoleID = result, Message = "Role updated successfully." });

            return NotFound(new { RoleID = result, Message = "Role not found or not updated." });
        }


        [HttpPost("DeleteRole")]
        public IActionResult DeleteRole([FromBody] DeleteRole role)
        {
            if (role.RoleID <= 0) return BadRequest("Invalid RoleID.");

            int result = _roleRepo.SaveRole("DELETE", role.RoleID, null, null, 0);

            if (result > 0)
                return Ok(new { RoleID = result,Message = "Role deleted successfully." });

            return NotFound(new { RoleID = result, Message = "Role not found or already deleted." });
        }

        [HttpPost("GetRoleById")]
        public IActionResult GetRoleById([FromBody] int roleId)
        {
            if (roleId <= 0) return BadRequest("Invalid RoleID.");

            var role = _roleRepo.GetRoleById(roleId);

            if (role == null)
                return NotFound(new { Message = "Role not found." });

            return Ok(role);
        }

    }
}
