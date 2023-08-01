using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {
    private readonly ToDoListContext _db;

    public ItemsController(ToDoListContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Item> model = _db.Items
                            .Include(item => item.Category)
                            .ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Item item, string userDueDate)
    {
      if (!ModelState.IsValid)
      {
        // if not valid, redirect to create page 
        ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
        return View(item);
      }
      else
      {
        // if valid
        // parses date string, may fail given faulty user input
        // or an unexpected locale-specific way of writing dates
        // but this is just practice, so it's okay for now
        item.DueDate = DateTime.Parse(userDueDate);
        _db.Items.Add(item);
        _db.SaveChanges();
        return RedirectToAction("Index");
      }
    }

    public ActionResult Details(int id)
    {
      Item thisItem = _db.Items
                          .Include(item => item.Category)
                          .Include(item => item.JoinEntities)
                          .ThenInclude(join => join.Tag)
                          .FirstOrDefault(item => item.ItemId == id);
      return View(thisItem);
    }

    public ActionResult Edit(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult Edit(Item item)
    {
      _db.Items.Update(item);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      return View(thisItem);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      _db.Items.Remove(thisItem);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddTag(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Title");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult AddTag(Item item, int tagId)
    {
      // check if item-tag relationship already exists
      #nullable enable
      ItemTag? joinEntity = _db.ItemTags.FirstOrDefault(join => (join.TagId == tagId && join.ItemId == item.ItemId));
      #nullable disable

      // if relationship doesn't exist AND at least one tag exists
      if (joinEntity == null && tagId != 0)
      {
        // then create and add the new relationship
        _db.ItemTags.Add(new ItemTag() { TagId = tagId, ItemId = item.ItemId });
        _db.SaveChanges();
      }
      // then redirect to the details page
      return RedirectToAction("Details", new { id = item.ItemId });
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      ItemTag joinEntry = _db.ItemTags.FirstOrDefault(entry => entry.ItemTagId == joinId);
      _db.ItemTags.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult MarkCompleted(int itemId)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == itemId);
      thisItem.Completed = true;
      _db.Items.Update(thisItem);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
