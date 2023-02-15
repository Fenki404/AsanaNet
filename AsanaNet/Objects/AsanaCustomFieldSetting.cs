namespace AsanaNet.Objects
{
    public class AsanaCustomFieldSetting: AsanaReference
    {
        [AsanaDataAttribute("project")]
        public AsanaReference Project { get; set; }

        [AsanaDataAttribute("is_important")]
        public bool IsImportant { get; set; }

        [AsanaDataAttribute("parent")]
        public AsanaReference Parent { get; set; }

        [AsanaDataAttribute("custom_field")]
        public AsanaCustomField CustomField { get; set; }
    }
}