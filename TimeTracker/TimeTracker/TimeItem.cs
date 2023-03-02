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
        /// <summary>
        /// Sets the start DateTime equal to whatever DateTime is passed in through the _start parameter
        /// </summary>
        /// <param name="_start"></param>
        public void SetStart(DateTime _start)
        {
            start = _start;
        }
        /// <summary>
        /// Sets the end DateTime equal to whatever DateTime is passed in through the _end parameter.
        /// Also sets "lastSaved" to the same value.
        /// </summary>
        /// <param name="_end"></param>
        public void SetEnd(DateTime _end)
        {
            end = _end;
            lastSaved = end;
        }
        /// <summary>
        /// Method for finishing a task.
        /// Calculates time taken by creating a timespan equal to the difference between start and end, then subtracting the value of timepaused.
        /// Then clears the values of the DateTimes and TimeSpans other than lastSaved as we do not want stale data being stored.
        /// If TimeTaken is null or empty, sets it to a new TimeSpan value, otherwise it adds to the current value.
        /// </summary>
        /// <param name="newPause">When set to false will not set the end DateTime, this means the end DateTime can be set separately if required. Defaults to True.</param>
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
        /// 'Bloats' the hours by a multiplier, "bloater", indicated by a float parameter
        /// This was originally intended as a way of modifying the time for one very specific situation but never ended up seeing any use.
        /// As such, this was never implemented into the UI. Leaving code present in case it is required in the future, though it's unlikely to occur.
        /// </summary>
        /// <param name="bloater">Parameter indicating the multiplier to be applied to the task: 0.5 halves the hours, 1 keeps it the same, 2 doubles the hours, ect.</param>
        public void Bloat(float bloater)
        {
            end = start.AddHours(totalHoursTaken * bloater);
        }
        public void StartPause()
        {
            pauseStart = DateTime.Now;
        }
        /// <summary>
        /// Ends the current pause and creates a timeSpan to represent the time the task was paused for.
        /// This is done to accurately calculate the time taken to complete the task without including breaks or distractions.
        /// </summary>
        /// <param name="cancel">if true, cancels the pause entirely by removing the pauseStart DateTime. Currently unused.</param>
        public void EndPause(bool cancel = false)
        {
            if (cancel)
            {
                pauseStart = new DateTime();
                return;
            }
            if (pauseStart == null || pauseStart == new DateTime())
            {
                // This should not occur, but leaving a messagebox just so it doesn't fail silently if it ever were to occur. Returns to prevent crashes.
                MessageBox.Show("pauseStart is null or empty");
                return;
            }
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
