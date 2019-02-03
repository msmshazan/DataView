using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace DataView
{
    public class BlogTableAdapter : BaseAdapter<BlogTable>
    {
        private readonly IList<BlogTable> _items;
        private readonly Context _context;

        public BlogTableAdapter(Context context, IList<BlogTable> items)
        {
            _items = items;
            _context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var view = convertView;

            if (view == null)
            {
                var inflater = LayoutInflater.FromContext(_context);
                view = inflater.Inflate(Resource.Layout.row, parent, false);
            }

            view.FindViewById<TextView>(Resource.Id.id).Text = item.Id.ToString();
            view.FindViewById<TextView>(Resource.Id.author).Text = item.Author;
            view.FindViewById<TextView>(Resource.Id.content).Text = item.Content;
            view.FindViewById<TextView>(Resource.Id.title).Text = item.Title;

            return view;
        }

        public void Update(IList<BlogTable> items)
        {
            _items.Clear();
            foreach (var item in items)
            {
                _items.Add(item);
            }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override BlogTable this[int position]
        {
            get { return _items[position]; }
        }
    }
}

