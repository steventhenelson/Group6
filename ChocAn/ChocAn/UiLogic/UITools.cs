using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ChocAn.Repositories;

namespace ChocAn.UiLogic {
    public class UiTools {
        // Method to read a menu choice from the user. Int min is the lowest number of user entry options. Int max
        // is the upper bound of options for the menu being used and the string is used to display as a prompt.
        // If no string is given, it defaults to ": " and returns the valid choice selected by the user.
        protected int GetIntegerFromUser(int min = 0, int max = 0, string message = ": ", string autofill = "") {
            int choice = 0;
            bool repeat = true;

            // loop until a valid entry has been parsed from the user
            while (repeat) {
                Console.Write(message);
                if (!string.IsNullOrEmpty(autofill)) SendKeys.SendWait(autofill);
                var readLine = Console.ReadLine();
                if (readLine != null && (int.TryParse(readLine.Trim(), out choice) &&
                            choice >= min && choice <= max)) {
                    repeat = false;
                }
            }
            return choice;
        }

        // Method to get a currency value from the user. Decimal min is the lowest value, max is the highest. The first string
        // is the prompt message and the second is an auto-fill. Both strings are optional. Returns the decimal.
        protected decimal GetCurrencyFromUser(decimal min = 0, decimal max = 1000, string message = ": ",
                string autofill = "") {
            decimal choice = 0;
            bool repeat = true;

            while (repeat) {
                if (!String.IsNullOrEmpty(autofill)) SendKeys.SendWait(autofill);
                var readLine = Console.ReadLine();
                if (readLine != null && (decimal.TryParse(readLine.Trim(), out choice) &&
                            choice >= min && choice <= max)) {
                    repeat = false;
                }
            }
            return choice;
        }

        // Method to get a folder save location from the user. Accepts no arguments and returns the chosen path.
        protected string GetFilePathFromUser() {
            bool repeat = true;
            string path = "";

            while (repeat) {
                try {
                    // Set up a new folder browser dialog and check security permissions
                    FolderBrowserDialog browser = new FolderBrowserDialog();

                    if (browser.ShowDialog() == DialogResult.OK) {
                        path = browser.SelectedPath;

                        // This is to check security access for file directory. this will throw 
                        // the UnauthorizedAccessException if the user doesn't have access
                        DirectorySecurity ds = Directory.GetAccessControl(path);
                        repeat = false;
                    }
                }
                catch (UnauthorizedAccessException) {
                    repeat = true;
                    Console.WriteLine("You do not have access to write to this location.");
                }
            }

            return path;
        }

        // Method to get a valid user id from the console. Accepts a message to display and returns the user id if it
        // is valid and exits in the database.
        public int GetUserIdFromUser(string message = ": ", string autofill = "") {
            int result = 0;
            bool repeat = true;

            while (repeat) {
                Console.Write(message);
                string entry = Console.ReadLine();
                if (!string.IsNullOrEmpty(autofill)) SendKeys.SendWait(autofill);

                if (entry != null && 
                        (entry.Length == 9 && int.TryParse(entry.Trim('0'), out result)) && 
                        new UnitOfWork().UserProfileRepository.Retrieve().First(profile => profile.UserId == result) != null
                   ) {
                    repeat = false;
                }
                else {
                    repeat = Continue("An invalid user id was entered. Try again? [y/n]: ");
                }
            }
            return result;
        }

        // Method to get a valid service id from the console. Accepts a string for prompt message and a string for auto-fill;
        // both being optional. Returns the service id if it exits.
        protected int GetServiceIdFromUser(string message = ": ", string autofill = "") {
            int result = 0;
            bool repeat = true;

            while (repeat) {
                Console.WriteLine(message);
                string entry = Console.ReadLine();
                if (!string.IsNullOrEmpty(autofill)) SendKeys.SendWait(autofill);

                if (entry != null && 
                        entry.Length == 6 && int.TryParse(entry.Trim('0'), out result) &&
                        new UnitOfWork().ServiceRepository.Retrieve().First(service => service.ServiceId == result) != null) {
                    repeat = false;
                }
                else {
                    repeat = Continue("An invalid service id was entered. Try again? [y/n]: ");
                }
            }
            return result;
        }

        // Method to get a valid service id from the console. Accepts a string for prompt message and returns the 
        // service id 
        protected int GetTreatmentRecordIdFromUser(string message = ": ") {
            int result = 0;
            bool repeat = true;

            while (repeat) {
                Console.WriteLine(message);
                string entry = Console.ReadLine();

                if (entry != null && 
                        int.TryParse(entry.Trim(), out result) &&
                        new UnitOfWork().TreatmentRecordRepository.Retrieve().First(record => record.TreatmentRecordId == result) != null) {
                    repeat = false;
                }
                else {
                    repeat = Continue("An invalid record id was entered. Try again? [y/n]: ");
                }
            }
            return result;
        }

        // Method to query user to repeat on invalid entry. Accepts a string for display message and returns true if the user 
        // wants to repeat the operation
        protected bool Continue(string message = "") {
            string entry = "";

            while (entry != "Y" && entry != "N") {
                Console.Write(message);
                var readLine = Console.ReadLine();
                if (readLine != null) entry = readLine.Trim().ToUpper();
            }

            return entry == "Y";
        }

        // Method to query the user for a string input. Accepts an optional string as a display message and
        // an optional string as an auto-fill. Returns the string entered by the user
        protected string GetStringFromUser(string message = ": ", string autofill = "") {
            Console.Write(message);
            if (!string.IsNullOrEmpty(autofill)) SendKeys.SendWait(autofill);

            var readLine = Console.ReadLine();
            if (readLine != null) return readLine.Trim();
            else return "";
        }

        // Method to get a valid state input from the user. Accepts a string for prompt message and optional string for auto-fill.
        // Returns the state in all caps
        protected string GetStateFromUser(string message = ": ", string autofill = "") {
            string[] states = {
                "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "HI", "IA", "ID", "IL",
                "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ",
                "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI",
                "WV", "WY"
            };
            string state;

            do {
                state = GetStringFromUser(message, autofill).ToUpper();
            } while (!states.Contains(state));

            return state;
        }

        // Method to display standard CRUD menu. Accepts an array of strings as to display. The first
        // array index represents the title. The remaining represent user options.
        // Returns the integer selected
        protected int GetMenuOptionFromUser(string[] options) {
            if (options.Length == 0) return 0;

            foreach (string t in options) {
                Console.WriteLine(t);
            }

            return GetIntegerFromUser(1, options.Length);
        }

        // Method to get a valid date from user input. Accepts a string for message prompt and a string for auto-fill. Both
        // are optional. Returns the date
        protected DateTime GetDateFromUser(string message = ": ", string autofill = "") {
            //Regex regex =
            //    new Regex(
            //            @"^(((((((0?[13578])|(1[02]))[\.\-/]?((0?[1-9])|([12]\d)|(3[01])))|(((0?[469])|(11))[\.\-/]?((0?[1-9])|([12]\d)|(30)))|((0?2)[\.\-/]?((0?[1-9])|(1\d)|(2[0-8]))))[\.\-/]?(((19)|(20))?([\d][\d]))))|((0?2)[\.\-/]?(29)[\.\-/]?(((19)|(20))?(([02468][048])|([13579][26])))))$");
            DateTime result;
            string dateString;

            do {
                dateString = GetStringFromUser(message + " [DD-MM-YYYY]: ", autofill);
            } while (/*!regex.Match(dateString).Success || */!DateTime.TryParse(dateString, out result));

            return result;
        }
    }
}
