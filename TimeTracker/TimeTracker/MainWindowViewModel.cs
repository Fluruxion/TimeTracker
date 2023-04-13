using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using Microsoft.Win32;

namespace TimeTracker
{
    class MainWindowViewModel : ObservableObject
    {
        ModifyTimeWindow popupView { get; set; }
        ModifyTimeWindowViewModel popupViewModel { get; set; }
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
        private bool _fullDayActive { get; set; }
        public bool fullDayActive
        {
            get
            {
                return _fullDayActive;
            }
            set
            {
                _fullDayActive = value;
                OnPropertyChanged("fullDayActive");
            }
        }
        private TimeSpan TotalTimeMinusActive
        {
            get
            {
                if (loggedTasks == null || loggedTasks.Count == 0) return new TimeSpan();

                TimeSpan tempTotal = new TimeSpan(0, 0, 0);

                foreach (TimeItem item in loggedTasks)
                {
                    tempTotal = tempTotal.Add(item.timeTaken);
                }

                return tempTotal;
            }
        }
        private string _TotalTimeTakenDisplay { get; set; }
        public string TotalTimeTakenDisplay
        {
            get
            {
                return _TotalTimeTakenDisplay;
            }
            set
            {
                _TotalTimeTakenDisplay = value;
                OnPropertyChanged("TotalTimeTakenDisplay");
            }
        }
        private ICommand _CommandPause { get; set; }
        private ICommand _CommandStart { get; set; }
        private ICommand _CommandFinish { get; set; }
        private ICommand _CommandCancel { get; set; }
        private ICommand _CommandDelete { get; set; }
        private ICommand _CommandContinue { get; set; }
        private ICommand _CommandSave { get; set; }
        private ICommand _CommandEdit { get; set; }
        private ICommand _CommandRefresh { get; set; }
        private ICommand _CommandClosePopup { get; set; }
        private ICommand _CommandImportCSV { get; set; }
        private ICommand _CommandFullDay { get; set; }
        private ICommand _CommandConfirmFullDay { get; set; }
        private ICommand _CommandCancelFullDay { get; set; }

