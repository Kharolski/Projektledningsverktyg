using System;
using System.Windows;

namespace Projektledningsverktyg.Data.Entities
{
    public enum CommentType
    {
        Task,
        Discussion,
        AIAssistant,
        FamilyChat,
        Event
    }
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberId { get; set; }
        public CommentType Type { get; set; }


        // Nullable foreign keys for different features
        public int? TaskId { get; set; }
        public int? DiscussionId { get; set; }
        public int? EventId { get; set; }
        public int? ChatId { get; set; }

        // Navigation properties
        public virtual Task Task { get; set; }
        public virtual Discussion Discussion { get; set; }
        public virtual Event Event { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Member Member { get; set; }

        // To show our comment base on user inloged
        public FlowDirection CommentFlow
        {
            get { return MemberId == App.CurrentUser.Id ? FlowDirection.RightToLeft : FlowDirection.LeftToRight; }
        }
        public bool IsCurrentUserComment
        {
            get
            {

                var isCurrentUser = MemberId == App.CurrentUser.Id;
                System.Diagnostics.Debug.WriteLine($"MemberId: {MemberId}, CurrentUser: {App.CurrentUser.Id}, IsMatch: {isCurrentUser}");
                return isCurrentUser;
            }
        }




    }
}
