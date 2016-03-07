using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace JoinRpg.CommonUI.Models
{
  public enum CommentExtraAction
  {
    [Display(Name = "���������� �������� ������������", ShortName = "��������")]
    [UsedImplicitly]
    ApproveFinance,
    [Display(Name = "���������� �������� ���������", ShortName = "��������")]
    [UsedImplicitly]
    RejectFinance,
    [Display(Name = "������ �������� ��������", ShortName = "��������")]
    [UsedImplicitly]
    ApproveByMaster,
    [Display(Name = "������ ��������� ��������", ShortName = "���������")]
    [UsedImplicitly]
    DeclineByMaster,
    [Display(Name = "������ ������������� ��������", ShortName = "�������������")]
    [UsedImplicitly]
    RestoreByMaster,
    [Display(Name = "������ ���������� ��������", ShortName = "��������")]
    [UsedImplicitly]
    MoveByMaster,
    [Display(Name = "������ �������� �������", ShortName = "��������")]
    [UsedImplicitly]
    DeclineByPlayer,
    [Display(Name = "������������� ������ �������", ShortName = "��������")]
    [UsedImplicitly]
    ChangeResponsible,
    [Display(Name = "����� ������", ShortName = "������")]
    [UsedImplicitly]
    NewClaim,
    [Display(Name = "������ ���������� � ���� ��������", ShortName = "��������")]
    [UsedImplicitly]
    OnHoldByMaster
  }
}