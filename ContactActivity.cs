using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
    [Activity(Label = "ContactActivity")]
    public class ContactActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Contact);

            var countriesSpinner = FindViewById<Spinner>(Resource.Id.countriesSpinner);

            var adapter = new ArrayAdapter<Country>(this, Android.Resource.Layout.SimpleListItem1, new List<Country>());
            countriesSpinner.Adapter = adapter;

            new ContactsService().RefreshCountries().ContinueWith(l => {
                adapter.AddAll(l.Result);
                adapter.NotifyDataSetChanged();
            });
        }
    }
}