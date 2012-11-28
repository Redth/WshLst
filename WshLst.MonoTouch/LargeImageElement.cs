using System;
using System.Collections.Generic;
using System.Drawing;
using CrossUI.Touch.Dialog.Elements;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WshLst.MonoTouch
{
	public class LargeImageElement : Element, IElementSizing
	{
		public LargeImageElement() : base(string.Empty)
		{
		}

		UIImage image = null;

		public UIImage Image
		{ 
			get { return image; }
			set
			{ 
				image = value;
	
				if (ivImg != null)
					this.ivImg.Image = this.image; 
			}
		}

		UITableViewCell cellImg;
		UIImageView ivImg;
		float hBorder = 15f;
		float vBorder = 5f;
		float cellHeight = 300f;

		protected override UITableViewCell GetCellImpl(UITableView tv)
		{
			if (cellImg == null)
			{
				var cWidth = (tv.Bounds.Width - (2 * hBorder));
                				
				cellImg = new UITableViewCell(UITableViewCellStyle.Default, "LargeImagesCell");
			
				ivImg = new UIImageView();
				ivImg.Frame = new RectangleF(hBorder, vBorder, cWidth, cellHeight - (vBorder * 2));
				ivImg.ContentMode = UIViewContentMode.ScaleAspectFit;

				ivImg.Tag = 101;

				cellImg.Add(ivImg);
			} 

			ivImg.Image = this.Image;

			return cellImg;
		}
	
		public float GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			return cellHeight;
		}
	}
}

