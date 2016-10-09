using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;

namespace App1
{
    [Activity(Label = "ContactActivity")]
    public class ContactActivity : Activity
    {
        ContactsDB db = new ContactsDB();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            {
                SetContentView(Resource.Layout.Contact);
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = "New contact";
            }
            {
                var PhonesAdapter = new ArrayAdapter<Phone>(this, Resource.Layout.Phone, new List<Phone>());
                var phonesListView = FindViewById<ListView>(Resource.Id.phonesListView);
                phonesListView.Adapter = PhonesAdapter;

                var AddPhone = FindViewById(Resource.Id.AddPhone);
                AddPhone.Click += (sender, e) => PhonesAdapter.Add(new Phone());
            }
            {
                var EmailsAdapter = new ArrayAdapter<Email>(this, Resource.Layout.Email, new List<Email>());
                var EmailsListView = FindViewById<ListView>(Resource.Id.emailsListView);
                EmailsListView.Adapter = EmailsAdapter;

                var AddEmail = FindViewById(Resource.Id.AddEmail);
                AddEmail.Click += (sender, e) => EmailsAdapter.Add(new Email());

            }
            {
                var CancelEdit = FindViewById<Button>(Resource.Id.CancelContact);
                CancelEdit.Click += (sender, e) =>
                {
                    var intent = new Intent(this, typeof(MainActivity))
                           .SetFlags(ActivityFlags.ReorderToFront);
                    StartActivity(intent);
                };
                var SaveContact = FindViewById<Button>(Resource.Id.SaveContact);
                SaveContact.Click += (sender, e) =>
                {
                    TextView ContactNameText = FindViewById<TextView>(Resource.Id.ContactNameText);
                    TextView LastName = FindViewById<TextView>(Resource.Id.LastName);

                    var Contact = new Contact();
                    Contact.Name = ContactNameText.Text;
                    Contact.LastName = LastName.Text;


                    new ContactsDB().Insert(Contact);

                    var intent = new Intent(this, typeof(MainActivity))
                           .SetFlags(ActivityFlags.ReorderToFront);
                    base.StartActivity(intent);

                };
            }
            
            {
                var countriesSpinner = FindViewById<Spinner>(Resource.Id.countriesSpinner);
                var CountriesAdapter = new ArrayAdapter<Country>(this, Android.Resource.Layout.SimpleListItem1, new List<Country>());
                countriesSpinner.Adapter = CountriesAdapter;
                if (db.Countries.Count == 0)
                    new ContactsRestService().GetCountries().ContinueWith(Task =>
                    {
                   
                        var List = Task.Result;
                        List.ForEach(c => db.Insert(c));

                        CountriesAdapter.AddAll(List);
                        CountriesAdapter.NotifyDataSetChanged();
                    });
                else
                {
                    CountriesAdapter.AddAll(db.Countries);
                    CountriesAdapter.NotifyDataSetChanged();
                }
            }
        }

    }
}