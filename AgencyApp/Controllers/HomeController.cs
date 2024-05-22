using AgencyApp.Models;
using Business.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgencyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IPortfolioServices _portfolioServices;
        public HomeController(ILogger<HomeController> logger, IPortfolioServices portfolioServices)
        {
            _logger = logger;
            _portfolioServices = portfolioServices;
        }

        public IActionResult Index()
        {
            var portfolio=_portfolioServices.GetAllPortfolio();
            return View(portfolio);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}