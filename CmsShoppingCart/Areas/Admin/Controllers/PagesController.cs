using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declare list of PageVM
            List<PageVM> pagesList;

            // Init the list
            using (Db db = new Db())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            // Return view with list
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPages()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPages(PageVM model)
        {
            // Check Model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // declare the slug
                string slug;

                // init the PageDTO
                PageDTO dto = new PageDTO();

                // DTO Title
                dto.Title = model.Title;

                // Check and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower(); 
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Ensure Title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exist");
                    return View(model);
                }

                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // Save the Dto
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // set tempdata msg
            TempData["SM"] = "You have added a new page!";

            // Redirect
            return RedirectToAction("AddPages");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declare pageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm Page exists
                if(dto == null)
                {
                    return Content("The page does not exist");
                }

                // Init pageVM
                model = new PageVM(dto);  // The convertion is already done in PageVM model via ctor. or
                //model = new PageVM()
                //{
                //    Id = dto.Id,
                //    Title = dto.Title
                //};

            }

            // Return view with model
            return View(model);
        }

        // Post: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // get page id
                int id = model.Id;                 // use hidden field of strongly typed view on the UI

                // declare/init slug
                string slug = "home";

                // get the page
                PageDTO dto = db.Pages.Find(id);   // local variable - id 

                // DTO the Title
                dto.Title = model.Title;

                // Check for slug and set it if need be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // Ensure the Title and slug are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exist");
                    return View(model);
                }

                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save the DTO 
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You have edited the page!";

            // redirect 
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the pages
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exist");
                }

                // Init PageVM
                model = new PageVM(dto);
            }

            // return view with model

            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Remove the page
                db.Pages.Remove(dto);

                // Save
                db.SaveChanges();
            }

            // Redirect
            return RedirectToAction("Index");
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // Set Initial count
                int count = 1;

                // Declare PageDTO
                PageDTO dto;

                // Set Sorting for each page
                foreach(var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }                
            }
        }

        /******************************* SIDEBAR *******************************/

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()   // Retrieve Sidebar form
        {
            // Declare model
            SidebarVM model;

            using (Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);  // hardcoded

                // Init model
                model = new SidebarVM(dto);
            }

            // return view with model
            return View(model);
        }

        // Post: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)  // Edit sidebar information
        {
            using (Db db = new Db())
            {
                // Get the dto
                SidebarDTO dto = db.Sidebar.Find(1);

                // DTO the body
                dto.Body = model.Body;

                // Save
                db.SaveChanges();
            }

            // Set Tempdata message
            TempData["SM"] = "You have edited the sidebar";

            // redirect
            return RedirectToAction("EditSidebar");
        }

    }
}