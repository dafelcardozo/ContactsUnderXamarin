using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
namespace App1
{
    [Activity(Label = "Contacts activity", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            Spinner countriesSpinner = FindViewById<Spinner>(Resource.Id.countriesSpinner);

            ArrayAdapter<Country> adapter = new ArrayAdapter<Country>(this, Android.Resource.Layout.SimpleListItem1, new List<Country>());
            countriesSpinner.Adapter = adapter;

            new ContactsService().RefreshCountries().ContinueWith(l => {
                adapter.AddAll(l.Result);
                adapter.NotifyDataSetChanged();
            });
        }        
    }
}