        public string timer
        {
            get
            {
                if (currentTask == null) return "0 Hours\n0 Minutes";

                return currentTask.GetCurrentTimeTaken();
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
        public ICommand CommandContinue
        {
            get
            {
                if (_CommandContinue == null)
                {
                    _CommandContinue = new DelegateCommand<string>(p => ContinueLoggedTask(p));
                }

                return _CommandContinue;
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
        public ICommand CommandEdit
        {
            get
            {
                if (_CommandEdit == null)
                {
                    _CommandEdit = new DelegateCommand<string>(p => EditLoggedTask(p));
                }

                return _CommandEdit;
            }
        }
        public ICommand CommandRefresh
        {
            get
            {
                if (_CommandRefresh == null)
                {
                    _CommandRefresh = new DelegateCommand(p => RefreshScreen());
                }

                return _CommandRefresh;
            }
        }
        public ICommand CommandClosePopup
        {
            get
            {
                if (_CommandClosePopup == null)
                {
                    _CommandClosePopup = new DelegateCommand(p => ClosePopup());
                }

                return _CommandClosePopup;
            }
        }
        public ICommand CommandImportCSV
        {
            get
            {
                if (_CommandImportCSV == null)
                {
                    _CommandImportCSV = new DelegateCommand(p => OpenCSV());
                }

                return _CommandImportCSV;
            }
        }
        public ICommand CommandFullDay
        {
            get
            {
                if (_CommandFullDay == null)
                {
                    _CommandFullDay = new DelegateCommand(p => FullDayBegin());
                }

                return _CommandFullDay;
            }
        }
        public ICommand CommandConfirmFullDay
        {
            get
            {
                if (_CommandConfirmFullDay == null)
                {
                    _CommandConfirmFullDay = new DelegateCommand(p => ConfirmFullDay());
                }

                return _CommandConfirmFullDay;
            }
        }
        public ICommand CommandCancelFullDay
        {
            get
            {
                if (_CommandCancelFullDay == null)
                {
                    _CommandCancelFullDay = new DelegateCommand(p => CancelFullDay());
                }

                return _CommandCancelFullDay;
            }
        }
        public TimeItem currentTask
        {
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
                UpdateTotalTimeTakenDisplay();
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

            OnPropertyChanged("loggedTasks");

            // This timer is used to update the label presenting the currentTimeDisplay value. It measures in minutes so don't need to set the interval too low.
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
        }

        /// <summary>
        /// This is the main method used to update the screen whenever we want all the information to be updated at once instead of a small section.
        /// </summary>
        public void RefreshScreen()
        {
            // Re-assign the list into loggedTasks so that the UI is able to grab the new information, otherwise OnPropertyChanged does nothing with it
            List<TimeItem> _logged = new List<TimeItem>();
            _logged.AddRange(loggedTasks);
            loggedTasks = _logged;
            OnPropertyChanged("loggedTasks");
            OnPropertyChanged("inputComments");
            OnPropertyChanged("inputName");
            OnPropertyChanged("isPaused");
            OnPropertyChanged("loggedTasks");
            OnPropertyChanged("currentTask");
            OnPropertyChanged("timer");
        }

        /// <summary>
        /// Pauses or unpauses the currently tracked task depending on the value of isPaused
        /// </summary>
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
        /// <summary>
        /// Creates a new TimeItem and sets it as the current task
        /// </summary>
        public void StartTask()
        {
            currentTask = new TimeItem(inputName, DateTime.Now);
            dispatcherTimer.Start();
            active = true;
        }
        /// <summary>
        /// Adds the current task to the list of logged tasks and clears the UI to make room for the next one.
        /// If the current task is somehow blank or invalid it will create a blank task in the list instead. This is done to allow mass creation of tasks for testing.
        /// It is not possible to "finish" a blank task from the UI as the button to do so is not usable unless there is a currently active task.
        /// Automatically appends a number to the end of a task name if a task already exists with the same name.
        /// This was originally done to ensure correct task was selected but also is preferred for timesheet output by employer.
        /// </summary>
        public void FinishTask()
        {
            List<TimeItem> _items = new List<TimeItem>();
            if (currentTask == null) currentTask = new TimeItem(inputName, DateTime.Now);
            currentTask.SetComment(inputComments);
            currentTask.Finished();
            _items.AddRange(loggedTasks);
            if (_items.FindIndex(x => x.name == currentTask.name) != -1) currentTask.name += CountMatches(_items, currentTask.name).ToString();
            _items.Add(new TimeItem(currentTask));
            isPaused = false;
            loggedTasks = _items;
            OnPropertyChanged("loggedTasks");
            CancelTask();
        }
        /// <summary>
        /// Counts the amount of tasks in the logged task list with the same name as the current task.
        /// This is used to assign it with a unique name.
        /// </summary>
        /// <param name="itemList">This is usually going to be the logged task list, but can be any list of TimeItems provided</param>
        /// <param name="match">The name we're checking for</param>
        /// <returns>Returns an int value equal to the amount of times the parameter "match" was found in the provided list of TimeItems</returns>
        private int CountMatches(List<TimeItem> itemList, string match)
        {
            int count = 0;
            foreach (TimeItem item in itemList)
            {
                if (item.name.Contains(match)) count++;
            }
            return count;
        }
        /// <summary>
        /// Clears the current task UI and associated data/values to prepare it for the next task.
        /// </summary>
        public void CancelTask()
        {
            currentTask = null;
            inputComments = "";
            inputName = "";
            isPaused = false;
            dispatcherTimer.Stop();
            OnPropertyChanged("timer");
            active = false;
        }
        /// <summary>
        /// Deletes the logged task which matches the provided name. This is usually called from the UI where it passes it's own name through the ICommand as a parameter.
        /// Could probably update this to be cleaner by passing through the TimeItem itself to prevent the need for protecting against duplicate names.
        /// Might be worth investigating later, though this currently works without issue and there is no pressing reason to change it. Unique names is also preferable for current timesheet outputs.
        /// </summary>
        /// <param name="_name">The name of the TimeItem to delete</param>
        public void DeleteLoggedTask(string _name)
        {
            List<TimeItem> _items = new List<TimeItem>();
            _items.AddRange(loggedTasks);
            int removalIndex = _items.FindIndex(x => x.name == _name);
            if (removalIndex != -1) _items.RemoveAt(removalIndex); // Index of -1 means it doesn't exist, so only try to delete if it actually exists

            //loggedTasks.RemoveAt(loggedTasks.FindIndex(x => x.name == _name));

            loggedTasks = _items;
        }
        /// <summary>
        /// Sets a logged TimeItem as the current TimeItem.
        /// </summary>
        /// <param name="_name">The name of the TimeItem to set as current</param>
        public void ContinueLoggedTask(string _name)
        {

            TimeItem tempCurrentTask = loggedTasks.Find(x => x.name == _name);
            inputName = tempCurrentTask.name;
            inputComments = tempCurrentTask.comments;

            StartTask();

            currentTask = tempCurrentTask;
            currentTask.SetStart(DateTime.Now);

            DeleteLoggedTask(_name);

            RefreshScreen();
        }
        /// <summary>
        /// Allows manual editing of the hours of a task. Useful if task was left ongoing by mistake or if adding a task that has a known duration that does not currently exist in logged tasks.
        /// Opens a new window with the required UI to edit the timings, this is due to space constraints on the main window.
        /// </summary>
        /// <param name="_name">Name of TimeItem to modify</param>
        public void EditLoggedTask(string _name)
        {
            TimeItem editItem = loggedTasks.Find(x => x.name == _name);
            popupView = new ModifyTimeWindow();
            popupViewModel = new ModifyTimeWindowViewModel(ref editItem, CommandRefresh, CommandClosePopup);
            popupView.DataContext = popupViewModel;
            popupView.Show();
        }
        private void UpdateTimer(object sender, EventArgs e)
        {
            OnPropertyChanged("timer");
        }

        /// <summary>
        /// Updates a label beneath the loggedtasks list to represent the total time taken of all logged tasks.
        /// </summary>
        private void UpdateTotalTimeTakenDisplay()
        {
            if (TotalTimeMinusActive.TotalSeconds == 0) TotalTimeTakenDisplay = "";
            else if (TotalTimeMinusActive.TotalSeconds < 60) TotalTimeTakenDisplay = string.Format("Logged: {0:##} seconds", TotalTimeMinusActive.TotalSeconds);
            else if (TotalTimeMinusActive.TotalMinutes < 60) TotalTimeTakenDisplay = string.Format("Logged: {0:##.#} minutes", TotalTimeMinusActive.TotalMinutes);
            else TotalTimeTakenDisplay = string.Format("Logged: {0:##} hours, {1} minutes", Math.Floor(TotalTimeMinusActive.TotalHours), TotalTimeMinusActive.Minutes);
        }

        /// <summary>
        /// Creates a .csv file containing the data of all logged tasks. Layout is made to fit requirements for current employment.
        /// Does this by simply appending text to an output string and saving as required.
        /// Might be worth saving the most recent save location to registry so it re-opens at the same place next time the user attempts to save a file?
        /// </summary>
        private void SaveCSV()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string lastPath = GetLastPathInRegistry();
            if (lastPath != "")
                desktop = lastPath;

            Microsoft.Win32.SaveFileDialog saveFile = new Microsoft.Win32.SaveFileDialog();
            string output = "";
            double totalHours = 0;
            double totalSeconds = 0;
            output += "Task,Hours Taken (2 Decimal Places),Comments,Date & Time Completed,\n";
            foreach (TimeItem item in loggedTasks)
            {
                totalHours += item.totalHoursTaken;
                totalSeconds += item.totalSecondsTaken;
                output += string.Format("{0},{1:F2},{2},{3},\n", item.name, item.totalHoursTaken, item.comments, item.lastSaved);
            }

            output += string.Format("\nTotal:,{0:F2},\n", totalHours);
            string fileName = string.Format(@"{0}-{1}-{2}-{3}{4}{5}.CSV", DateTime.Now.Date.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            saveFile.FileName = fileName;
            saveFile.DefaultExt = ".CSV";
            saveFile.Filter = "Comma Seperated Values (.CSV)|*.CSV*";
            saveFile.InitialDirectory = desktop;
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFile.FileName, output);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save CSV.\nMake sure you're not trying to save to a file that's in use.");
                }
                finally
                {
                    string test = Directory.GetParent(saveFile.FileName).FullName;
                    SetLastPathInRegistry(test);
                }
            }
        }
        /// <summary>
        /// This closes the edit TimeItem popup which is used to edit the timings of a logged TimeItem.
        /// A command which calls this method is passed into the popup constructor when it is created.
        /// </summary>
        private void ClosePopup()
        {
            popupView.Close();
            popupView = null;
            popupViewModel = null;
        }
        /// <summary>
        /// Allows the user to open a previously created .csv file with an expected format and add all entries to the logged tasks list as TimeItems.
        /// Useful for creating timesheets that span over more than just a day. Was added to match requirement of submitting 1 timesheet per month.
        /// Might be worth investigating a way to simply append to a specified timesheet instead of having to import it each time, though this works for now and I've had no issues with it.
        /// </summary>
        private void OpenCSV()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            string lastPath = GetLastPathInRegistry();
            if (lastPath != "")
                desktop = lastPath;

            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();

            openFile.DefaultExt = ".CSV";
            openFile.Filter = "Comma Seperated Values (.CSV)|*.CSV*";
            openFile.InitialDirectory = desktop;

            if (openFile.ShowDialog() == true)
            {
                List<string> fileInput = new List<string>();

                try
                {
                    fileInput = File.ReadLines(openFile.FileName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File is open or in use.\nPlease close the file before attempting to open it again.");
                    return;
                }
                finally
                {
                    string test = Directory.GetParent(openFile.FileName).FullName;
                    SetLastPathInRegistry(test);
                }

                int count = 0;

                List<TimeItem> importedItems = new List<TimeItem>();

                foreach (string line in fileInput)
                {
                    count++;

                    if (count == 1) continue;

                    string[] splitLine = line.Split(',');

                    if (splitLine[0].ToLower() == "" || splitLine[0].ToLower().Contains("total")) break;


                    importedItems.Add(new TimeItem(splitLine[0], splitLine[1], splitLine[2], splitLine[3]));
                }

                List<TimeItem> _logged = new List<TimeItem>();
                _logged.AddRange(loggedTasks);
                _logged.AddRange(importedItems);

                loggedTasks = _logged;

                RefreshScreen();
            }
            else return;
        }

        /// <summary>
        /// Part of 3 functions used to calculate a task that represents the remaining hours of a set workday.
        /// This function just sets up the main screen to be ready to mark tasks as part of the work day.
        /// </summary>
        private void FullDayBegin()
        {
            fullDayActive = true;
        }

        /// <summary>
        /// Takes all marked tasks, adds the times together and subtracts it from 8. Generates a task to represent the remainder of the set workday. Then sets the program back to normal mode.
        /// This function is currently hardcoded for a full workday on Tuesday, this is because it's the only full agreed workday I currently have. May adjust application in the future if needed.
        /// Note: This function does not wipe the IsChecked value when it completes, this means the Full Workday task can easily be regenerated with the same tasks if required.
        /// </summary>
        private void ConfirmFullDay()
        {
            // This expects the user to be generating the Full Workday task on the same day as the full workday itself, if that is not the case the CSV can be edited as required.
            string FullDayTaskName = string.Format("Remainder of full workday - {0}/{1}/{2}", DateTime.Now.Date.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year);
            string DayOfTheWeek = DateTime.Now.DayOfWeek.ToString().ToUpper();

            // If we are creating a duplicate Full Workday task then delete the original.
            DeleteLoggedTask(FullDayTaskName);

            TimeSpan FullDayHours = new TimeSpan(8, 0, 0); // 8 Hour workday
            TimeSpan CombinedTasks = new TimeSpan(0, 0, 0);

            foreach (TimeItem item in loggedTasks)
            {
                if (item.isChecked)
                {
                    CombinedTasks = CombinedTasks.Add(item.timeTaken);
                    if (item.comments == null) item.comments = string.Format(" - {0} (full workday)", DayOfTheWeek);
                    if (!item.comments.ToLower().Contains(DayOfTheWeek.ToLower())) item.comments += string.Format(" - {0} (full workday)", DayOfTheWeek);
                }
            }
            TimeSpan RemainderOfFullDay = new TimeSpan(0, 0, 0);
            if (CombinedTasks.TotalSeconds > 0) RemainderOfFullDay = FullDayHours.Subtract(CombinedTasks);
            
            TimeItem FullDayRemainder = new TimeItem(FullDayTaskName, RemainderOfFullDay, string.Format("{0} - This task represents the remaining hours of a set full workday. Hours from tasks done during a full workday are subtracted from the original 8 hours and the remainder is added to this task.", DayOfTheWeek));

            // Have to do this so the listView updates appropriately.
            List<TimeItem> _loggedTasks = new List<TimeItem>();
            _loggedTasks.AddRange(loggedTasks);
            _loggedTasks.Add(FullDayRemainder);
            loggedTasks = _loggedTasks;
            OnPropertyChanged("loggedTasks");

            fullDayActive = false;
        }


        /// <summary>
        /// Removes marks from all tasks, sets the program back to normal mode.
        /// </summary>
        private void CancelFullDay()
        {
            foreach (TimeItem item in loggedTasks)
            {
                item.isChecked = false;
            }
            fullDayActive = false;
        }

        public string GetLastPathInRegistry()
        {
            RegistryKey RegPath = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TimeTracker");
            if (RegPath == null)
                return "";

            object returnPath = RegPath.GetValue("LastPath");
            if (returnPath == null)
                returnPath = "";

            return returnPath.ToString();
        }
        public void SetLastPathInRegistry(string path)
        {
            RegistryKey RegSave = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TimeTracker");
            if (RegSave == null)
                return;

            if (path != null && path != "") RegSave.SetValue("LastPath", path);
        }
    }
}