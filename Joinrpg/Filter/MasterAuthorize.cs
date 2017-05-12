﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JetBrains.Annotations;
using JoinRpg.Data.Interfaces;
using JoinRpg.DataModel;
using JoinRpg.Web.Controllers.Common;
using JoinRpg.Web.Models;
using Microsoft.AspNet.Identity;

namespace JoinRpg.Web.Filter
{

  public class MasterAuthorize : AuthorizeAttribute
  {
    public Permission Permission { get; set; }

    public bool AllowPublish { get; set; }

    public bool AllowAdmin { get; set; }

    public MasterAuthorize(Permission permission = Permission.None)
    {
      Permission = permission;
    }

    private const string ProjectidKey = "projectId";
    private const string ProjectrepositoryKey = "projectRepository";

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
      var projectId = filterContext.Controller.ValueProvider.GetValue("ProjectId");
      var projectRepository = ((ControllerGameBase) filterContext.Controller).ProjectRepository;
      filterContext.HttpContext.Items.Add(ProjectidKey, projectId);
      filterContext.HttpContext.Items.Add(ProjectrepositoryKey, projectRepository);
      base.OnAuthorization(filterContext);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
      {
        base.HandleUnauthorizedRequest(filterContext);
        return;
      }

      var project = LoadProject(filterContext.HttpContext);
      if (project == null)
      {
        filterContext.Result = new HttpNotFoundResult();
      }
      else
      {
        filterContext.Result = new ViewResult()
        {
          ViewName = "ErrorNoAccessToProject",
          ViewData = new ViewDataDictionary(new ErrorNoAccessToProjectViewModel(project, Permission))
        };
      }
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      return base.AuthorizeCore(httpContext) && AuthorizeMaster(httpContext);
    }

    private bool AuthorizeMaster(HttpContextBase httpContext)
    {
      var project = LoadProject(httpContext);

      if (project == null)
      {
        return false;
      }

      if (AllowPublish && project.Details.PublishPlot)
      {
        return true;
      }

      var userId = httpContext.User.Identity.GetUserId();

      if (AllowAdmin && httpContext.User.IsInRole(Security.AdminRoleName))
      {
        return true;
      }
      
      return project.ProjectAcls.Any(acl => acl.UserId.ToString() == userId && GetPermssionExpression()(acl));
    }

    [CanBeNull]
    private static Project LoadProject(HttpContextBase httpContext)
    {
      var projectId = GetProjectId(httpContext);
      var repository = (IProjectRepository) httpContext.Items[ProjectrepositoryKey];
      return repository.GetProjectAsync(projectId).GetAwaiter().GetResult();
    }

    private static int GetProjectId(HttpContextBase httpContext)
    {
      var projectIdRawValue = GetRawValue(httpContext, ProjectidKey);
      if (projectIdRawValue.GetType().IsArray)
      {
        return int.Parse(((string[]) projectIdRawValue)[0]);
      }
      else
      {
        return int.Parse((string)projectIdRawValue);
      }
    }

    [MustUseReturnValue]
    private Func<ProjectAcl, bool> GetPermssionExpression()
    {
      switch (Permission)
      {
        case Permission.CanChangeFields:
          return acl => acl.CanChangeFields;
        case Permission.CanChangeProjectProperties:
          return acl => acl.CanChangeProjectProperties;
        case Permission.CanGrantRights:
          return acl => acl.CanGrantRights;
        case Permission.CanManageClaims:
          return acl => acl.CanManageClaims;
        case Permission.CanEditRoles:
          return acl => acl.CanEditRoles;
        case Permission.CanManageMoney:
          return acl => acl.CanManageMoney;
        case Permission.CanSendMassMails:
          return acl => acl.CanSendMassMails;
        case Permission.CanManagePlots:
          return acl => acl.CanManagePlots;
        case Permission.None:
          return acl => true;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static object GetRawValue(HttpContextBase httpContext, string key)
    {
      return ((ValueProviderResult) httpContext.Items[key]).RawValue;
    }
  }
}