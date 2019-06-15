using MVCeTicaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCeTicaret.Controllers
{
    public class ShoppingController : Controller
    {
        Context db;
        public ShoppingController()
        {
            db = new Context();
        }

        public ActionResult Cart()
        {

            return View(db.OrderDetails.Where(x => x.CustomerID == TemporaryUserData.UserID && x.IsCompleted == false).ToList());
        }

        [HttpPost] 
        public ActionResult AddToCart(int id, FormCollection frm)
        {
            if (Session["OnlineKullanici"] == null)
                return RedirectToAction("Login", "Login");



            int miktar = Convert.ToInt32(frm["quantity"]);

            ControlCart(id, miktar);

            int urunID = id;
            // TODO: CustomerID dinamik olarak çekilecek!..
            return RedirectToAction("ProductDetail", "Product", new { id = urunID });  //HATA
        }

        public ActionResult AddToWishlist(int id)
        {
            if (Session["OnlineKullanici"] == null)
                return RedirectToAction("Login", "Login");

            // TODO: CustomerID dinamik olarak çekilecek!..
            ControlWishlist(id);

            // TODO: CustomerID dinamik olarak çekilecek!..
            return RedirectToAction("ProductDetail", "Product", new { id = id });
        }


        [HttpPost]
        public ActionResult UpdateQuantity(int id, FormCollection frm)
        {
            OrderDetail od = db.OrderDetails.Find(id);
            od.Quantity = int.Parse(frm["quantity"]);
            od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }


        private void ControlWishlist(int id)
        {
             Wishlist wishlist = db.Wishlists.FirstOrDefault(x => x.ProductID == id && x.CustomerID == TemporaryUserData.UserID && x.IsActive == true);

            if (wishlist == null)
            {
                wishlist = new Wishlist();
                wishlist.ProductID = id;
                wishlist.CustomerID = TemporaryUserData.UserID; // TODO: CustomerID dinamik olarak çekilecek!..
                wishlist.IsActive = true;

                db.Wishlists.Add(wishlist);
                db.SaveChanges();
            }
        }

        public ActionResult RemoveFromCart(int id)
        {

            db.OrderDetails.Remove(db.OrderDetails.Find(id));
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }
       

        public ActionResult AddToWishlistFromCart(int id)
        {
            int productID = db.OrderDetails.Find(id).ProductID;  //hatavar
            ControlWishlist(productID);

            db.OrderDetails.Remove(db.OrderDetails.Find(id));
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Wishlist()
        {
            return View(db.Wishlists.Where(x=>x.CustomerID==TemporaryUserData.UserID&&x.IsActive==true).ToList());
        }

        public ActionResult RemoveFromWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            wishlist.IsActive = false;
            wishlist.IsActive = false;
            db.SaveChanges();


            return RedirectToAction("Wishlist","Shopping");
        }

        public ActionResult AddToCartFromWishlist(int id)
        {
            int productId = db.Wishlists.Find(id).ProductID;
            ControlCart(productId);

            Wishlist wishlist = db.Wishlists.Find(id);
            wishlist.IsActive = false;
            db.SaveChanges();

            return RedirectToAction("Wishlist","Shopping");
        }

        public void ControlCart(int id, int miktar= 1)
        {
            OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id && x.IsCompleted == false && x.CustomerID == TemporaryUserData.UserID).FirstOrDefault();

            if (od == null)
            {
                od = new OrderDetail();
                od.ProductID = id;
                od.CustomerID = TemporaryUserData.UserID;
                od.IsCompleted = false;
                od.UnitPrice = db.Products.Find(id).UnitPrice;
                od.Discount = db.Products.Find(id).Discount;
                od.OrderDate = DateTime.Now;

                if (db.Products.Find(id).UnitsInStock >= miktar)
                    od.Quantity = miktar;
                else
                    od.Quantity = db.Products.Find(id).UnitsInStock;
                od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);
                db.OrderDetails.Add(od);

            }
            else
            {
                if (db.Products.Find(id).UnitsInStock > od.Quantity + miktar)
                {
                    od.Quantity += miktar;
                    od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);
                }
            }
            db.SaveChanges();
        }
    }

}