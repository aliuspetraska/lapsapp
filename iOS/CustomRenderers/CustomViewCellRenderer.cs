using System;
using LapsMobileApp.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewCellRenderer))]
namespace LapsMobileApp.iOS.CustomRenderers
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        public CustomViewCellRenderer() { }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);

            if (cell != null)
            {
                cell.LayoutMargins = UIEdgeInsets.Zero;
                cell.PreservesSuperviewLayoutMargins = false;

                cell.ContentView.LayoutMargins = UIEdgeInsets.Zero;
                cell.ContentView.PreservesSuperviewLayoutMargins = false;

                cell.SeparatorInset = UIEdgeInsets.Zero;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }

            return cell;
        }
    }
}
