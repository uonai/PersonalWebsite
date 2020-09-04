using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Website.Controllers
{
    public class StripeController : Controller
    {
        private int amount = 100;
        private readonly string WebhookSecret = "whsec_OurSigningSecret";
        public IActionResult Index()
        {
            ViewBag.PaymentAmount = amount;
            return View("~/Views/Home/Donate.cshtml");
        }

        [HttpPost]
        public IActionResult Processing(string stripeToken, string stripeEmail)
        {
            Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("Product", "RubberDuck");
            Metadata.Add("Quantity", "10");
            var options = new ChargeCreateOptions
            {
                Amount = amount,
                Currency = "USD",
                Description = "Buying 10 rubber ducks",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,
                Metadata = Metadata
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
            return View("~/Views/Home/Donate.cshtml");
        }

        [HttpPost]
        public IActionResult ChargeChange()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            //try
            //{
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], WebhookSecret, throwOnApiVersionMismatch: true);
                Charge charge = (Charge)stripeEvent.Data.Object;
                switch (charge.Status)
                {
                    case "succeeded":
                        //This is an example of what to do after a charge is successful
                        // charge.Metadata.TryGetValue("Product", out string Product);
                        // charge.Metadata.TryGetValue("Quantity", out string Quantity);
                        // Database.ReduceStock(Product, Quantity);
                        Console.WriteLine(charge);
                        break;
                    case "failed":
                        //Code to execute on a failed charge
                        break;
                }
            //}
            //catch (Exception)
            //{
            //    // e.Ship(HttpContext);
            //    return BadRequest();
            //}
            return Ok();
        }
    }
}
