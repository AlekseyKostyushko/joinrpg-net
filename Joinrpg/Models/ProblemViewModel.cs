using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using JoinRpg.Domain;

namespace JoinRpg.Web.Models
{
  public class ProblemViewModel
  {
    public ProblemViewModel(ClaimProblem problem)
    {
      ProblemType = (ProblemTypeViewModel) problem.ProblemType;
      ProblemTime = problem.ProblemTime;
      Severity = problem.Severity;
      Extra = problem.ExtraInfo;
    }

    public ProblemTypeViewModel ProblemType { get;  }

    public DateTime? ProblemTime { get; }

    public ProblemSeverity Severity { get;  }

    public string Extra { get; }
  }


  /// <summary>
  /// <see cref="ClaimProblemType"/>
  /// </summary>
  public enum ProblemTypeViewModel
  {
    [Display(Name = "�� �������� ������")]
    [UsedImplicitly]
    NoResponsibleMaster,
    [Display(Name = "�������� ������")]
    [UsedImplicitly]
    InvalidResponsibleMaster,
    [Display(Name = "������ ��� ������")]
    [UsedImplicitly]
    ClaimNeverAnswered,
    [Display(Name = "�� ������ ��� �������")]
    [UsedImplicitly]
    ClaimNoDecision,
    [UsedImplicitly]
    [Display(Name = "�������� ��� �����")]
    ClaimActiveButCharacterHasApprovedClaim,
    [UsedImplicitly]
    [Display(Name = "����� �� ���������")]
    FinanceModerationRequired,
    [UsedImplicitly]
    [Display(Name = "���� ��������� �� ������")]
    TooManyMoney,
    [UsedImplicitly]
    [Display(Name = "���������� ������������")]
    ClaimDiscussionStopped,
    [UsedImplicitly]
    [Display(Name = "��� ��������� � ������")]
    NoCharacterOnApprovedClaim,
    [UsedImplicitly]
    [Display(Name = "����� ������� ��������")]
    FeePaidPartially,
    [UsedImplicitly]
    [Display(Name = "������ � ���������� ������")]
    UnApprovedClaimPayment,
    [UsedImplicitly]
    [Display(Name = "������ �� ������ �����������")]
    ClaimWorkStopped,
    [UsedImplicitly]
    [Display(Name = "������ �� ��������� �� � ����")]
    ClaimDontHaveTarget,
    [UsedImplicitly]
    [Display(Name = "�������� � ��������� ����")]
    DeletedFieldHasValue,
    [UsedImplicitly]
    [Display(Name = "���� �� ���������")]
    FieldIsEmpty
  }
}