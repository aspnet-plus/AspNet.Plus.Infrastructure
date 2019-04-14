// ASP.NET.Plus
// Copyright (c) 2019 AspNet.Plus.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.ExceptionInterceptSample.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Mvc.Controller" />
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException">No such user exists for login.</exception>
        /// <exception cref="ValidationException">Missing user name.</exception>
        /// <exception cref="ArgumentNullException"></exception>
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
