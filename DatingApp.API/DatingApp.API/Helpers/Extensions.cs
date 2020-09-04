﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
       public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-error",message);
            response.Headers.Add("CORS-expose-error", "ApplicationError");
            response.Headers.Add("CORS-allow-any-origin", "*");

        }
    }
}
