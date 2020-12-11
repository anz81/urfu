using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class ProjectsController : BaseController
    {
        /// <summary>
        /// Получаем новый проект
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IActionResult Post(ProjectApiDto project)
        {
            Logger.Info($"Пришел проект {project?.id}");

            if (project == null)
            {
                Logger.Info("Пустой проект");
                return BadRequest("Пустой проект");
            }

            ProjectSync syncProject = new ProjectSync();
            if (syncProject.AddProject(project))
            {
                Logger.Info($"Проект {project.id} успешно получен");
                return Json(new { success = true });
            }
            else
            {
                Logger.Info($"Ошибка при получении проекта {project.id}. {syncProject.Message}");
                return BadRequest(syncProject.Message);
            }
        }

        /// <summary>
        /// Обновляем существующий проект
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IActionResult Put(int id, ProjectApiDto project)
        {
            Logger.Info($"Пришло обновление проекта {project?.id}");

            if (project == null)
            {
                Logger.Info("Пустой проект");
                return BadRequest("Пустой проект");
            }

            ProjectSync syncProject = new ProjectSync();
            if (syncProject.UpdateProject(project))
            {
                Logger.Info($"Проект {project.id} успешно обновлен");
                return Json(new { success = true });
            }
            else
            {
                Logger.Info($"Ошибка при обновлении проекта {project.id}");
                return BadRequest(syncProject.Message);
            }
        }
    }
}