using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeTracker
{
    class TimeItem
    {
        // For the excel output, ideally col1=name, col2=totalhourstaken, col3=comments OR seperate Hours/Minutes as appropriate
        public string name { get; set; }
        public string comments { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public TimeSpan timePaused { get; set; }
        DateTime pauseStart { get; set; }
        DateTime pauseEnd { get; set; }
        double hoursTaken
        {
            get
            {
                if (timePaused != null && timePaused.Hours > 0) return end.Subtract(start).Hours - timePaused.Hours;
                return end.Subtract(start).Hours;
            }
        }
        double minutesTaken
        {
            get
            {
                if (timePaused != null && timePaused.Minutes > 0) return end.Subtract(start).Minutes - timePaused.Minutes;
                return end.Subtract(start).Minutes;
            }
        }
        public double totalHoursTaken
        {
            get
            {
                if (timePaused != null && timePaused.TotalHours > 0) return end.Subtract(start).TotalHours - timePaused.TotalHours;
                return end.Subtract(start).TotalHours - timePaused.TotalHours;
            }
        }
        public double totalMinutesTaken
        {
            get
            {
                if (timePaused != null && timePaused.TotalMinutes > 0) return end.Subtract(start).TotalMinutes - timePaused.TotalMinutes;
                return end.Subtract(start).TotalMinutes;
            }
        }
        public double totalSecondsTaken
        {
            get
            {
                if (timePaused != null && timePaused.TotalSeconds > 0) return end.Subtract(start).TotalSeconds - timePaused.TotalSeconds;
                return end.Subtract(start).TotalSeconds;
            }
        }
        public string totalTimeTakenHoursAndMinutes
        {
            get
            {
                return string.Format("{0} hours {1} minutes", hoursTaken, minutesTaken);
            }
        }
        public string totalTimeTakenHoursAsDecimal
        {
            get
            {
                return string.Format("{0}h", totalHoursTaken);
            }
        }
        public string variableTimeDisplay
        {
            get
            {
                if (totalSecondsTaken < 60) return String.Format("{0:##} seconds", totalSecondsTaken);
                else if (totalMinutesTaken < 60) return String.Format("{0:##.#} minutes", totalMinutesTaken);
                return String.Format("{0:##.##} hours", totalHoursTaken);
            }
        }
        public TimeItem(string _name)
        {
            name = _name;
            start = DateTime.Now;
        }
        public TimeItem(string _name, DateTime _start)
        {
            name = _name;
            start = _start;
        }
        public TimeItem(string _name, DateTime _start, DateTime _end)
        {
            name = _name;
            start = _start;
            end = _end;
        }
        public TimeItem(TimeItem _item)
        {
            name = _item.name;
            comments = _item.comments;
            start = _item.start;
            end = _item.end;
            pauseStart = _item.pauseStart;
            pauseEnd = _item.pauseEnd;
            timePaused = _item.timePaused;
        }
        public void SetStart(DateTime _start)
        {
            start = _start;
        }
        public void SetEnd(DateTime _end)
        {
            end = _end;
        }
        public void Finished()
        {
            end = DateTime.Now;
        }
        public void ChangeName(string _name)
        {
            name = _name;
        }
        /// <summary>
        /// Bloats the hours by a multiplier, "bloater", indicated by a float parameter
        /// 0.5 halves the hours, 1 keeps it the same, 2 doubles the hours, ect
        /// </summary>
        /// <param name="bloater"></param>
        public void Bloat(float bloater)
        {
            end = start.AddHours(totalHoursTaken * bloater);
        }
        public void StartPause()
        {
            pauseStart = DateTime.Now;
        }
        public void EndPause(bool cancel = false)
        {
            if (cancel)
            {
                pauseStart = new DateTime();
                return;
            }
            if (pauseStart == null || pauseStart == new DateTime())
            {
                MessageBox.Show("pauseStart is null or empty");
                return;
            }
            // Enter code to set timePaused equal to the gap
            pauseEnd = DateTime.Now;
            timePaused = timePaused.Add(pauseEnd.Subtract(pauseStart));
            pauseStart = new DateTime();
            pauseEnd = new DateTime();
        }
        public void RemovePause()
        {
            timePaused = new TimeSpan();
        }
        public void SetComment(string _comments)
        {
            comments = _comments;
        }
    }
}
