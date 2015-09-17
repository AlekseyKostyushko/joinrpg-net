﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using JoinRpg.Data.Interfaces;
using JoinRpg.DataModel;
using JoinRpg.Domain;
using JoinRpg.Helpers;
using JoinRpg.Services.Interfaces;
using JoinRpg.Web.Models;
using JoinRpg.Web.Models.Plot;

namespace JoinRpg.Web.Controllers
{
  public class CharacterController : Common.ControllerGameBase
  {
    private readonly IPlotRepository _plotRepository;

    public CharacterController(ApplicationUserManager userManager, IProjectRepository projectRepository,
      IProjectService projectService, IPlotRepository plotRepository) : base(userManager, projectRepository, projectService)
    {
      _plotRepository = plotRepository;
    }

    [HttpGet]
    public async Task<ActionResult> Details(int projectid, int characterid)
    {
      var field = await ProjectRepository.GetCharacterAsync(projectid, characterid);
      return WithEntity(field) ?? await ShowCharacter(field.Project, field);
    }

    private async Task<ActionResult> ShowCharacter(Project project, Character character)
    {
      var hasMasterAccess = project.HasAccess(CurrentUserIdOrDefault);
      var approvedClaimUser = character.ApprovedClaim?.Player;
      var hasAnyAccess = hasMasterAccess || (User.Identity.IsAuthenticated && approvedClaimUser?.UserId == CurrentUserId);
      var viewModel = new CharacterDetailsViewModel
      {
        CharacterName = character.CharacterName,
        Description = character.Description,
        ApprovedClaimId = character.ApprovedClaim?.ClaimId,
        ApprovedClaimUser = approvedClaimUser,
        CanAddClaim = character.IsAvailable,
        DiscussedClaims = LoadIfMaster(project, () => character.Claims.Where(claim => claim.IsInDiscussion)).Select(ClaimListItemViewModel.FromClaim),
        RejectedClaims = LoadIfMaster(project, () => character.Claims.Where(claim => !claim.IsActive)).Select(ClaimListItemViewModel.FromClaim),
        CharacterFields = character.Fields().Select(pair => pair.Value),
        ProjectName = project.ProjectName,
        ProjectId = project.ProjectId,
        CharacterId = character.CharacterId,
        HasPlayerAccessToCharacter = hasAnyAccess,
        HasMasterAccess = hasMasterAccess,
      };
      if (hasAnyAccess)
      {
        viewModel.Plot = (await _plotRepository.GetPlotsForCharacter(character)).Select(p => PlotElementViewModel.FromPlotElement(p, hasMasterAccess));
      }
      return View(viewModel);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<ActionResult> Details(int projectId, int characterId, string characterName, MarkdownString description,
      FormCollection formCollection)
    {
      try
      {
        await ProjectService.SaveCharacterFields(projectId, characterId, CurrentUserId, characterName, description.Contents,
          GetCharacterFieldValuesFromPost(formCollection.ToDictionary()));
        return RedirectToAction("Details", new {projectId, characterId});
      }
      catch
      {
        return await Details(projectId, characterId);
      }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Create(int projectid, int charactergroupid)
    {
      var field = await ProjectRepository.LoadGroupAsync(projectid, charactergroupid);

      return AsMaster(field, pa => pa.CanChangeFields) ?? View(new AddCharacterViewModel()
      {
        Data = CharacterGroupListViewModel.FromGroupAsMaster(field.Project.RootGroup),
        ProjectId = projectid,
        ParentCharacterGroupIds = new List<int> {charactergroupid}
      });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Create(AddCharacterViewModel viewModel)
    {
      var project1 = ProjectRepository.GetProject(viewModel.ProjectId);
      return AsMaster(project1, acl => true) ?? ((Func<Project, ActionResult>) (project =>
      {
        try
        {
          ProjectService.AddCharacter(
            viewModel.ProjectId,
            viewModel.Name, viewModel.IsPublic, viewModel.ParentCharacterGroupIds, viewModel.IsAcceptingClaims,
            viewModel.Description.Contents);

          return RedirectToIndex(project);
        }
        catch
        {
          return View(viewModel);
        }
      }))(project1);
    }
  }
}