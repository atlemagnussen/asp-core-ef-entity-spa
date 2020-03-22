namespace Test.model.Users
{
    public class ListUserViewModel
    {
        public ListUserViewModel(ApplicationUser applicationUser)
        {
            Name = applicationUser.FullName;
            Email = applicationUser.Email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
