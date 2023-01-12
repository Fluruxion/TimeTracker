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
        public TimeSpan timeTaken { get; set; }
        public DateTime lastSaved { get; set; }
        double hoursTaken
        {
            get
            {
                return timeTaken.Hours;
            }
        }
        double minutesTaken
        {
            get
            {
                return timeTaken.Minutes;
            }
        }
        public double totalHoursTaken
        {
            get
            {
                return timeTaken.TotalHours;
            }
        }
        public double totalMinutesTaken
        {
            get
            {
                return timeTaken.TotalMinutes;
            }
        }
        public double totalSecondsTaken
        {
            get
            {
                return timeTaken.TotalSeconds;
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
            timeTaken = new TimeSpan(0);
        }
        public TimeItem(string _name, DateTime _start)
        {
            name = _name;
            start = _start;
            timeTaken = new TimeSpan(0);
        }
        public TimeItem(string _name, DateTime _start, DateTime _end)
        {
            name = _name;
            start = _start;
            end = _end;
            timeTaken = new TimeSpan(0);
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
            timeTaken = _item.timeTaken;
            lastSaved = _item.lastSaved;
        }
        public TimeItem(string _taskName, string _hoursTaken, string _comments, string _dateTimeCompleted)
        {
            name = _taskName;

            double parsedHours = 0;
            if (double.TryParse(_hoursTaken, out parsedHours))
            {
                timeTaken = TimeSpan.FromHours(parsedHours);
            }
            else
            {
                timeTaken = new TimeSpan(0, 0, 0);
            }

            comments = _comments;

            DateTime parsedDateTimeCompleted = new DateTime();
            if (DateTime.TryParse(_dateTimeCompleted, out parsedDateTimeCompleted))
            {
                lastSaved = parsedDateTimeCompleted;
            }
            else
            {
                lastSaved = DateTime.Now;
            }
        }
        public void SetStart(DateTime _start)
        {
            start = _start;
        }
        public void SetEnd(DateTime _end)
        {
            end = _end;
            lastSaved = end;
        }
        public void Finished(bool newPause = true)
        {
            if (newPause) end = DateTime.Now;
            if (timePaused != null && timePaused.TotalSeconds > 0)
            {
                if (timeTaken != null && timeTaken.TotalSeconds > 0) timeTaken = timeTaken.Add(end.Subtract(start).Subtract(timePaused));
                else timeTaken = end.Subtract(start).Subtract(timePaused);
            }
            else
            {
                if (timeTaken != null && timeTaken.TotalSeconds > 0) timeTaken = timeTaken.Add(end.Subtract(start));
                else timeTaken = end.Subtract(start);
            }
            lastSaved = end;
            pauseStart = new DateTime();
            pauseEnd = new DateTime();
            timePaused = new TimeSpan();
            start = new DateTime();
            end = new DateTime();
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
