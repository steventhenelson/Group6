using System;
using System.Data.Entity;
using ChocAn.EfData;
using ChocAn.UiLogic;

namespace ChocAn
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Database.SetInitializer(new ChocAnContentInitializer());
            ChocAnDb contentContext = new ChocAnDb();
            contentContext.Database.Initialize(true);
            
            UserSession userSession = new UserSession();

            // userSession.Login();
        }
    }
}