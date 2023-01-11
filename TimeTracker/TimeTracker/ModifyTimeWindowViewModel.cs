using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace TimeTracker
{
    class ModifyTimeWindowViewModel : ObservableObject
    {
        private string m_CurrentTime { get; set; }
        private string m_Hours { get; set; }
        private string m_Minutes { get; set; }
        private ICommand m_CommandModifyTimeItem { get; set; }
        private ICommand m_CommandRefresh { get; set; }
        private ICommand m_CommandClosePopup { get; set; }
        public ICommand CommandModifyTimeItem
        {
            get
            {
                if (m_CommandModifyTimeItem == null)
                {
                    m_CommandModifyTimeItem = new DelegateCommand<string>(p => ModifyTimeItem(p));
                }

                return m_CommandModifyTimeItem;
            }
        }
        public string CurrentTime
        {
            get
            {
                return m_CurrentTime;
            }
            set
            {
                m_CurrentTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }
        public string Hours
        {
            get
            {
                return m_Hours;
            }
            set
            {
                m_Hours = value;
                OnPropertyChanged("Hours");
            }
        }
        public string Minutes
        {
            get
            {
                return m_Minutes;
            }
            set
            {
                m_Minutes = value;
                OnPropertyChanged("Minutes");
            }
        }
        public TimeItem CurrentItem { get; set; }
        public ModifyTimeWindowViewModel(ref TimeItem timeItem, ICommand _refreshWindow, ICommand _closePopup)
        {
            Hours = "0";
            Minutes = "0";
            CurrentItem = timeItem;
            m_CommandRefresh = _refreshWindow;
            m_CommandClosePopup = _closePopup;
        }
        /// <summary>
        /// Modifies this class' timeItem based on the action parameter
        /// </summary>
        /// <param name="action">1=subtract, 2=set, 3=add</param>
        public void ModifyTimeItem(string action)
        {
            int hours = 0;
            int minutes = 0;
            if (!Int32.TryParse(Hours, out hours) || !Int32.TryParse(Minutes, out minutes))
            {
                MessageBox.Show("Invalid hours or minutes");
                return;
            }

            switch (action)
            {
                case "1":
                    CurrentItem.timeTaken = CurrentItem.timeTaken.Subtract(new TimeSpan(hours, minutes, 0));
                    break;
                case "2":
                    CurrentItem.timeTaken = new TimeSpan(hours, minutes, 0);
                    break;
                case "3":
                    CurrentItem.timeTaken = CurrentItem.timeTaken.Add(new TimeSpan(hours, minutes, 0));
                    break;
            }

            m_CommandRefresh.Execute(null);
            m_CommandClosePopup.Execute(null);
        }
    }
}
