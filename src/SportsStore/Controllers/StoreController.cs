using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Domain;

namespace SportsStore.Controllers
{
    public class StoreController : Controller
    {

        private readonly IProductRepository _productRepository;

       public StoreController(IProductRepository productsRepository)
        {
            _productRepository = productsRepository;
        }

        public IActionResult Index()
        {
            return View(
                _productRepository.GetByAvailability(
                    new List<Availability>() { Availability.ShopAndOnline, Availability.OnlineOnly}));
        }

    }
}
