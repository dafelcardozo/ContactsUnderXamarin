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
        private List<Contact> Contacts = new List<Contact>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetActionBar(toolbar);
            //ActionBar.Title = "My Toolbar";

            var AddContact = FindViewById<Button>(Resource.Id.AddContact);
            AddContact.Click += delegate {
                StartActivity(typeof(ContactActivity));
            };
            
            
        }

     
    }
}

