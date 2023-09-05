using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Middleware
{
    public class CustomErrorHandler
    {
        private readonly RequestDelegate _next;

        public CustomErrorHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (ex)
                {
                    case CustomException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                // var result = JsonSerializer.Create().Serialize(new { message = ex?.Message });
                var result = JsonConvert.SerializeObject(new { message = ex?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
