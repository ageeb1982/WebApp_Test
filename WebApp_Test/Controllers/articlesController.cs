﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp_Test.Models;
using WebApp_Test.Models.Tools;

namespace WebApp_Test.Controllers
{
    /// <summary>
    /// كلاس المواضيع
    /// </summary>
    [Authorize]
    public class articlesController : Controller
    {
        private DB db = new DB();

        /// <summary>
        /// تظهر قائمة المواضيع وهي متاحة لكلا Users 
        ///Admin and User
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin) + "," + nameof(Users_Type.Articles_Viewer))]
        public ActionResult Index(string srch)
        {
            var restult = db.articles.ToList();
            if (!string.IsNullOrEmpty(srch))
            { restult = restult.Where(x => x.Name.ToLower().Contains(srch.ToLower())).ToList(); }
            return View(restult);
        }


/// <summary>
        /// للبحث عن موضوع معين بالإسم في الشاشة نفسها 
        /// AutoComplate
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        /// 
       // [Authorize(Roles = nameof(Users_Type.Admin) + "," + nameof(Users_Type.Articles_Viewer))]
        public JsonResult Srch_Name(string term)
        {
            var dd = Request;
            var listQ = db.articles.ToList();

            if(!string.IsNullOrEmpty(term))
            {
                listQ = listQ.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
            }
         
            return Json(listQ.Select(x=>x.Name),JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        ///تظهر تفاصيل المواضيع وهي متاحة لكلا Users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin) + "," + nameof(Users_Type.Articles_Viewer))]

        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            article article = db.articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }



        /// <summary>
        /// تنشئ موضوع جديد لكنها متاحة للمستخدم 
        /// Admin
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// حفظ الموضوع
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(article article)
        {

            if (db.articles.Any(x => x.Name.Trim() == article.Name.Trim()))
            {
                ModelState.AddModelError("Name", "This name has already been entered");
            }

            if (ModelState.IsValid)
            {
                article.AddTime = DateTime.Now;
                db.articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }

        /// <summary>
        /// تعدل موضوع موجود وهي متاحة للمستخدم
        /// Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            article article = db.articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        /// <summary>
        /// حفظ تعديلات الموضوع
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(article article)
        {


            if (db.articles.Any(x => x.Name.Trim() == article.Name.Trim() && x.Id != article.Id))
            {
                ModelState.AddModelError("Name", "This name has already been entered");
            }

            if (ModelState.IsValid)
            {



                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }
        /// <summary>
        /// تحذف موضوع موجود وهي متاحة للمستخدم 
        /// Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            article article = db.articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        /// <summary>
        /// تأكيد حذف الموضوع
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Users_Type.Admin))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            article article = db.articles.Find(id);
            db.articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        /// <summary>
        /// إنهاء كنترولر المواضيع
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
