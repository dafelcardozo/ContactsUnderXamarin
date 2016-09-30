
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;


using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using SQLite;

public class ContactsService 
{
    HttpClient client;

  public ContactsService()
  {
    client = new HttpClient();
  }

 
    public async Task<List<Country>> RefreshCountries()
    {
        var uri = new Uri("https://contactmanager.banlinea.com/api/countries");
        var response = await client.GetAsync(uri);
       // if (response.IsSuccessStatusCode)
       // {
            var content = await response.Content.ReadAsStringAsync();
           return JsonConvert.DeserializeObject<List<Country>>(content);
        //}
 }
}

public class Country
{
    [PrimaryKey]
    public int Code { get; set; }
    public string Name { get; set; }
    public Boolean Enabled { get; set; }
    
    public override String ToString()
    {
        return Name;
    }
}

public class Contact
{
    public string Company { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }
    public Photo Photo { get; set; }
    public List<Phone> PhoneNumbers { get; set; }
    public List<Email> EmailsAddress { get; set; }
}


public class Email
{
    public Contact Contact {get; set;}
    public String Address { get; set; }
}

public class Phone
{
    public Contact Contact { get; set; }
    public Country Country { get; set; }
    public string Number { get; set; }
}

public class Photo
{
    public byte[] Raw { get; set; }
}

public class Location
{
    double Longitude { get; set; }
    double Latitude { get; set; }
}

public class ContactsReport
{
    public List<Contact> Contacts { get; set; }
    public Location Location { get; set; }
    public string RegisteredBy { get; set; }
    public int Type { get; set; }
}


public class ContactsDB
{
    string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

    private void createDatabase(string path)
    {
        var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(folder, "contacts.db"));
        connection.CreateTableAsync<Country>();
                
            
    }

    private void insert(Country data)
    {
        var db = new SQLiteConnection(System.IO.Path.Combine(folder, "contacts.db"));
        db.Insert(data);
        
    }
    public List<Country> Countries
    {
        get
        {
            var db = new SQLiteConnection(System.IO.Path.Combine(folder, "contacts.db"));
            return db.Query<Country>("select * from Country");
        }
    }
}