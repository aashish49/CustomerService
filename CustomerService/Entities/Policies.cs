using Microsoft.AspNetCore.Authorization;

namespace CustomerService.Entities
{
    public class Policies
    {
        public const string ADMIN_ROLE = "Admin";
        public const string USER_ROLE = "User";
        public static AuthorizationPolicy GetAdminPolicy ()
        {
            return
                new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(ADMIN_ROLE)
                .Build();
        }
        public static AuthorizationPolicy GetUserPolicy ()
        {
            return
                new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(USER_ROLE)
                .Build();
        }
    }
}
