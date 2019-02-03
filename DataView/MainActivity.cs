using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Http;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using System.Linq;
using Android.Views;
using Newtonsoft.Json;
using Android.Widget;
using System.Threading.Tasks;

namespace DataView
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity , SearchView.IOnQueryTextListener , Spinner.IOnItemSelectedListener
    {
        HttpClient httpClient;
        ObservableCollection<BlogTable> data;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState); httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri("https://www.shazan.ga/api/blogtables"));
            data = new ObservableCollection<BlogTable>( JsonConvert.DeserializeObject<List<BlogTable>>(await result.Content.ReadAsStringAsync()));
            SetContentView(Resource.Layout.activity_main);
            var listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = new BlogTableAdapter(this, data);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            var listmembers = typeof(BlogTable).GetProperties().Select(x => x.Name).Skip(1).ToList();
            var spinadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, listmembers);
            spinadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = spinadapter;
            spinner.OnItemSelectedListener = this;
            var searchview = FindViewById<SearchView>(Resource.Id.searchView1);
            searchview.SetOnQueryTextListener(this);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnQueryTextChange(string newText)
        {
            UpdateListView(newText);
            return true;
        }

        private void UpdateListView(string newText)
        {
            var res = httpClient.GetAsync(new Uri("https://www.shazan.ga/api/blogtables")).GetAwaiter().GetResult();
            var spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            var resdata = JsonConvert.DeserializeObject<List<BlogTable>>(res.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            {
                resdata = resdata.FindAll(x => (x.GetType().GetProperty(spinner.SelectedItem.ToString()).GetValue(x, null) as string).StartsWith(newText));
            }
            RunOnUiThread(() =>
            {
                var adapt = FindViewById<ListView>(Resource.Id.listView).Adapter as BlogTableAdapter;
                adapt.Update(resdata);
                adapt.NotifyDataSetChanged();
            });
        }

        public bool OnQueryTextSubmit(string query)
        {
            return true;
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            var res = httpClient.GetAsync(new Uri("https://www.shazan.ga/api/blogtables")).GetAwaiter().GetResult();
            var searchview = FindViewById<SearchView>(Resource.Id.searchView1);
            var spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            var resdata = JsonConvert.DeserializeObject<List<BlogTable>>(res.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            {
                resdata = resdata.FindAll(x => (x.GetType().GetProperty(spinner.SelectedItem.ToString()).GetValue(x, null) as string).StartsWith(searchview.Query));
            }
            RunOnUiThread(() =>
            {
                var adapt = FindViewById<ListView>(Resource.Id.listView).Adapter as BlogTableAdapter;
                adapt.Update(resdata);
                adapt.NotifyDataSetChanged();
            });
        }

        public void OnNothingSelected(AdapterView parent)
        {
           
        }
    }
    public partial class BlogTable
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}

