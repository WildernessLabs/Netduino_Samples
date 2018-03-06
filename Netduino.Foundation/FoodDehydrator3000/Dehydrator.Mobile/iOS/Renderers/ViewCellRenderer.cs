using Xamarin.Forms;
using Dehydrator.Mobile.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(StandardViewCellRenderer))]
namespace Dehydrator.Mobile.iOS
{
	public class StandardViewCellRenderer : ViewCellRenderer
	{

		public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			switch (item.StyleId)
			{
				default:
				case "none":
					cell.Accessory = UIKit.UITableViewCellAccessory.None;
					break;
				case "checkmark":
					cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
					break;
				case "detail-button":
					cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
					break;
				case "detail-disclosure-button":
					cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
					break;
				case "disclosure":
					cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
					break;
			}
			return cell;
		}

	}
}