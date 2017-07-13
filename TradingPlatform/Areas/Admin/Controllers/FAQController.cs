using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TradingPlatform.Models.PageModel;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class FAQController : Controller
    {
        private FAQDataLayer dl = new FAQDataLayer();
        public ActionResult Index(int? groupId = null)
        {
            var model = dl.GetHelpGroups(groupId);

            List<SelectListItem> groups = new List<SelectListItem>();
            groups.Add(new SelectListItem()
            {
                Text = "Все",
                Value = "0",
                Selected = !groupId.HasValue
            });
            groups.AddRange(dl.GetHelpGroupsDdl(groupId));
            ViewBag.Group = groups;
            ViewBag.GroupId = groupId.HasValue ? groupId.Value : 0;
            return View(model);
        }

        public ActionResult CreateGroup()
        {
            HelpGroupViewModel model = new HelpGroupViewModel();
            model.OrderId = dl.CalculateGroupOrderId();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateGroup(HelpGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dl.CreateGroup(model);
                    return RedirectToAction("Index", new { groupId = model.Id });
                }
                catch (Exception err)
                {
                    ModelState.AddModelError("", err.Message);
                }
            }
            return View(model);
        }

        public ActionResult EditGroup(int id)
        {
            HelpGroupViewModel model = dl.GetHelpGroup(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditGroup(HelpGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dl.UpdateGroup(model);
                    return RedirectToAction("Index", new { groupId = model.Id });
                }
                catch (Exception err)
                {
                    ModelState.AddModelError("", err.Message);
                }
            }

            return View(model);
        }

        public ActionResult DeleteGroup(int id)
        {
            try
            {
                dl.DeleteGroup(id);
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create(int? groupId = null)
        {
            HelpViewModel model = new HelpViewModel();
            List<SelectListItem> group = dl.GetHelpGroupsDdl(groupId);

            model.OrderId = 1;
            if (groupId.HasValue)
            {
                model.OrderId = dl.CalculateOrderId(groupId.Value);
                model.HelpGroupId = groupId.Value;
            }

            ViewBag.Group = group;

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(HelpViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dl.Create(model);
                    return RedirectToAction("Index", new { groupId = model.HelpGroupId });
                }
                catch (Exception err)
                {
                    ModelState.AddModelError("", err.Message);
                }
            }

            ViewBag.Group = dl.GetHelpGroupsDdl(model.HelpGroupId);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            HelpViewModel model = dl.GetHelp(id);
            ViewBag.Group = dl.GetHelpGroupsDdl(model.HelpGroupId);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(HelpViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dl.Update(model);
                    return RedirectToAction("Index", new { groupId = model.HelpGroupId });
                }
                catch (Exception err)
                {
                    ModelState.AddModelError("", err.Message);
                }
            }

            ViewBag.Group = dl.GetHelpGroupsDdl(model.HelpGroupId);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                dl.Delete(id);
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
            }
            return RedirectToAction("Index");
        }
    }
}