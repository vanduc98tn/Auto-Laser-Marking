using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{

    public class WorkItem : PropertyChangedBase
    {
        // Tọa độ vật lý trong mảng
        public int PhysR { get; set; }
        public int PhysC { get; set; }

        // Số thứ tự hiển thị (1, 2, 3...)
        public int Index { get; set; }

        private bool _isNG;
        public bool IsNG
        {
            get => _isNG;
            set
            {
                _isNG = value;
                // Khi IsNG thay đổi, thông báo để UI (cả 2 bảng) cùng đổi màu
                OnPropertyChanged(nameof(IsNG));
            }
        }

        private bool _hasBeenReached;
        public bool HasBeenReached
        {
            get => _hasBeenReached;
            set
            {
                _hasBeenReached = value;
                OnPropertyChanged(nameof(HasBeenReached));
            }
        }

        private bool _isCurrent;
        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                _isCurrent = value;
                OnPropertyChanged(nameof(IsCurrent));
            }
        }
    }
    //public class WorkItem : INotifyPropertyChanged
    //{
    //    public int Index { get; set; }
    //    public int PhysR { get; set; }
    //    public int PhysC { get; set; }
    //    private bool _isNg; public bool IsNG { get => _isNg; set { _isNg = value; OnPropertyChanged(nameof(IsNG)); } }
    //    private bool _hasBeenReached; public bool HasBeenReached { get => _hasBeenReached; set { _hasBeenReached = value; OnPropertyChanged(nameof(HasBeenReached)); } }
    //    private bool _isCurrent; public bool IsCurrent { get => _isCurrent; set { _isCurrent = value; OnPropertyChanged(nameof(IsCurrent)); } }
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //}
    //public class WorkItem : INotifyPropertyChanged
    //{
    //    public int Index { get; set; }
    //    public int PhysR { get; set; }
    //    public int PhysC { get; set; }
    //    private bool _isNg; public bool IsNG { get => _isNg; set { _isNg = value; OnPropertyChanged(nameof(IsNG)); } }
    //    private bool _hasBeenReached; public bool HasBeenReached { get => _hasBeenReached; set { _hasBeenReached = value; OnPropertyChanged(nameof(HasBeenReached)); } }
    //    private bool _isCurrent; public bool IsCurrent { get => _isCurrent; set { _isCurrent = value; OnPropertyChanged(nameof(IsCurrent)); } }
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //}

}
