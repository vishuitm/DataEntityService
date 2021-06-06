
namespace DataEntityService.Entities
{
    using System;

    public class OpportunityData
    {
        public Guid OpportunitySourceId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OpportunitySourceCode { get; set; }
        public string OpportunitySourceName { get; set; }
        public string OpportunitySourceDescription { get; set; }
        public bool Active { get; set; }
        public bool Void { get; set; }
        public Guid UserId { get; set; }
    }
}
