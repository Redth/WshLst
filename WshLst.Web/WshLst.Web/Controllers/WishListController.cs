using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WshLst.Web.Controllers
{
    public class WishListController : Controller
    {
        //
        // GET: /List/

        public async Task<ActionResult> Index(string id)
        {
			if (string.IsNullOrEmpty(id))
				return RedirectToAction("Empty");


			var viewModel = new ViewModels.WishListViewModel(id);

			await viewModel.LoadList();

			if (viewModel.WishList == null)
				return RedirectToAction("Empty");

			await viewModel.LoadEntries();

			return View(viewModel);
        }

		public ActionResult Empty()
		{
			return View();
		}
    }
}
