using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using Java.IO;
using Android.Graphics;
using Android.Net;
using Android.Views;
using Newtonsoft.Json;

namespace App1
{
    [Activity(Label = "ContactActivity")]
    public class ContactActivity : Activity
    {
        ContactsDB db = new ContactsDB();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Contact);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "New contact";

            /*
            var Phones = new List<Phone> { new Phone() };
            var PhonesAdapter = new ArrayAdapter<Phone>(this, Resource.Layout.Phone, Phones);
            var phonesListView = FindViewById<ListView>(Resource.Id.phonesListView);
            phonesListView.Adapter = PhonesAdapter;
            */
            //var AddPhone = FindViewById(Resource.Id.AddPhone);
            //AddPhone.Click += (sender, e) => PhonesAdapter.Add(new Phone());
            var Contact = JsonConvert.DeserializeObject < Contact>(Intent.GetStringExtra("Contact"));
            TextView ContactNameText = FindViewById<TextView>(Resource.Id.ContactNameText);
            TextView LastName = FindViewById<TextView>(Resource.Id.LastName);

            ContactNameText.Text = Contact.Name ;
            LastName.Text = Contact.LastName ;

            var EmailsAdapter = new EmailsAdapter(this, Contact);     
            var EmailsListView = FindViewById<ListView>(Resource.Id.emailsListView);
            EmailsListView.Adapter = EmailsAdapter;

            var AddEmail = FindViewById<Button>(Resource.Id.AddEmail);
            AddEmail.Click += (sender, e) => EmailsAdapter.AddEmail();


            FindViewById<Button>(Resource.Id.CancelContact).Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(MainActivity))
                       .SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
            };
            FindViewById<Button>(Resource.Id.SaveContact).Click += (sender, e) =>
            {
                Contact.Name = ContactNameText.Text;
                Contact.LastName = LastName.Text;

                if (Contact.Id == 0)
                {
                    new ContactsDB().Insert(Contact);
                } else
                {
                    new ContactsDB().Update(Contact);
                }

                var intent = new Intent(this, typeof(MainActivity))
                       .SetFlags(ActivityFlags.ReorderToFront);
                base.StartActivity(intent);

            };
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

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }

//            Button button = FindViewById<Button>(Resource.Id.TakePicture);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            _imageView.Click += TakeAPicture;

        }
        ImageView _imageView;
        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakeAPicture(object sender, System.EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                _imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            System.GC.Collect();
        }
    }


    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);
            return resizedBitmap;
        }
    }
    public class EmailsAdapter : BaseAdapter
    {
        readonly List<Email> Emails = new List<Email> { new Email () };
        readonly Activity _activity;
        readonly Contact Contact;

        public EmailsAdapter(Activity activity, Contact contact)
        {
            _activity = activity;
            this.Contact = contact;
        }
        private void FillContacts()
        {
            ContactsDB db = new ContactsDB();
            Emails.Clear();
            Emails.AddRange(db.ContactEmails(Contact));
        }

        public override int Count
        {
            get { return Emails.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;// Emails[position];
        }

        public override long GetItemId(int position)
        {
            return Emails[position].Id;
        }
        public void AddEmail()
        {
            ContactsDB db = new ContactsDB();
            Email email = new global::Email();
            db.Insert(email);
            Emails.Add(email);
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.Email, parent, false);
            var Email = view.FindViewById<TextView>(Resource.Id.Email);
            Email.Text = Emails[position].Address;       
            Email.TextChanged += (sender, e) => {
                Emails[position].Address = Email.Text;
                ContactsDB db = new ContactsDB();
                db.Update(Emails[position]);
            };
            return view;
        }

    }
}