﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using JoinRpg.Data.Interfaces;
using JoinRpg.DataModel;
using JoinRpg.Domain;
using JoinRpg.Services.Interfaces;
using JoinRpg.Web.Controllers.Common;
using JoinRpg.Web.Helpers;
using JoinRpg.Web.Models;

namespace JoinRpg.Web.Controllers
{
  public class CharacterListController : ControllerGameBase
  {
    private IPlotRepository PlotRepository { get; }

    [HttpGet, Authorize]
    public Task<ActionResult> Active(int projectid, string export)
     => MasterCharacterList(projectid, claim => claim.IsActive, export, "Все персонажи");

    [HttpGet, Authorize]
    public Task<ActionResult> Deleted(int projectId, string export)
      => MasterCharacterList(projectId, character => !character.IsActive, export, "Удаленные персонажи");

    [HttpGet, Authorize]
    public async Task<ActionResult> ByUnAssignedField(int projectfieldid, int projectid, string export)
    {
      var field = await ProjectRepository.GetProjectField(projectid, projectfieldid);
      return await MasterCharacterList(projectid,
        character => character.HasProblemsForField(field) && character.IsActive, export, "Поле (непроставлено): " + field.FieldName);
    }

    private async Task<ActionResult> MasterCharacterList(int projectId, Func<Character, bool> predicate, string export, string title)
    {
      var characters = (await ProjectRepository.GetCharacters(projectId)).Where(predicate).ToList();

      var error = await AsMaster(characters, projectId);
      if (error != null) return error;

      var plots = await PlotRepository.GetPlotsWithTargets(projectId);
      var project = await GetProjectFromList(projectId, characters);

      var list = new CharacterListViewModel(CurrentUserId, title, characters, plots, project);

      var exportType = GetExportTypeByName(export);

      if (exportType == null)
      {
        return View("Index", list);
      }

      return await ExportWithCustomFronend(list.Items, list.Title, exportType.Value, new CharacterListItemViewModelExporter(list.Fields), list.ProjectName);
    }

    public CharacterListController(ApplicationUserManager userManager, IProjectRepository projectRepository, IProjectService projectService, IExportDataService exportDataService, IPlotRepository plotRepository) : base(userManager, projectRepository, projectService, exportDataService)
    {
      PlotRepository = plotRepository;
    }

    public async Task<ActionResult> ByGroup(int projectid, int charactergroupid, string export)
    {
      var characterGroup = await ProjectRepository.GetGroupAsync(projectid, charactergroupid);
      return
        await
          MasterCharacterList(projectid, character => character.IsActive &&  character.IsPartOfGroup(charactergroupid), export,
            "Персонажи — " + characterGroup.CharacterGroupName);
    }
  }
}