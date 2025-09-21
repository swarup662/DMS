﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.COMMON.Models
{
    public class Role
    {
        public int RoleID { get; set; }

        [Required(ErrorMessage = "Role Name is required.")]
        [StringLength(20, ErrorMessage = "Role Name cannot exceed 100 characters.")]
        public string RoleName { get; set; } 

        [Required(ErrorMessage = "Role Description is required.")]
        [StringLength(100, ErrorMessage = "Role Description cannot exceed 250 characters.")]
        public string RoleDescription { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public int? ActiveFlag { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
    public class DeleteRole
    {
        public int? RoleID { get; set; }

        public int? CreatedBy { get; set; }
        

    }


    public class RolesPagedResponse
    {
        public List<Role> Items { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDir { get; set; }
        public string? Search { get; set; }
        public string? SearchCol { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalRecords / (double)PageSize) : 0;
    }



    public class RolesRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string? Search { get; set; } 
        public string? SortColumn { get; set; } 
        public string? SortDir { get; set; } 
        public string? SearchCol { get; set; }
    }
}
