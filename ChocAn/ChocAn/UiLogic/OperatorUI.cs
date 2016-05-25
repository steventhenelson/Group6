using System;
using System.Linq;
using ChocAn.Models;
using ChocAn.Repositories;

namespace ChocAn.UiLogic
{
    public class OperatorUi : UiTools
    {
    private UserSession _session;
        private UnitOfWork _workUnit;

        public OperatorUi(UserSession session) {
            if (session.CurrentUser.RoleId != 3) throw new InvalidRoleException();
            _session = session;
        }

        // Operators main menu
        public void Start() {
            _workUnit = new UnitOfWork();

            bool repeat = true;

            string[] options = {
                "Operator UI",
                "1. manage members and providers",
                "2. Manage services",
                "3. Logout"
            };

            while (repeat) {
                switch (GetMenuOptionFromUser(options)) {
                    case 1:
                        UserCrudMenu();
                        break;
                    case 2:
                        UserCrudMenu();
                        break;
                    case 3:
                        ServicesCrudMenu();
                        break;
                    case 4:
                        repeat = false;
                        break;
                }
            }
        }

        // CRUD menu for user profile operations
        private void UserCrudMenu() {
            bool repeat = true;
            string[] options = {
                "Member and Provider Operations",
                "1. Add new",
                "2. View",
                "3. Edit existing",
                "4. Change status",
                "5. Exit"
            };

            Console.Clear();
            while (repeat) {
                switch (GetMenuOptionFromUser(options)) {
                    case 1:
                        CreateNewUser();
                        break;
                    case 2:
                        ViewUserData();
                        break;
                    case 3:
                        EditExistingUser();
                        break;
                    case 4:
                        ChangeUserStatus();
                        break;
                    case 5:
                        repeat = false;
                        break;
                }
            }
        }

        // method to create a new user. accepts an integer to represent the type of user to be created
        // return a true/false success code
        private bool CreateNewUser() {
            UserProfile newProfile = new UserProfile();
            bool repeat = true, commit = false;

            // enter profile data
            while (repeat) {
                newProfile = EnterProfileData(newProfile);

                // validate changes
                if (Continue("Confirm member information \n" + newProfile + "\n[y/n]: ")) {
                    repeat = false;
                    commit = true;
                }
                else if (!Continue("Try again? [y/n]: ")) repeat = false;
            }

            // save to db
            if (commit) {
                _workUnit.UserProfileRepository.AddEntity(newProfile);
                commit = _workUnit.SaveChanges();
            }

            return commit;
        }

        // method used to enter profile data. accepts the user profile to be edited and a bool to determine
        // whether or not this is an edit to existing data. if true, the address entries will display
        // returns the adjusted profile
        private UserProfile EnterProfileData(UserProfile profile, bool editAddress = false) {
            profile.FirstName = GetStringFromUser("First name: ", profile.FirstName);
            profile.LastName = GetStringFromUser("Last name: ", profile.LastName);
            profile.RoleId = GetIntegerFromUser(1, 2, "Enter Role [1 for user/2 for provider]: ",
                profile.RoleId.ToString());

            if (editAddress) {
                profile.Address.AddressLineOne = GetStringFromUser("Address line one: ", profile.Address.AddressLineOne);
                profile.Address.AddressLineTwo = GetStringFromUser("Address line two: ", profile.Address.AddressLineTwo);
                profile.Address.City = GetStringFromUser("City: ", profile.Address.City);
                profile.Address.State = GetStateFromUser("Two character state [NM]: ", profile.Address.State);
                profile.Address.Zip = GetIntegerFromUser(0, 99999, "Zip code: ", profile.Address.ZipToString);
            }

            return profile;
        }

        // method for operator edit an existing user
        private bool EditExistingUser() {
            bool repeat = true, commit = false;
            UserProfile profile = new UserProfile();

            Console.Clear();
            while (repeat) {
                // only allow the edit of members and providers
                do {
                    profile = _workUnit.UserProfileRepository.GetEntityById(GetUserIdFromUser("Enter user id of member to edit: "));
                } while (profile.RoleId == 1 || profile.RoleId == 2);
                Console.WriteLine(profile);

                // query to edit the user data and proceed if yes
                if (Continue("Edit this user? [y/n]: ")) {
                    repeat = false;
                    profile = EnterProfileData(profile, Continue("Edit address? [y/n]: "));
                }
                else repeat = false;

                // validate changes
                if (Continue("Confirm member information \n" + profile + "\n[y/n]: ")) {
                    commit = true;
                }
            }

            // save to db
            if (commit) {
                _workUnit.UserProfileRepository.UpdateEntity(profile);
                commit = _workUnit.SaveChanges();
            }

            return commit;
        }

