using Business.Exceptions;
using Business.Services.Abstracts;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using FileNotFoundException = Business.Exceptions.FileNotFoundException;

namespace AgencyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class PortfolioController : Controller
    {
        private readonly IPortfolioServices _portfolioServices;

        public PortfolioController(IPortfolioServices portfolioServices)
        {
            _portfolioServices = portfolioServices;
        }

        public IActionResult Index()
        {
            var listPortfolio=_portfolioServices.GetAllPortfolio();
            return View(listPortfolio);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Portfolio portfolio)
        {
            if(!ModelState.IsValid)
            {
                return View(portfolio);
            }
            if(portfolio == null)
            {
                return NotFound();
            }
            try
            {
                _portfolioServices.AddPortfolio(portfolio);
            }
            catch (EntityNullException ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (FileSizeException ex)
            {
                ModelState.AddModelError(ex.PropertyName,ex.Message);
                return View();
            }
            catch(ContentTypeException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int id)
        {
            var portfolio=_portfolioServices.GetPortfolio(x=>x.Id == id);
            if(portfolio == null)
            {
                return NotFound();
            }

            try
            {
                _portfolioServices.DeletePortfolio(portfolio.Id);
            }
            catch (EntityNullException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return NotFound(ex.Message);

            }
            catch (FileNotFoundException ex)
            {
                ModelState.AddModelError(ex.PropertyName,ex.Message);
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("",ex.Message);
                return NotFound(ex.Message);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            var portfolio = _portfolioServices.GetPortfolio(x=>x.Id == id);
            if(portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }
        [HttpPost]
        public IActionResult Update(Portfolio portfolio)
        {
            if (!ModelState.IsValid)
            {
                return View(portfolio);
            }
            if (portfolio == null)
            {
                return NotFound();
            }


            try
            {
                _portfolioServices.UpdatePortfolio(portfolio.Id,portfolio);
            }
            catch (EntityNullException ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch(FileSizeException ex)
            {
                ModelState.AddModelError(ex.PropertyName,ex.Message);
                return View();
            }
            catch(ContentTypeException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch(FileNotFoundException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest();
            }
            return RedirectToAction("Index");
        }
    }
}
