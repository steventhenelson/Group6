using System;
using System.Collections.Generic;
using System.Linq;
using ChocAn.Models;
using ChocAn.Repositories;

namespace ChocAn.UiLogic
{
    public class ProviderUi : UiTools
    {
        private readonly UserSession _session;
        private readonly UnitOfWork _workUnit;

        public ProviderUi(UserSession session)
        {
            if (session.CurrentUser.RoleId != 2) throw new InvalidRoleException();
            _session = session;
            _workUnit = new UnitOfWork();
        }

        // method to process provider main menu
        public void Start()
        {
            bool repeat = true;
            string[] options = {
                "Provider UI",
                "1. View services",
                "2. Add new treatment record",
                "3. Edit existing treatment record",
                "4. View provider report",
                "5. Logout "
            };

            Console.Clear();
            while (repeat)
            {
                Console.WriteLine();
                switch (GetMenuOptionFromUser(options))
                {
                    case 1:
                        ViewServices();
                        break;
                    case 2:
                        CreateNewTreatmentRecord();
                        break;
                    case 3:
                        EditExistingRecord();
                        break;
                    case 4:
                        PrintReport();
                        break;
                    case 5:
                        repeat = false;
                        break;
                }
            }
        }

        // method to print providers report
        private void PrintReport()
        {
            Reports reports = new Reports();
            reports.PrintProviderReport(GetFilePathFromUser(), _session.CurrentUser);
        }

        // method for a provider to add a new record to the db
        private bool CreateNewTreatmentRecord()
        {
            bool commit = false, repeat = true;
            TreatmentRecord record = new TreatmentRecord();

            Console.Clear();
            while (repeat)
            {
                UserProfile member = _workUnit.UserProfileRepository.GetEntityById(GetUserIdFromUser("Enter member id: "));

                if (member != null)
                {
                    if (!member.Suspended)
                    {
                        record = EnterRecordData(record);

                        if (Continue("Confirm treatment record: " + record + " [y/n]"))
                        {
                            repeat = false;
                            commit = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Member has been suspended from receiving services.");
                        repeat = false;
                    }
                }
            }

            // save to db
            if (commit)
            {
                _workUnit.TreatmentRecordRepository.AddEntity(record);
                commit = _workUnit.SaveChanges();
            }
            return commit;
        }

        // method to enter treatment record data
        private TreatmentRecord EnterRecordData(TreatmentRecord record)
        {
            record.TreatmentDate = GetDateFromUser("Enter treatment date", record.EntryDateString);
            record.Service = _workUnit.ServiceRepository.GetEntityById(GetServiceIdFromUser("Enter service code: ", record.Service.ServiceCode));
            record.Comments = GetStringFromUser("Comments: ", record.Comments);

            return record;
        }

        // method to for provider to edit existing records
        private bool EditExistingRecord()
        {
            bool repeat = true, commit = false;
            TreatmentRecord record = new TreatmentRecord();

            Console.Clear();
            while (repeat)
            {
                record = _workUnit.TreatmentRecordRepository.GetEntityById(GetTreatmentRecordIdFromUser("Enter record id: "));
                Console.WriteLine(record);

                // query the user again to edit record data and proceed if yes
                if (Continue("Edit this treatment record? [y/n]: "))
                {
                    repeat = false;
                    record = EnterRecordData(record);
                }
                else repeat = false;

                // validate changes
                if (Continue("Confirm treatment record: " + record + " [y/n]"))
                {
                    commit = true;
                }
            }

            // save to db
            if (commit)
            {
                _workUnit.TreatmentRecordRepository.UpdateEntity(record);
                commit = _workUnit.SaveChanges();
            }

            return commit;
        }

        // method for the provider to view all available services
        private void ViewServices()
        {
            Console.Clear();
            List<Service> services = _workUnit.ServiceRepository.Retrieve()
                .Where(s => s.Enabled)
                .OrderBy(s => s.ServiceName)
                .ToList();

            foreach (var service in services)
            {
                Console.WriteLine(service);
            }
        }

        // TODO: Add delete record for provider
    }
}