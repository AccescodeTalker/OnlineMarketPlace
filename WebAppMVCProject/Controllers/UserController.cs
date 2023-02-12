using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebAppMVCProject.Models;
using static System.Collections.Specialized.BitVector32;

namespace WebAppMVCProject.Controllers
{
    public class UserController : Controller
    {
        ProjectEntities1 db = new ProjectEntities1();
        // GET: User
        public ActionResult Index(int? page)
        {

            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.categories.Where(x => x.cstatus == 1).OrderByDescending(x => x.cId).ToList();
            IPagedList<category> stu = list.ToPagedList(pageindex, pagesize);

            return View(stu);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(user uvm)
        {
            user u = new user();
            u.uName = uvm.uName;
            u.uemail = uvm.uemail;
            u.uPass = uvm.uPass;
            u.ucon = uvm.ucon;
            db.users.Add(u);
            db.SaveChanges();
            return RedirectToAction("login");
        }

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(user uvm)
        {

            user ad = db.users.Where(x => x.uemail == uvm.uemail && x.uPass == uvm.uPass).SingleOrDefault();
            if (ad != null)
            {
                Session["uid"] = ad.uId.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid User or Password";
            }

            return View();
        }

        public ActionResult Ad()
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cId", "cName");
            return View();
        }

        [HttpPost]
        public ActionResult Ad(product pvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                product p = new product();
                p.pName = pvm.pName;
                p.pprice = pvm.pprice;
                p.pimg = path;
                p.pFKcat = pvm.pFKcat;
                p.pdesc = pvm.pdesc;
                p.pFKuser = Convert.ToInt32(Session["uid"].ToString());
                db.products.Add(p);
                db.SaveChanges();
                Response.Redirect("Index");
            }
            return View();
        }

        // confirming images
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


        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.products.Where(x => x.pFKcat == id).OrderByDescending(x => x.pId).ToList();
            IPagedList<product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);

        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.products.Where(x => x.pName.Contains(search)).OrderByDescending(x => x.pId).ToList();
            IPagedList<product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);

        }

        public ActionResult ViewAd(int? id)
        {
            AddViewmodel ad = new AddViewmodel();
            product p = db.products.Where(x => x.pId == id).SingleOrDefault();
            ad.pId = p.pId;
            ad.pName = p.pName;
            ad.pimg = p.pimg;
            ad.pprice = p.pprice;

            category ca = db.categories.Where(x => x.cId == p.pFKcat).SingleOrDefault();
            ad.cName = ca.cName;

            user us = db.users.Where(x => x.uId == p.pFKuser).SingleOrDefault();
            ad.uName = us.uName;
            ad.ucon = us.ucon;
            ad.pFKuser = us.uId;

            return View(ad);
        }
        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAd(int? id)
        {
            product p = db.products.Where(x => x.pId == id).SingleOrDefault();
            db.products.Remove(p);
            db.SaveChanges();
            return View("Index");
        }

        public ActionResult AdToCart(int ?id)
        {
            TempData.Keep();
            product p = db.products.Where(x=>x.pId==id).SingleOrDefault();

            return View(p);
        }
        List<cart> li = new List<cart>();

        [HttpPost]
        public ActionResult AdToCart(product po,string qty ,int? id)
        {
            product p = db.products.Where(x => x.pId == id).SingleOrDefault();
            cart c = new cart();
            c.Id=p.pId;
            c.Name=p.pName;
            c.price = (float)p.pprice;
            c.qty=Convert.ToInt32(qty);
            c.bill = c.price * c.qty;

            if (TempData["cart"]==null)
            {
                li.Add(c);
                TempData["cart"] = li;
            }
            else
            {
                List<cart> l = TempData["cart"]as List<cart>;
                int flag = 0;
                foreach(var item in l)
                {
                    if(item.Id == c.Id)
                    {
                        item.qty += c.qty;
                        item.bill = c.bill;
                        flag=1;
                    }
                }
                if(flag==0)
                {
                    l.Add(c);
                }
                TempData["cart"] = l;
            }


            TempData.Keep();

            return RedirectToAction("Index");
        }
        public ActionResult remove(int ?id)
        {
            List<cart> l = TempData["cart"] as List<cart>;
            cart c = l.Where(x=>x.Id==id).SingleOrDefault();
            l.Remove(c);
            float h = 0;
            foreach(var item in l)
            {
                h += item.bill;
            }
            TempData["total"] = h;

            return RedirectToAction("check");
        }

            public ActionResult Checkout()
        {
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<cart> li = TempData["cart"] as List<cart>;
                foreach (var item in li)
                {
                    x += item.bill;
                }
                TempData["total"] = x;
            }

            TempData.Keep();
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(order O, FormCollection form)
        {
            Random r = new Random();
            TempData["random"] = r.Next();

            List<cart> li = TempData["cart"] as List<cart>;

            invoice iv = new invoice();
            iv.iFKuser = Convert.ToInt32(Session["uid"].ToString());
            iv.iDate=System.DateTime.Now;
            iv.itbill = (float)TempData["total"];
            iv.Addresss = form["Address"]; 
            db.invoices.Add(iv);
            db.SaveChanges();

            foreach(var item in li)
            {
                order od = new order ();
                od.oFKpro = item.Id;
                od.oFKinvoice =iv.iid;
                od.odate= System.DateTime.Now;
                od.ouprice = (int)item.price;
                od.obill= item.bill;
                db.orders.Add(od);
                db.SaveChanges();
            }
            TempData.Remove("total");
            TempData.Remove("cart");

            TempData["msg"] = "Transion completed...";
            TempData.Keep();
            return RedirectToAction("Index");
        }
    }
}