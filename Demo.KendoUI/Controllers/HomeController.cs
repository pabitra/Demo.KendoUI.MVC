﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.KendoUI.Models;
using EarlyTech.Demo.DataAccess;

namespace Demo.KendoUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
 
            return View();
        }

        public ActionResult About()
        {
           return View();
        }

        public ActionResult Contact()
        {
           return View();
        }

        /// <summary>
        /// Creates new products by inserting the data posted by the Kendo Grid in the database.
        /// </summary>
        /// <param name="products">The products created by the user.</param>
        /// <returns>The inserted products so the Kendo Grid is aware of the database generated ProductID</returns>
        [HttpPost]
        public ActionResult Create(IEnumerable<ProductViewModel> products)
        {
            var result = new List<Product>();

            using (var northwind = new NorthwindEntities())
            {
                //Iterate all created products which are posted by the Kendo Grid
                foreach (var productViewModel in products)
                {
                    // Create a new Product entity and set its properties from productViewModel
                    var product = new Product
                    {
                        ProductName = productViewModel.ProductName,
                        UnitPrice = productViewModel.UnitPrice,
                        UnitsInStock = productViewModel.UnitsInStock,
                        Discontinued = productViewModel.Discontinued
                    };

                    // store the product in the result
                    result.Add(product);

                    // Add the entity
                    northwind.Products.Add(product);
                }

                // Insert all created products to the database
                northwind.SaveChanges();

                // Return the inserted products - the Kendo Grid needs their ProductID which is generated by SQL server during insertion

                return Json(result.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock,
                    Discontinued = p.Discontinued
                })
                .ToList());
            }
        }

        /// <summary>
        /// Reads the available products to provide data for the Kendo Grid
        /// </summary>
        /// <returns>All available products as JSON</returns>
        [HttpPost]
        public ActionResult Read(int take, int skip, IEnumerable<Sort> sort, CustomFilter filter)
        {
            using (var northwind = new NorthwindEntities())
            {
                var result = northwind.Products
                    .OrderBy(p => p.ProductID) // EF requires ordered IQueryable in order to do paging
                    // Use a view model to avoid serializing internal Entity Framework properties as JSON
                    .Select(p => new ProductViewModel
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        UnitsInStock = p.UnitsInStock,
                        Discontinued = p.Discontinued
                    })
                    .ToDataSourceResult(take, skip, sort, filter);

                return Json(result);
            }
        }

        public ActionResult Products()
        {
            var northwind = new NorthwindEntities();
            
                return Json(northwind.Products.Select(m=> new {ProductName=m.ProductName}), JsonRequestBehavior.AllowGet);
            
        }

        /// <summary>
        /// Updates existing products by updating the database with the data posted by the Kendo Grid.
        /// </summary>
        /// <param name="products">The products updated by the user</param>
        [HttpPost]
        public ActionResult Update(IEnumerable<ProductViewModel> products)
        {
            using (var northwind = new NorthwindEntities())
            {
                //Iterate all updated products which are posted by the Kendo Grid
                foreach (var productViewModel in products)
                {
                    // Create a new Product entity and set its properties from productViewModel
                    var product = new Product
                    {
                        ProductID = (int)productViewModel.ProductID,
                        ProductName = productViewModel.ProductName,
                        UnitPrice = productViewModel.UnitPrice,
                        UnitsInStock = productViewModel.UnitsInStock,
                        Discontinued = productViewModel.Discontinued
                    };

                    // Attach the entity
                    northwind.Products.Attach(product);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    // northwind.ObjectStateManager.ChangeObjectState(product, EntityState.Modified);
                }

                // Save all updated products to the database
                northwind.SaveChanges();

                //Return emtpy result
                return Json(null);
            }
        }

        /// <summary>
        /// Destroys existing products by deleting them from the database.
        /// </summary>
        /// <param name="products">The products deleted by the user</param>
        [HttpPost]
        public ActionResult Destroy(IEnumerable<ProductViewModel> products)
        {
            using (var northwind = new NorthwindEntities())
            {
                //Iterate all destroyed products which are posted by the Kendo Grid
                foreach (var productViewModel in products)
                {
                    // Create a new Product entity and set its properties from productViewModel
                    var product = new Product
                    {
                        ProductID = (int)productViewModel.ProductID,
                    };

                    // Attach the entity
                    northwind.Products.Attach(product);
                    // Delete the entity
                    northwind.Products.Remove(product);
                }

                // Delete the products from the database
                northwind.SaveChanges();

                //Return emtpy result
                return Json(null);
            }
        }
    }
}
