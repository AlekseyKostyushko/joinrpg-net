using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JoinRpg.DataModel;
using JoinRpg.Domain.CharacterFields;

namespace JoinRpg.Services.Interfaces.Notification
{




    public class AddCommentEmail : ClaimEmailModel
    {
    }

    public class NewClaimEmail : ClaimEmailModel, IEmailWithUpdatedFieldsInfo
    {
        public IReadOnlyCollection<FieldWithPreviousAndNewValue> UpdatedFields { get; set; } =
            new List<FieldWithPreviousAndNewValue>();

        public IFieldContainter FieldsContainer => Claim;

        public IReadOnlyDictionary<string, PreviousAndNewValue> OtherChangedAttributes { get; } =
            new Dictionary<string, PreviousAndNewValue>();
    }

    public class ApproveByMasterEmail : ClaimEmailModel
    {
    }

    public class CheckedInEmal : ClaimEmailModel
    {
    }

    public class SecondRoleEmail : ClaimEmailModel
    {
    }

    public class DeclineByMasterEmail : ClaimEmailModel
    {
    }

    public class OnHoldByMasterEmail : ClaimEmailModel
    {

    }

    public class FieldsChangedEmail : EmailModelBase, IEmailWithUpdatedFieldsInfo
    {
        public IReadOnlyCollection<FieldWithPreviousAndNewValue> UpdatedFields { get; set; } =
            new List<FieldWithPreviousAndNewValue>();

        public IFieldContainter FieldsContainer => (IFieldContainter) Claim ?? Character;

        [CanBeNull]
        public IReadOnlyDictionary<string, PreviousAndNewValue> OtherChangedAttributes { get; set; }
            =
            new Dictionary<string, PreviousAndNewValue>();

        public Character Character { get; set; }

        [CanBeNull]
        public Claim Claim { get; set; }

        //Is character is null, Claim is not null and vioce versa. (restricted by constructors).
        public bool IsCharacterMail => Character != null;

        public FieldsChangedEmail(
            Claim claim,
            User initiator,
            ICollection<User> recipients,
            IReadOnlyCollection<FieldWithPreviousAndNewValue> updatedFields)
            : this(null, claim, initiator, recipients, updatedFields, null)
        {
        }

        public FieldsChangedEmail(
            Character character,
            User initiator,
            ICollection<User> recipients,
            IReadOnlyCollection<FieldWithPreviousAndNewValue> updatedFields,
            Dictionary<string, PreviousAndNewValue> otherChangedAttributes)
            : this(character, null, initiator, recipients, updatedFields, otherChangedAttributes)
        {
        }

        private FieldsChangedEmail(
            Character character,
            Claim claim,
            User initiator,
            ICollection<User> recipients,
            [NotNull]
            IReadOnlyCollection<FieldWithPreviousAndNewValue> updatedFields,
            [CanBeNull]
            Dictionary<string, PreviousAndNewValue> otherChangedAttributes)
        {
            if (updatedFields == null) throw new ArgumentNullException(nameof(updatedFields));
            if (character != null && claim != null)
                throw new ArgumentException(
                    $"Both {nameof(character)} and {nameof(claim)} were provided");
            if (character == null && claim == null)
                throw new ArgumentException(
                    $"Neither  {nameof(character)} nor {nameof(claim)} were provided");
            otherChangedAttributes =
                otherChangedAttributes ?? new Dictionary<string, PreviousAndNewValue>();

            Character = character;
            Claim = claim;
            ProjectName = character?.Project.ProjectName ?? claim?.Project.ProjectName;
            Initiator = initiator;
            Text = new MarkdownString();
            Recipients = recipients;
            UpdatedFields = updatedFields;
            OtherChangedAttributes = otherChangedAttributes;
        }
    }

    public class RestoreByMasterEmail : ClaimEmailModel
    {
    }

    public class MoveByMasterEmail : ClaimEmailModel
    {
    }

    public class ChangeResponsibleMasterEmail : ClaimEmailModel
    {
    }

    public class DeclineByPlayerEmail : ClaimEmailModel
    {
    }

    public class FinanceOperationEmail : ClaimEmailModel
    {
        public int FeeChange { get; set; }
        public int Money { get; set; }
    }

    public class ClaimEmailModel : EmailModelBase
    {
        public ParcipantType InitiatorType { get; set; }
        public Claim Claim { get; set; }
        public CommentExtraAction? CommentExtraAction { get; set; }
    }


    public enum ParcipantType
    {
        Nobody,
        Master,
        Player
    }
}
