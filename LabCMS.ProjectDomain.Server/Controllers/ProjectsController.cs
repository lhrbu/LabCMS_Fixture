﻿using LabCMS.ProjectDomain.Server.Repositories;
using LabCMS.ProjectDomain.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabCMS.ProjectDomain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsRepository _repository;
        public ProjectsController(
            ProjectsRepository repository)
        { _repository = repository; }

        [HttpGet]
        public IAsyncEnumerable<Project> GetAsync() => _repository.Projects.AsNoTracking().AsAsyncEnumerable();
    }
}