﻿using Azure.Core;
using DMS.API.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json; // ← Use this instead of System.Text.Json
using System.Net;

namespace DMS.API.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DBHelper _dbHelper;

        public ErrorLoggingMiddleware(RequestDelegate next, DBHelper dbHelper)
        {
            _next = next;
            _dbHelper = dbHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogError(ex, context);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { error = "Internal Server Error" }); // ← Changed here
                await context.Response.WriteAsync(result);
            }
        }

        private Task LogError(Exception ex, HttpContext context)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@UserID", "0"},
                    { "@ErrorType", "API EXCEPTION"},
                    { "@ErrorMessage", ex.Message }, 
                    { "@StackTrace", ex.StackTrace },
                    { "@RequestPath", context.Request.Path.ToString() },
                    { "@Headers", JsonConvert.SerializeObject(context.Request.Headers) } // ← Changed here
                };
                _dbHelper.ExecuteSP_ReturnInt("sp_ErrorLogs", parameters);
            }
            catch
            {
                // Optionally log internal logging error
            }

            return Task.CompletedTask;
        }
    }
}
