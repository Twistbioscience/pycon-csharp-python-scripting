﻿using System.Threading;
using System.Threading.Tasks;

namespace Twist.PyCon
{
    public class Device : ViewModelBase
    {
        private readonly object _lock = new object();
        private volatile bool StopRequested = false;

        public RelayCommand<object> FillCommand { get; private set; }
        public RelayCommand<object> EmptyCommand { get; private set; }

        #region Properties
        private int _status = 0;
        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        #endregion

        public Device()
        {
            FillCommand = new RelayCommand<object>(async _ => await Task.Run(() => Fill()));
            EmptyCommand = new RelayCommand<object>(async _ => await Task.Run(() => Empty()));
        }

        public void Fill()
        {
            lock (_lock)
            {
                while (Status < 100)
                {
                    if (StopRequested)
                    {
                        StopRequested = false;
                        break;
                    }

                    Thread.Sleep(100);
                    Status++;
                }
            }
        }


        public void Empty()
        {
            lock (_lock)
            {
                while (Status > 0)
                {
                    Thread.Sleep(100);
                    if (StopRequested)
                    {
                        StopRequested = false;
                        break;
                    }

                    Status--;
                }
            }
        }

        public void Stop()
        {
            StopRequested = true;
        }
    }
}
