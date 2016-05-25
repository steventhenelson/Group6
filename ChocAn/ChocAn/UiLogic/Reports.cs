using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ChocAn.Models;
using ChocAn.Repositories;

namespace ChocAn.UiLogic {
    public class Reports
    {
        public int PrintProviderReport(string path, UserProfile provider) {
            if (path.Length == 0 || provider == null) return 0;

            UnitOfWork workUnit = new UnitOfWork();
            List<TreatmentRecord> records = workUnit.TreatmentRecordRepository.Retrieve().ToList();
            decimal feeToBeTransferred = records.Sum(r => r.Service.Fee);

            using (StreamWriter file = new StreamWriter(path + "\\provider report.txt")) {
                file.WriteLine(provider.FirstName + " " + provider.LastName + ": " + provider.LoginId);
                file.Write("\n" + provider.Address);
                foreach (var record in records) {
                    file.WriteLine();
                    file.WriteLine("Date of service: {0:MM-dd-yyy}", record.TreatmentDate);
                    file.WriteLine("Form submission date/time: {0:MM-dd-yyy hh:mm:ss}", record.EntryDate);
                    file.WriteLine("Member name: " + record.Member.FirstName + " " + record.Member.LastName);
                    file.WriteLine("Member number: " + record.Member.LoginId);
                    file.WriteLine("Service code: " + record.Service.ServiceCode);
                    file.WriteLine("Fee: " + record.Service.Fee);
                }

                file.WriteLine();
                file.WriteLine("Total consulations: " + records.Count);
                file.WriteLine("Total fee for the week: {0:C}", feeToBeTransferred);
            }

            writeEftTransaction(provider, feeToBeTransferred);
            return 1;
        }

        // method to write EFT transaction log. accepts the profile of the prvider sending the report and the total fee
        // writes to the base program debug directory
        private void writeEftTransaction(UserProfile provider, decimal amountToBeTransfered) {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\EFT Transaction Log.txt";

            using (StreamWriter file = File.AppendText(path)) {
                file.WriteLine(provider.FullName + ":" + provider.LoginId + ":" + amountToBeTransfered);
            }
        }

        public int PrintSummaryReport(string path) {
            if (String.IsNullOrEmpty(path)) return 0;
            UnitOfWork workUnit = new UnitOfWork();

            List<UserProfile> providers = workUnit.UserProfileRepository.Retrieve().ToList();

            using (StreamWriter file = new StreamWriter(path + "\\summary report.txt")) {
                foreach (var provider in providers) {
                    //DateTime now = DateTime.Now;
                    //DateTime start = now.AddDays(-(int)now.DayOfWeek);
                    //DateTime end = now.AddDays(7);

                    List<TreatmentRecord> records = workUnit.TreatmentRecordRepository.Retrieve().Where(r => r.Provider.UserId == provider.UserId).ToList();

                    file.WriteLine("Provider: " + provider.FirstName + " " + provider.LastName);
                    file.WriteLine("Provider Number: " + provider.LoginId);

                    file.WriteLine("Total consulations: " + records.Count);
                    file.WriteLine("Total fee for the week: {0:C}", records.Sum(r => r.Service.Fee));
                    file.WriteLine();
                }
            }

            return 1;
        }

        public int PrintMemberReport(string path, UserProfile member) {
            if (path.Length == 0) return 0;
            DateTime now = DateTime.Now;
            DateTime start = now.AddDays(-(int)now.DayOfWeek);
            DateTime end = now.AddDays(7);

            using (StreamWriter writer = new StreamWriter(path + "\\member report.txt")) {
                writer.WriteLine(member.FirstName + " " + member.LastName + ": " + member.LoginId);
                writer.Write("\n" + member.Address);

                List<TreatmentRecord> records = new UnitOfWork().TreatmentRecordRepository.Retrieve()
                    .Where(r => 
                        r.Member.UserId == member.UserId &&
                        r.TreatmentDate >= start &&
                        r.TreatmentDate <= end)
                    .OrderBy(r => r.TreatmentDate)
                    .ToList();

                foreach (var record in records) {
                    writer.WriteLine();
                    writer.WriteLine("Treatment date: {0:MM-dd-yyy}", record.TreatmentDate);
                    writer.WriteLine("Provider: " + record.Provider.FirstName + " " + record.Provider.LastName);
                    writer.WriteLine("Service: " + record.Service.ServiceName);
                }
            }

            return 1;
        }
    }
}
