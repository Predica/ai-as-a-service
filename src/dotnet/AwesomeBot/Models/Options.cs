namespace AwesomeBot.Models
{
    public class QnAMakerOption
    {
        public string KnowledgeBaseId { get; set; }
        public string Host { get; set; }
        public string EndpointKey { get; set; }
        
    }

    public class LuisOption
    {
        public string ApplicationId { get; set; }
        public string Host { get; set; }
        public string EndpointKey { get; set; }
        
    }
}