namespace Test.model.Users
{
    public class ListUserViewModel
    {
        public ListUserViewModel(ApplicationUser applicationUser, bool isAdmin = false)
        {
            Name = applicationUser.FullName;
            Email = applicationUser.Email;
            IsAdmin = isAdmin;
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
