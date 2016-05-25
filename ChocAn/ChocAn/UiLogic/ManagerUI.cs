using System;

namespace ChocAn.UiLogic
{
    public class ManagerUi : UiTools
    {
        private UserSession _session;

        public ManagerUi(UserSession session)
        {
            if (session.CurrentUser.RoleId != 4) throw new InvalidRoleException();
            _session = session;
        }

        // controls the managers main menu accepts and returns nothing whatsoever
        public void Start()
        {
            bool repeat = true;

            while (repeat)
            {
                Console.Clear();
                Console.WriteLine("Manager UI");
                Console.WriteLine("1. Print summary report");
                Console.WriteLine("2. Logout");

                switch (GetIntegerFromUser(1, 2))
                {
                    case 1:
                        Reports report = new Reports();
                        report.PrintSummaryReport(GetFilePathFromUser());
                        break;
                    case 2:
                        repeat = false;
                        break;
                }
            }
        }
    }
}
