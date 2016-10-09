
using System;
using System.Threading.Tasks;


using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

public class ContactsRestService 
{
    HttpClient client;

  public ContactsRestService()
  {
    client = new HttpClient();
  }

 
    public async Task<List<Country>> GetCountries()
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
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Company { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }

    //   public Photo Photo { get; set; }
    //[OneToMany]
    //public List<Phone> PhoneNumbers { get; set; }
    //[OneToMany]
    //public List<Email> EmailsAddress { get; set; }
}


public class Email
{
    [ForeignKey(typeof(Contact))]
    public Contact Contact {get; set;}
    public string Address { get; set; }
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
}

public class Phone
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [ForeignKey(typeof(Contact))]
    public Contact Contact { get; set; }
    [ForeignKey(typeof(Country))]
    public Country Country { get; set; }
    public string Number { get; set; }

    public override string ToString()
    {
        return Number;
    }
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

    public ContactsDB()
    {
        createDatabase();
    }

    private void createDatabase()
    {
        var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "contacts.db"));
        connection.CreateTable<Country>();
        connection.CreateTable<Contact>();


    }

    public void Insert(object data)
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
    public List<Contact> Contacts
    {
        get
        {
            var db = new SQLiteConnection(System.IO.Path.Combine(folder, "contacts.db"));
            return db.Query<Contact>("select * from Contact");
        }
    }

}