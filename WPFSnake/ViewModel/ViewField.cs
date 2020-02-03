using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSnake.ViewModel
{
    class ViewField : ViewModelBase
    {
        private string _color;
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public string Color 
        {
            get { return _color; }
                set 
                {
                    if (_color != value)
                    {
                        _color = value;
                        OnPropertyChanged();
                    }   
                } 
        }
        // CONSTRUCTOR -> to call OnPropertyChanged()

    }
}
