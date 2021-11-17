using System.Collections.Generic;

namespace AwesomeBot.Models
{
    public class LanguageOption
    {
        public string KnowledgeBaseId { get; set; }
        public string Host { get; set; }
        public string EndpointKey { get; set; }
        public string Project { get; set; }
        public string Deployment { get; set; }
    }

    public class LuisOption
    {
        public string ApplicationId { get; set; }
        public string Host { get; set; }
        public string EndpointKey { get; set; }
    }

    public class TranslationOption
    {
        public string Endpoint { get; set; }
        public string EndpointKey { get; set; }
        public string EndpointRegion { get; set; }
    }
}