namespace TimeTrackingApp.Models.Entities
{
    public class EmailLog
    {
        public int id { get; set; }
        public string recipientemail { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public DateTime sentat { get; set; }
        public bool issuccess { get; set; }
        public string? errormessage { get; set; }
    }
}
