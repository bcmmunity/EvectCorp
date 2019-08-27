using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public abstract class DB 
    {
        protected ApplicationContext Connect()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            return new ApplicationContext(optionsBuilder.Options);
        }
        
        
        
        
    }
}