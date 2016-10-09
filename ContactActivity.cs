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

            var PhonesAdapter = new ArrayAdapter<Phone>(this, Resource.Layout.Phone, new List<Phone>());
            var phonesListView = FindViewById<ListView>(Resource.Id.phonesListView);
            phonesListView.Adapter = PhonesAdapter;

            var AddPhone = FindViewById(Resource.Id.AddPhone);
            AddPhone.Click += (sender, e) => PhonesAdapter.Add(new Phone());
            var EmailsAdapter = new ArrayAdapter<Email>(this, Resource.Layout.Email, new List<Email>());
            var EmailsListView = FindViewById<ListView>(Resource.Id.emailsListView);
            EmailsListView.Adapter = EmailsAdapter;

            var AddEmail = FindViewById(Resource.Id.AddEmail);
            AddEmail.Click += (sender, e) => EmailsAdapter.Add(new Email());

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

            Button button = FindViewById<Button>(Resource.Id.TakePicture);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            button.Click += TakeAPicture;

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
            //BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            //int outHeight = 128;// options.OutHeight;
            //int outWidth = 60; // options.OutWidth;
            /*
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }
            */
            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = 10;
            options.InJustDecodeBounds = false;
            options.InScaled = true;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}