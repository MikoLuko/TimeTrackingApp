namespace TimeTrackingApp.Models.ViewModel
{
    public class EditViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public List<string> AllRoles { get; set; }
    }

}
