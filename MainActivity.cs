using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
namespace App1
{
    [Activity(Label = "Contacts activity", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ArrayAdapter<string> ContactAdapter;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "My Contacts";

            var AddContact = FindViewById<Button>(Resource.Id.AddContact);
            AddContact.Click += delegate {
                StartActivity(typeof(ContactActivity));
            };

            var db = new ContactsDB();
            var Contacts = db.Contacts.ConvertAll(c => c.Name + " "+c.LastName);

            ContactAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Contacts);
            ListView ContactsList = FindViewById< ListView>(Resource.Id.ContactsList);
            ContactsList.Adapter = ContactAdapter;

        }

        protected override void OnResume()
        {
            base.OnResume();
            ContactAdapter.Clear();
            var db = new ContactsDB();
            var Contacts = db.Contacts.ConvertAll(c => c.Name + " " + c.LastName);
            ContactAdapter.AddAll(Contacts);
            //            adapter.swapItems(dbHelper.getItems());
        }
    }
}

