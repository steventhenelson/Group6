using ChocAn.Models;
using ChocAn.Repositories;

namespace ChocAn.UiLogic
{
    public class UserSession
    {
        private UserProfile _currentUser;
        public UserProfile CurrentUser => _currentUser;

        public void Login() {
            UiTools tools = new UiTools();

            // <--- A fancy console UX should be displayed here

            // The user will enter a User ID number to login. The database will fill _currentUser if a valid ID is entered with the 
            // correct information as necessary. The necessary menu will be displayed depending on the Role ID.
            _currentUser = new UnitOfWork().UserProfileRepository.GetEntityById(tools.GetUserIdFromUser("Please enter in your User ID to login: "));
            if (_currentUser == null) return;

            switch (_currentUser.RoleId) {
                case 2:
                    ProviderUi providerUi = new ProviderUi(this);
                    providerUi.Start();
                    break;
                case 3:
                    OperatorUi operatorUi = new OperatorUi(this);
                    operatorUi.Start();
                    break;
                case 4:
                    ManagerUi managerUi = new ManagerUi(this);
                    managerUi.Start();
                    break;
            }
        }
    }
}