        private void ViewUserData() {
            Console.Clear();
            do {
                Console.Write(_workUnit.UserProfileRepository.GetEntityById(GetUserIdFromUser("Enter user id: ")));
                Console.WriteLine();
            } while (Continue("Display more? [y/n]: "));
        }

        // method for switching the users suspension status
        private bool ChangeUserStatus() {
            UserProfile profile;
            do {
                profile = _workUnit.UserProfileRepository.GetEntityById(GetUserIdFromUser("Enter user id of member to edit: "));
            } while (profile != null && (profile.RoleId == 1 || profile.RoleId == 2));

            if (profile != null && Continue(profile.Suspended ? "Enable user? [y/n]" : "Suspend user? [y/n]: ")) {
                profile.Suspended = !profile.Suspended;
                _workUnit.UserProfileRepository.UpdateEntity(profile);
            }

            return _workUnit.SaveChanges();
        }

        // operators crud menu for services
        private void ServicesCrudMenu() {
            bool repeat = true;
            string[] options = {
                "Service Operations",
                "1. Add new",
                "2. View",
                "3. Edit existing",
                "4. Enable or disable",
                "5. Exit"
            };

            Console.Clear();
            while (repeat) {
                switch (GetMenuOptionFromUser(options)) {
                    case 1:
                        CreateNewService();
                        break;
                    case 2:
                        ViewService();
                        break;
                    case 3:
                        EditExistingService();
                        break;
                    case 4:
                        DisableService();
                        break;
                    case 5:
                        repeat = false;
                        break;
                }
            }
        }

        // method for an operator to add a new service
        private bool CreateNewService() {
            Service service = new Service();
            bool repeat = true, commit = false;

            // continue to prompt for 
            while (repeat) {
                do {
                    service.ServiceName = GetStringFromUser("Service name: ");
                    service.Fee = GetCurrencyFromUser(0, 1000, "Service fee: ");
                } while (_workUnit.ServiceRepository.Retrieve().First(s => s.ServiceName == service.ServiceName) != null);

                if (Continue("Confirm service information: " + service + " [y/n]: ")) {
                    repeat = false;
                    commit = true;
                }
                else if (!Continue("Try again? [y/n]: ")) repeat = false;
            }

            // save to db
            if (commit) {
                _workUnit.ServiceRepository.AddEntity(service);
                commit = _workUnit.SaveChanges();
            }

            return commit;
        }

        // method to allow operators ability to edit a service
        private bool EditExistingService() {
            bool repeat = true, commit = false;
            Service service = new Service();

            Console.Clear();
            while (repeat) {
                service = _workUnit.ServiceRepository.GetEntityById(GetServiceIdFromUser("Enter service id to edit: "));

                Console.WriteLine(service);
                if (Continue("Edit this service? [y/n]: ")) {
                    repeat = false;
                    service.ServiceName = GetStringFromUser("Service name: ", service.ServiceName);
                    service.Fee = GetCurrencyFromUser(0, 1000, "Service fee: ", service.FeeToString);
                }

                if (Continue("Confirm service information: " + service + " [y/n]: ")) {
                    repeat = false;
                    commit = true;
                }
                else if (!Continue("Try again? [y/n]: ")) repeat = false;
            }

            if (commit) {
                _workUnit.ServiceRepository.UpdateEntity(service);
                commit = _workUnit.SaveChanges();
            }
            return commit;
        }

        // method for the operator to view services
        private void ViewService() {
            do {
                Console.WriteLine();
                Console.WriteLine(_workUnit.ServiceRepository.GetEntityById(GetServiceIdFromUser("Enter service id: ")));
            } while (Continue("Display another? [y/n]: "));
        }

        // method for the operator to enable or disable a service
        private bool DisableService() {
            Service service = _workUnit.ServiceRepository.GetEntityById(GetServiceIdFromUser("Enter service id: "));

            if (service != null && Continue(service.Enabled ? "Enable service? [y/n]" : "Disable service? [y/n]: ")) {
                service.Enabled = !service.Enabled;
                _workUnit.ServiceRepository.UpdateEntity(service);
            }

            return _workUnit.SaveChanges();
        }
    }
}
