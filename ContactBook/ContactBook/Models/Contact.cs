using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Models
{
    public class Contact : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (_firstName == value)
                    return;

                _firstName = value;


                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));

            }
        }

        private string _lastName;

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (_lastName == value)
                    return;

                _lastName = value;


                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));

            }
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsBlocked { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
