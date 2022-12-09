﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace TimeTracker
{
    class MainWindowViewModel : ObservableObject
    {
        DispatcherTimer dispatcherTimer { get; set; }
        private TimeItem _currentTask { get; set; }
        private List<TimeItem> _loggedTasks { get; set; }
        private bool _isPaused { get; set; }
        private string _inputName { get; set; }
        private string _inputComments { get; set; }
        private bool _active { get; set; }
        public bool active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                OnPropertyChanged("active");
            }
        }
        private ICommand _CommandPause { get; set; }
        private ICommand _CommandStart { get; set; }
        private ICommand _CommandFinish { get; set; }
        private ICommand _CommandCancel { get; set; }
        private ICommand _CommandDelete { get; set; }
        private ICommand _CommandSave { get; set; }
        public string timer
        {
            get
            {
                // Using actual time as the Dispatch timer doesn't appear to trigger exactly as expected which results in a wobbly timer unless checking datetime.now and comparing to start time
                if (currentTask == null) return "0 Hours\n0 Minutes";

                TimeSpan test = DateTime.Now.Subtract(currentTask.start);
                if (currentTask.timePaused != null) test = test.Subtract(currentTask.timePaused);
                
                return string.Format("{0:#0} Hours\n{1:#0} Minutes", test.Hours, test.Minutes);
            }
        }
        public ICommand CommandPause
        {
            get
            {
                if (_CommandPause == null)
                {
                    _CommandPause = new DelegateCommand(p => PauseTask());
                }

                return _CommandPause;
            }
        }
        public ICommand CommandStart
        {
            get
            {
                if (_CommandStart == null)
                {
                    _CommandStart = new DelegateCommand(p => StartTask());
                }

                return _CommandStart;
            }
        }
        public ICommand CommandFinish
        {
            get
            {
                if (_CommandFinish == null)
                {
                    _CommandFinish = new DelegateCommand(p => FinishTask());
                }

                return _CommandFinish;
            }
        }
        public ICommand CommandCancel
        {
            get
            {
                if (_CommandCancel == null)
                {
                    _CommandCancel = new DelegateCommand(p => CancelTask());
                }

                return _CommandCancel;
            }
        }
        public ICommand CommandDelete
        {
            get
            {
                if (_CommandDelete == null)
                {
                    _CommandDelete = new DelegateCommand<string>(p => DeleteLoggedTask(p));
                }

                return _CommandDelete;
            }
        }
        public ICommand CommandSave
        {
            get
            {
                if (_CommandSave == null)
                {
                    _CommandSave = new DelegateCommand(p => SaveCSV());
                }

                return _CommandSave;
            }
        }
        public TimeItem currentTask {
            get
            {
                return _currentTask;
            }
            set
            {
                _currentTask = value;
                OnPropertyChanged("currentTask");
            }
        }
        public List<TimeItem> loggedTasks
        {
            get
            {
                return _loggedTasks;
            }
            set
            {
                _loggedTasks = value;
                OnPropertyChanged("loggedTasks");
            }
        }
        public bool isPaused
        {
            get
            {
                return _isPaused;
            }
            set
            {
                _isPaused = value;
                OnPropertyChanged("isPaused");
            }
        }
        public string inputName
        {
            get
            {
                return _inputName;
            }
            set
            {
                _inputName = value;
                OnPropertyChanged("inputName");
            }
        }
        public string inputComments
        {
            get
            {
                return _inputComments;
            }
            set
            {
                _inputComments = value;
                OnPropertyChanged("inputComments");
            }
        }

        public MainWindowViewModel()
        {
            isPaused = false;
            loggedTasks = new List<TimeItem>();
            /*loggedTasks.Add(new TimeItem("TaskName", DateTime.Now, DateTime.Now.AddMinutes(30)));
            loggedTasks.Add(new TimeItem("TaskName2", DateTime.Now, DateTime.Now.AddSeconds(150)));
            loggedTasks.Add(new TimeItem("TaskName3", DateTime.Now, DateTime.Now.AddHours(1).AddMinutes(30)));
            loggedTasks.Add(new TimeItem("TaskName4", DateTime.Now, DateTime.Now.AddHours(1)));*/
            OnPropertyChanged("loggedTasks");

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
        }

        public void PauseTask()
        {
            if (currentTask == null) return;
            if (!isPaused)
            {
                currentTask.StartPause();
                isPaused = true;
                dispatcherTimer.Stop();
            }
            else
            {
                currentTask.EndPause();
                isPaused = false;
                dispatcherTimer.Start();
            }
        }
        public void StartTask()
        {
            currentTask = new TimeItem(inputName, DateTime.Now);
            dispatcherTimer.Start();
            active = true;
        }
        public void FinishTask()
        {
            List<TimeItem> _items = new List<TimeItem>();
            if (currentTask == null) currentTask = new TimeItem(inputName, DateTime.Now);
            currentTask.SetEnd(DateTime.Now);
            currentTask.SetComment(inputComments);
            _items.AddRange(loggedTasks);
            if (_items.FindIndex(x => x.name == currentTask.name) != -1) currentTask.name += CountMatches(_items, currentTask.name).ToString();
            _items.Add(new TimeItem(currentTask));
            isPaused = false;
            loggedTasks = _items;
            OnPropertyChanged("loggedTasks");
            CancelTask();
        }
        private int CountMatches(List<TimeItem> itemList, string match)
        {
            int count = 0;
            foreach (TimeItem item in itemList)
            {
                if (item.name.Contains(match)) count++;
            }
            return count;
        }
        public void CancelTask()
        {
            currentTask = null;
            inputComments = "";
            inputName = "";
            isPaused = false;
            //OnPropertyChanged("inputComments");
            //OnPropertyChanged("inputName");
            dispatcherTimer.Stop();
            OnPropertyChanged("timer");
            active = false;
        }
        public void DeleteLoggedTask(string _name)
        {
            List<TimeItem> _items = new List<TimeItem>();
            _items.AddRange(loggedTasks);
            _items.RemoveAt(_items.FindIndex(x => x.name == _name));

            //loggedTasks.RemoveAt(loggedTasks.FindIndex(x => x.name == _name));

            loggedTasks = _items;
        }
        private void UpdateTimer(object sender, EventArgs e)
        {
            OnPropertyChanged("timer");
        }
        private void SaveCSV()
        {
            string output = "";
            double totalHours = 0;
            double totalSeconds = 0;
            output += "\"Task\",\"Hours Taken (2 Decimal Places)\",\"Comments\",\"Date & Time Completed\",\n";
            foreach (TimeItem item in loggedTasks)
            {
                totalHours += item.totalHoursTaken;
                totalSeconds += item.totalSecondsTaken;
                output += string.Format("\"{0}\",\"{1:F2}\",\"{2}\",\"{3}\",\n", item.name, item.totalHoursTaken, item.comments, item.end);
            }
            
            output += string.Format("\n\"Total:\",\"{0:F2}\",\n", totalHours);
            output += string.Format("\"\",\"Above value ignores rounding from each of the individual tasks, it's completely accurate as a total\",\n");
            string fileLocation = string.Format(@"C:\Users\alex\Desktop\{0}{1}{2}-{3}{4}{5}.CSV", DateTime.Now.Date.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            File.WriteAllText(fileLocation, output);
        }
    }
}
