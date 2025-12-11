using TimeTrackingApp.Models.Entities;
namespace TimeTrackingApp.Models.ViewModels
{
    public class EmpPanelViewModel
    {
        public bool IsWorking { get; set; }
        public TimeEntry? CurrentEntry { get; set; }
    }
}

