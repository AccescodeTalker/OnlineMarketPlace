using System;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using WebAppMVCProject.Models;
using PagedList.Mvc;



namespace WebAppMVCProject.Controllers
{
    public class AdminController : Controller
    {
        ProjectEntities1 db = new ProjectEntities1();
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(admin avm)
        {
            admin ad = db.admins.Where(x => x.adName == avm.adName && x.adPass == avm.adPass).SingleOrDefault();
            if(ad != null ) 
            {
                Session["id"]=ad.adId.ToString();
                return RedirectToAction("create");
            }
            else
            {
                ViewBag.error = "Invalid User or Password";
            }

            return View();
        }

        public ActionResult create()
        {
            if(Session["id"] ==null)
            {
                return RedirectToAction("login");
            }

            return View();
        }


        [HttpPost]
        public ActionResult Create(category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                
                category cat = new category();
                cat.cName = cvm.cName;
                cat.cimg = path;
                cat.CFKadmin = Convert.ToInt32(Session["id"].ToString());
                cat.cstatus = 1;
                db.categories.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }

            return View();
        } 

        public ActionResult ViewCategory(int ?page)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list =db.categories.Where(x=>x.cstatus== 1).OrderByDescending(x=>x.cId).ToList();
            IPagedList<category> stu =list.ToPagedList(pageindex,pagesize); 

            return View(stu);
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                          ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }
    }
}