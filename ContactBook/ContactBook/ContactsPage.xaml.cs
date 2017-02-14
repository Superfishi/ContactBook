using ContactBook.Models;
using ContactBook.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ContactBook
{
    public partial class ContactsPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Contact> _contacts;
        private bool _isDataLoaded;

        public ContactsPage()
        {
            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            // In a multi-page app, everytime we come back to this page, OnAppearing
            // method is called, but we want to load the data only the first time
            // this page is loaded. In other words, when we go to ContactDetailPage
            // and come back, we don't want to reload the data. The data is already
            // there. We can control this using a switch: isDataLoaded.
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;

            // I've extracted the logic for loading data into LoadData method. 
            // Now the code in OnAppearing method looks a lot cleaner. The 
            // purpose is very explicit. If data is loaded, return, otherwise,
            // load data. Details of loading the data is delegated to LoadData
            // method. 
            await LoadData();

            base.OnAppearing();
        }

        // Note that this method returns a Task, instead of void. Void should 
        // only be used for event handlers (e.g. OnAppearing). In all other cases,
        // you should return a Task or Task<T>.
        private async Task LoadData()
        {
            await _connection.CreateTableAsync<Contact>();
            var contacts = await _connection.Table<Contact>().ToListAsync();

            _contacts = new ObservableCollection<Contact>(contacts);
            
            listView.ItemsSource = _contacts;
        }

        private async void OnAddContact(object sender, EventArgs e)
        {
            var page = new ContactDetailsPage(new Contact());

            // We can subscribe to the ContactAdded event using a lambda expression.
            // If you're not familiar with this syntax, watch my C# Advanced course. 
            page.ContactAdded += (source, contact) =>
            {
                
                // ContactAdded event is raised when the user taps the Done button.
                // Here, we get notified and add this contact to our 
                // ObservableCollection.
                _contacts.Add(contact);
            };

            await Navigation.PushAsync(page);
        }

        private void OnContactSelected(object sender, SelectedItemChangedEventArgs e)
        {

            // We need to check if SelectedItem is null because further below 
            // we de-select the selected item. This will raise another ItemSelected
            // event, so this method will be called straight away. If we don't
            // check for null here, we'll get a NullReferenceException.
            if (listView.SelectedItem == null)
                return;

            // We de-select the selected item, so when we come back to the Contacts
            // page we can tap it again and navigate to ContactDetail.
            listView.SelectedItem = null;

            var selectedContact = e.SelectedItem as Contact;

            var page = new ContactDetailsPage(selectedContact);


            page.ContactUpdated += (source, contact) =>
            {

                // When the target page raises ContactUpdated event, we get 
                // notified and update properties of the selected contact. 
                // Here we are dealing with a small class with only a few 
                // properties. If working with a larger class, you may want 
                // to look at AutoMapper, which is a convention-based mapping
                // tool. 
                selectedContact.Id = contact.Id;
                selectedContact.FirstName = contact.FirstName;
                selectedContact.LastName = contact.LastName;
                selectedContact.Phone = contact.Phone;
                selectedContact.Email = contact.Email;
                selectedContact.IsBlocked = contact.IsBlocked;
            };

            Navigation.PushAsync(page);


        }

        private async void OnDeleteContact(object sender, EventArgs e)
        {
            var contact = (sender as MenuItem).CommandParameter as Contact;

            if (await DisplayAlert("Warning", $"Are you sure you want to delete {contact.FullName}?", "Yes", "No"))
            {
                _contacts.Remove(contact);

                await _connection.DeleteAsync(contact);

            }
        }
    }
}
