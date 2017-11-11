using LapsMobileApp.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]
namespace LapsMobileApp.iOS.CustomRenderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var listView = Control as UITableView;

                /*

                listView.LayoutMargins = UIEdgeInsets.Zero;
                listView.CellLayoutMarginsFollowReadableWidth = false;
                listView.SeparatorInset = UIEdgeInsets.Zero;

                */
            }
        }
    }
}
