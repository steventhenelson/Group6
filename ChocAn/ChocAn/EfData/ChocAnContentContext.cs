using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ChocAn.Models;

// this class will make any specific SQL table properties you might want to force set.
// entity framework will see the foreign keys and modeling properties from the ChocAn.Models
// and set up the SQL relationships accordingly. Very rarely does EF fail to create this 
// relationships correctly but if it does, this is the class where you would set the relationships in stone

namespace ChocAn.EfData
{
    public class ChocAnDb : DbContext
    {      
       
    }
}
