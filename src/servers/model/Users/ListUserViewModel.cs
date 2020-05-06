namespace Test.model.Users
{
    public class ListUserViewModel
    {
        public ListUserViewModel(ApplicationUser applicationUser, bool isAdmin = false)
        {
            Id = applicationUser.Id;
            Name = applicationUser.FullName;
            Email = applicationUser.Email;
            IsAdmin = isAdmin;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
