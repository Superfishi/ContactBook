using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ContactBook
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new ContactsPage());

            Properties["userName"] = "Mosh";
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
