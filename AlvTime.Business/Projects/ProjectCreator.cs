﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlvTime.Business.Projects
{
    public class ProjectCreator
    {
        private readonly IProjectStorage _storage;

        public ProjectCreator(IProjectStorage storage)
        {
            _storage = storage;
        }

        public ProjectResponseDto CreateProject(CreateProjectDto project)
        {
            if (!GetProject(project).Any())
            {
                _storage.CreateProject(project);
            }

            return GetProject(project).SingleOrDefault();
        }

        public IEnumerable<ProjectResponseDto> GetProject(CreateProjectDto project)
        {
            return _storage.GetProjects(new ProjectQuerySearch
            {
                Customer = project.Customer,
                Name = project.Name
            }).ToList();
        }
    }
}
