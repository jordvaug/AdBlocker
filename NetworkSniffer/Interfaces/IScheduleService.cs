namespace NetworkSniffer.Interfaces
{
    public interface IScheduleService
    {
        public int timer { get; set; }
        public void BeginSchedule();
        public void SchedulePrint(object sender, System.Timers.ElapsedEventArgs e);

    }
}
