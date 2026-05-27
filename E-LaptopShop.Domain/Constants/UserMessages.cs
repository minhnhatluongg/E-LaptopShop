namespace E_LaptopShop.Domain.Constants
{
    public static class UserMessages
    {
        // Validation
        public const string EmailRequired = "Email cannot be null or empty";
        public const string EmailAlreadyInUse = "Email '{0}' is already in use";
        public const string UserIdMustBePositive = "User ID must be greater than zero";

        // Not found
        public const string UserNotFoundById = "User with ID {0} not found";
        public const string UserNotFoundByEmail = "User with email {0} not found";

        // Error messages
        public const string ErrorRetrievingUser = "Error retrieving user {0}";
        public const string ErrorRetrievingAllUsers = "Error retrieving all users";
        public const string ErrorRetrievingByEmail = "Error retrieving user by email {0}";
        public const string ErrorChangingActiveStatus = "Error changing active status for user {0}";
        public const string ErrorRetrievingFiltered = "Error retrieving filtered users";
        public const string ErrorRetrievingPaged = "Error retrieving paged users";
    }
}
