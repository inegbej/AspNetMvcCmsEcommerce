using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index(or optione - Pages)
        public ActionResult Index(string page = "")  // partly sets up the front-end of online app
        {
            // Get/Set page slug
            if (page == "")
                page = "home";

            // Declare model and dto
            PageVM model;
            PageDTO dto;

            // Check if page exists
            using (Db db = new Db())
            {
                if (! db.Pages.Any(x => x.Slug.Equals(page))) // if no match of page in the DB
                {
                    return RedirectToAction("Index", new { page = "" });  // return to index or specified parameter
                }
            }

            // Get page dto
            using (Db db = new Db())  // if page exist in the db, return it
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Set Page Title
            ViewBag.PageTitle = dto.Title;

            // Check for sidebar
            if(dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // Init model
            model = new PageVM(dto);

            // return view with model
            return View(model);
        }
        
        public ActionResult PagesMenuPartial()   // retrieve the Pages data in the database
        {
            // Declare a list of PageVM
            List<PageVM> pageVMList;

            // Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }

            // Return partial view
            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Declare model
            SidebarVM model;

            // Init model 
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }

            // Return partial view with model
            return PartialView(model);
        }

    }
}