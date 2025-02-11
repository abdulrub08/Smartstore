﻿namespace Smartstore.Forums.Domain
{
    /// <summary>
    /// Represents a forum topic type.
    /// </summary>
    public enum ForumTopicType
    {
        /// <summary>
        /// Normal.
        /// </summary>
        Normal = 10,

        /// <summary>
        /// Sticky.
        /// </summary>
        Sticky = 15,

        /// <summary>
        /// Announcement.
        /// </summary>
        Announcement = 20,
    }

    public enum ForumDateFilter
    {
        LastVisit = 0,
        Yesterday = 1,
        LastWeek = 7,
        LastTwoWeeks = 14,
        LastMonth = 30,
        LastThreeMonths = 92,
        LastSixMonths = 183,
        LastYear = 365
    }

    /// <summary>
    /// Represents a forum editor type.
    /// </summary>
    public enum EditorType
    {
        /// <summary>
        /// Simple text box.
        /// </summary>
        SimpleTextBox = 10,

        /// <summary>
        /// BBCode editor.
        /// </summary>
        BBCodeEditor = 20
    }

    /// <summary>
    /// Represents the sorting of forum topics.
    /// </summary>
    public enum ForumTopicSorting
    {
        /// <summary>
        /// Initial state
        /// </summary>
        Initial = 0,

        /// <summary>
        /// Relevance
        /// </summary>
        Relevance,

        /// <summary>
        /// Subject: A to Z
        /// </summary>
        SubjectAsc,

        /// <summary>
        /// Subject: Z to A
        /// </summary>
        SubjectDesc,

        /// <summary>
        /// User name: A to Z
        /// </summary>
        UserNameAsc,

        /// <summary>
        /// User name: Z to A
        /// </summary>
        UserNameDesc,

        /// <summary>
        /// Creation date: Oldest first
        /// </summary>
        CreatedOnAsc,

        /// <summary>
        /// Creation date: Newest first
        /// </summary>
        CreatedOnDesc,

        /// <summary>
        /// Number of posts: Low to High
        /// </summary>
        PostsAsc,

        /// <summary>
        /// Number of posts: High to Low
        /// </summary>
        PostsDesc
    }

    [Flags]
    public enum ForumModerationPermissionFlags
    {
        None = 0,
        
        CanCreateTopics = 1 << 0,
        CanEditTopic = 1 << 1,
        CanMoveTopic = 1 << 2,
        CanDeleteTopic = 1 << 3,
        
        CanCreatePosts = 1 << 4,
        CanEditPost = 1 << 5,
        CanDeletePost = 1 << 6,

        CanCreatePrivateMessages = 1 << 7,

        All = CanCreateTopics | CanEditTopic | CanMoveTopic | CanDeleteTopic |
            CanCreatePosts | CanEditPost | CanDeletePost | CanCreatePrivateMessages,            
    }
}
