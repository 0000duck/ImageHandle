﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Application;
using WebApplication.Models;
using WebApplication.Common;
using WebApplication.Filters;

namespace WebApplication.Controllers
{
    public class OrderController : Controller
    {
        private OrderService orderService; 

        public OrderController()
        {
            orderService = new OrderService();
        }

        // GET: Order
        public ActionResult Index()
        {
            var orders = orderService.ListAll();
            return View(orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        

        // GET: Order/Edit/5
        //public ActionResult Update(int id, int status)
        //{
        //    Order order = orderService.GetOrderById(id);
        //    order.Status = status;
        //    if (status == (int)EnumStatus.待生产)
        //    {
        //        order.Auditor = 10; //to do, current login user id.
        //        order.AuditTime = DateTime.Now;
        //    }
        //    else if (status == (int)EnumStatus.生产中)
        //    {
        //        order.Productor = 10; //to do, current login user id.
        //        order.ProductTime = DateTime.Now;
        //    }
        //    else if (status == (int)EnumStatus.已删除)
        //    {
        //        order.DeleteTime = DateTime.Now;
        //    }

        //    orderService.Save(order);

        //    return View(order);
        //}

        // POST: Order/Edit/5
        [HttpPost]
        public ActionResult Update(int id, int status)
        {
            Order order = orderService.GetOrderById(id);
            order.Status = status;
            if (status == (int)EnumStatus.待生产)
            {
                order.Auditor = 10; //to do, current login user id.
                order.AuditTime = DateTime.Now;
            }
            else if (status == (int)EnumStatus.生产中)
            {
                order.Productor = 10; //to do, current login user id.
                order.ProductTime = DateTime.Now;
            }
            else if (status == (int)EnumStatus.已删除)
            {
                order.DeleteTime = DateTime.Now;
            }

            orderService.Save(order); 
            return RedirectToAction("Index");
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Add()
        {
            ViewData["ImageTypeList"] = ImageType.GetAll(); 
            OrderForm orderForm = new OrderForm();
            orderForm.ImageTypes = new ImageTypeModel();
            return View(orderForm);
        }

        [HttpPost]
        public ActionResult Add(OrderForm orderForm)
        {
            orderForm.ExpireTime = DateTime.Now.AddMinutes(20);
            orderForm.formGuid = Guid.NewGuid();
            orderForm.URL = string.Format(@"http://{0}\Front\Create?guid={1}", Request.Url.Authority, orderForm.formGuid);
            CacheHelper.SetCache(orderForm.formGuid.ToString(), orderForm.ImageTypes);
            ViewData["ImageTypeList"] = ImageType.GetAll();
            return View(orderForm);
        }
         
    }
}

 