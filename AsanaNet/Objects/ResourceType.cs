namespace AsanaNet.Objects
{
    public static class ResourceType
    {
        public static string Assigned => "assigned";
        public static string AddedToProject => "added_to_project";
        public static string DueDateChanged => "due_date_changed";
        public static string CommentAdded => "comment_added";
        public static string DependentAdded => "dependent_added";

        public static string Attachment => "attachment";
        public static string CustomFieldSetting => "custom_field_setting";
        public static string CustomField => "custom_field";
        public static string Event => "event";
        public static string Job => "job";
        public static string OrganizationExport => "organization_export";
        public static string Portfolio => "portfolio";
        public static string PortfolioMembership => "portfolio_membership";

        public static string Project => "project";
        public static string ProjectMembership => "project_membership";
        public static string ProjectStatus => "project_status";

        public static string Section => "section";
        public static string Story => "story";
        public static string Tag => "tag";

        public static string Task => "task";
        public static string Team => "Team";

        public static string User => "user";
        public static string UserTaskList => "user_task_list";
        public static string EnumOption => "enum_option";

        public static string Webhook => "webhook";
        public static string Workspace => "workspace";
        public static string WorkspaceMembership => "workspace_membership";
    }
}