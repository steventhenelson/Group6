using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ChocAn.Models;

// the purpose of this class is to randomly initialize the database with info for testing

namespace ChocAn.EfData
{
    public class ChocAnContentInitializer : DropCreateDatabaseIfModelChanges<ChocAnDb>
    {
    }
}
