using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Content;

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

            FindViewById<Button>(Resource.Id.AddContact).Click += (sender, e) => {
                var activity2 = new Intent(this, typeof(ContactActivity));
                activity2.PutExtra("Contact", Newtonsoft.Json.JsonConvert.SerializeObject(new Contact()));
                StartActivity(activity2);
            }; 
            

            var db = new ContactsDB();
            var Contacts = db.Contacts.ConvertAll(c => c.Name + " "+c.LastName);

            ContactAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Contacts);
            ListView ContactsList = FindViewById< ListView>(Resource.Id.ContactsList);
            ContactsList.Adapter = ContactAdapter;

            ContactsList.ItemClick += ContactsList_ItemClick;
        }

        private void ContactsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            var activity2 = new Intent(this, typeof(ContactActivity));
            activity2.PutExtra("Contact", Newtonsoft.Json.JsonConvert.SerializeObject(Contacts[e.Position]));
            StartActivity(activity2);
        }

        List<Contact> Contacts;

        protected override void OnResume()
        {
            base.OnResume();
            ContactAdapter.Clear();
            var db = new ContactsDB();
            Contacts = db.Contacts;
            ContactAdapter.AddAll(Contacts.ConvertAll(c => c.Name + " " + c.LastName));
        }
    }
}

