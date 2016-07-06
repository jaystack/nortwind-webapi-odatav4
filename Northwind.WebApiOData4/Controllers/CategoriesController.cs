﻿using Northwind;
using Northwind.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Northwind.Controllers
{
    public class CategoriesController : NewsBaseODataController<Category>
    {
        [EnableQuery(MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 10)]
        public IQueryable<Category> Get()
        {
            return db.Categories;
        }

        [EnableQuery]
        public SingleResult<Category> Get([FromODataUri] int key)
        {
            IQueryable<Category> result = db.Categories.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Category Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Categories.Add(Category);
            await db.SaveChangesAsync();
            return Created(Category);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Category> Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Categories.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Category.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }
        
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Category update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }
        
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var Category = await db.Categories.FindAsync(key);
            if (Category == null)
            {
                return NotFound();
            }
            db.Categories.Remove(Category);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public SingleResult<Product> GetProduct([FromODataUri] int key1, [FromODataUri] int key2)
        {
            return SingleResult.Create(db.Categories.Where(c => c.Id == key1).Select(c => c.Products.FirstOrDefault(a=>a.Id == key2)));
        }
        [EnableQuery]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return db.Categories.Where(c => c.Id == key).SelectMany(c => c.Products);
        }
        public string GetTitle([FromODataUri] int key)
        {
            return "catTitle";
        }



        [HttpGet]
        [Route("SFunction1")]
        public List<string> SFunction1([FromODataUri]int p1, [FromODataUri]string p2, [FromODataUri]IEnumerable<string> p3)
        {
            return new List<string>() { "f1_ ", p1.ToString(), p2 }.Concat(p3).ToList();
        }

        [HttpPost]
        [Route("SAction1")]
        public List<string> SAction1(ODataActionParameters param)
        {
            var p1 = (int)param["p1"];
            var p2 = (string)param["p2"];
            var p3 = param["p3"] as IEnumerable<string>;
            return new List<string>() { "a1_ ", p1.ToString(), p2 }.Concat(p3).ToList();
        }


        private bool ProductExists(int key)
        {
            return db.Categories.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
