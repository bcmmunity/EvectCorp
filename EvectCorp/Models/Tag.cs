using System.Collections.Generic;
using Evect.Models;

namespace EvectCorp.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public Tag ParentTag { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public List<UserTag> UserTags { get; set; }
        
    }
}