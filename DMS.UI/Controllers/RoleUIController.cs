﻿using DMS.COMMON.Models;
using DMS.UI.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DMS.UI.Controllers
{
    public class RoleUIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _api;
        private readonly PermissionHtmlProcessor _htmlProcessor;
        public RoleUIController(IHttpClientFactory httpClientFactory, SettingsService settingsService, PermissionHtmlProcessor htmlProcessor)
        {
            _httpClientFactory = httpClientFactory;
            _api = settingsService.ApiSettings;
            _htmlProcessor = htmlProcessor; 
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = null, string searchCol = "", string sortColumn = "", string sortDir = null)

        { 



            var model = new RolesRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                SearchCol = searchCol, // <-- Added
                SortColumn = sortColumn,
                SortDir = sortDir
            };
            var client = _httpClientFactory.CreateClient();
            var url = _api.BaseUrlRole + "/GetRoles";
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await client.PostAsync(url, content); ;
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Roles = new List<Role>();
                ViewBag.TotalRecords = 0;
            }
            else
            {
                var response = await resp.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<RolesPagedResponse>(response);

                ViewBag.Roles = apiResult?.Items ?? new List<Role>();
                ViewBag.TotalRecords = apiResult?.TotalRecords ?? 0;
            }

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.SearchCol = searchCol; // <-- Set in ViewBag
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDir = sortDir;

            return View();
        }


       
        [HttpPost]
        public async Task<IActionResult> SaveRole([FromBody] Role model)
        {
            if (!TryValidateModel(model))
            {
                // Collect validation errors into a dictionary
                var errors = "";
                return Json(new { success = false, errors });
            }

            try
            {
                if (model.RoleID == 0) model.RoleID = 0;

                var client = _httpClientFactory.CreateClient();
                var url = model.RoleID > 0 ? _api.BaseUrlRole + "/UpdateRole" : _api.BaseUrlRole + "/AddRole";
                var user = TokenHelper.UserFromToken(HttpContext);

                model.CreatedBy = user.UserID;

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Could not save role. Please try again." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult ValidateField([FromBody] Dictionary<string, string> fieldData)
        {
            var model = new Role();

            // Bind the single field into model
            foreach (var field in fieldData)
            {
                if (field.Key == "RoleName")
                    model.RoleName = field.Value;
                if (field.Key == "RoleDescription")
                    model.RoleDescription = field.Value;
            }

            // Validate only that property
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new { errors });
            }

            return Json(new { success = true });
        }





        [HttpPost]
        public async Task<IActionResult> DeleteRole([FromBody]int RoleID)
        {
            var user = TokenHelper.UserFromToken(HttpContext);
            var client = _httpClientFactory.CreateClient();
            var url = _api.BaseUrlRole + "/DeleteRole";

            var payload = new DeleteRole
            {
               
                RoleID = RoleID,
               
                CreatedBy = user.UserID
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await client.PostAsync(url, content);

            if (resp.IsSuccessStatusCode)
                return Json(new { success = true });
            else
                return Json(new { success = false });

            
        }

        [HttpPost]
        public async Task<IActionResult> GetRoleById([FromBody] int roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var url = _api.BaseUrlRole + "/GetRoleById";

            var json = JsonConvert.SerializeObject(roleId); // plain integer JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await client.PostAsync(url, content);
            if (!resp.IsSuccessStatusCode)
                return BadRequest();

            var response = await resp.Content.ReadAsStringAsync();
            var role = JsonConvert.DeserializeObject<Role>(response);

            return Json(role);
        }

        /// <summary>
        /// example of when action permission we need to give afer any evennt not onload
        /// </summary>

        //      [HttpGet]
        //public async Task<IActionResult> GetDynamicHtml()
        //{
        //    // Example HTML returned dynamically
        //    string html = @"
        //        <div>
        //            <button>Visible to everyone</button>

        //            <has-permission module-id=""5"" action-id=""2"">
        //                <button>Only Admins can see this</button>
        //            </has-permission>

        //            <has-permission module-id=""5"" action-id=""9"">
        //                <button>Only Managers can see this</button>
        //            </has-permission>
        //        </div>";

        //    // Process HTML according to current user's permissions
        //    string processedHtml = await _htmlProcessor.ProcessAsync(html);

        //    return Content(processedHtml, "text/html");
        //}


    }
}
