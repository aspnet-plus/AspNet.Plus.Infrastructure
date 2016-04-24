// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.AspNet.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.ExceptionInterceptSample.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        [HttpGet("{id}")]
        public Task<string> Get(int id)
        {
            // throw an exception to simulate an unhandled exception.
            if (id == 1)
            {
                throw new UnauthorizedAccessException("No such user exists for login.");
            }

            if (id == 2)
            {
                throw new ValidationException("Missing user name.");
            }

            throw new ArgumentNullException(nameof(id));
        }
    }
}
