using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;
using JoinRpg.DataModel;
using JoinRpg.Domain;

namespace JoinRpg.Web.Models
{
  public enum CharacterBusyStatusView
  {
    [Display(Name = "�����")]
    HasPlayer,
    [Display(Name = "�����������")]
    Discussed,
    [Display(Name = "��� ������")]
    NotSend,
  }

  public class CharacterListItemViewModel
  {
    [Display(Name="�����?")]
    public CharacterBusyStatusView BusyStatus { get; }

    public int ProjectId { get; }

    [Display(Name = "���")]
    public string Name { get; set; }

    public int CharacterId { get; }

    [NotNull, ReadOnly(true)]
    public CustomFieldsViewModel Fields { get; }

    public int? ApprovedClaimId { get; }

    [Display(Name = "�����"), CanBeNull]
    public User Player { get; set; }

    public int IndReadyPlotsCount { get; }
    public int IndAllPlotsCount { get; }
    public int ColReadyPlotsCount { get; }
    public int ColAllPlotsCount{ get; }

    public CharacterListItemViewModel ([NotNull] Character character, int currentUserId, IEnumerable<ClaimProblem> problems, IReadOnlyCollection<PlotElement> plots)
    {
      if (character == null) throw new ArgumentNullException(nameof(character));

      
      if (character.ApprovedClaim != null)
      {
        BusyStatus = CharacterBusyStatusView.HasPlayer;
        ApprovedClaimId = character.ApprovedClaim.ClaimId;
        Player = character.ApprovedClaim.Player;
      }
      else if (character.Claims.Any(c => c.IsActive))
      {
        BusyStatus = CharacterBusyStatusView.Discussed;
      }
      else 
      {
        BusyStatus = CharacterBusyStatusView.NotSend;
      }
      ProjectId = character.ProjectId;
      Name = character.CharacterName;
      CharacterId = character.CharacterId;
      Fields = new CustomFieldsViewModel(currentUserId, character);
      Problems = problems.Select(p => new ProblemViewModel(p)).ToList();

      IndReadyPlotsCount = plots.Count(p => p.IsCompleted && p.TargetCharacters.Contains(character));
      IndAllPlotsCount = plots.Count(p => p.IsActive && p.TargetCharacters.Contains(character));
      ColReadyPlotsCount = plots.Count(p => p.IsCompleted && !p.TargetCharacters.Contains(character));
      ColAllPlotsCount = plots.Count(p => p.IsActive && !p.TargetCharacters.Contains(character));
    }

    [Display(Name="��������")]
    public ICollection<ProblemViewModel> Problems { get; set; }
  }
}