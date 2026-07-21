using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using LaundryMVC.Models;
using LaundryMVC.Repository.IRepo;
using LaundryMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace LaundryMVC.Controllers
{
    public class LaunController : Controller
    {
        private readonly ILauIRepo _cont;

        public LaunController(ILauIRepo repo)
        {
            _cont = repo;
        }



        public async Task<IActionResult> Index()
        {
            var items = await _cont.GetAllItems();
            return View(items);
        }

       

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order)
        {
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("CustomerLogin", "Auth");
            }

            order.UserId = int.Parse(userId);

            var items = await _cont.GetAllItems();

            var selectedItem = items.FirstOrDefault(x => x.ItemName == order.ItemName);

            if (selectedItem != null)
            {
                order.Price = selectedItem.Price;
                order.TotalPrice = selectedItem.Price * order.Quantity;
            }

            HttpContext.Session.SetString("TempOrderItem", order.ItemName);
            HttpContext.Session.SetString("TempOrderQty", order.Quantity.ToString());
            HttpContext.Session.SetString("TempOrderPrice", order.Price.ToString());
            HttpContext.Session.SetString("TempOrderTotal", order.TotalPrice.ToString());

            return View("OrderSummary", order);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
            {
                ModelState.AddModelError("", "Please select payment method");
                return View("OrderSummary", order);
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("CustomerLogin", "Auth");
            }

            var email = HttpContext.Session.GetString("UserEmail");

            order.UserId = int.Parse(userId);
            order.Email = email;
            order.Status = "Pending";

            await _cont.SaveOrder(order);

            HttpContext.Session.SetString("LastOrderItem", order.ItemName);
            HttpContext.Session.SetString("LastOrderQty", order.Quantity.ToString());
            HttpContext.Session.SetString("LastOrderPrice", order.TotalPrice.ToString());
            HttpContext.Session.SetString("LastOrderPayment", order.PaymentMethod);
            HttpContext.Session.SetString("LastOrderEmail", order.Email);

            return RedirectToAction("Invoice");
        }

        public IActionResult Invoice()
        {
            var order = new Order
            {
                ItemName = HttpContext.Session.GetString("LastOrderItem"),

                Quantity = int.Parse(
                    HttpContext.Session.GetString("LastOrderQty")
                ),

                TotalPrice = decimal.Parse(
                    HttpContext.Session.GetString("LastOrderPrice")
                ),

                PaymentMethod = HttpContext.Session.GetString("LastOrderPayment"),

                Email = HttpContext.Session.GetString("LastOrderEmail")
            };

            return View(order);
        }

        

        [HttpPost]
        public async Task<IActionResult> SendInvoiceEmail(Order model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                TempData["Message"] = "Email is missing!";
                return View("Invoice", model);
            }

            var emailService = new EmailService();

            var body = new StringBuilder();

            body.Append("<h2>Laundry Invoice</h2>");
            body.Append($"<p><b>Item:</b> {model.ItemName}</p>");
            body.Append($"<p><b>Quantity:</b> {model.Quantity}</p>");
            body.Append($"<p><b>Total Price:</b> ₹ {model.TotalPrice}</p>");
            body.Append($"<p><b>Payment Method:</b> {model.PaymentMethod}</p>");
            body.Append("<hr/>");
            body.Append("<h3 style='color:green;'>Order Confirmed ✅</h3>");

            var pdfBytes = GeneratePdf(model);

            await emailService.SendInvoiceEmail(
                model.Email,
                "Laundry Invoice",
                body.ToString(),
                pdfBytes
            );

            TempData["Message"] = "Invoice sent successfully!";

            return View("Invoice", model);
        }

        public byte[] GeneratePdf(Order model)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);

                PdfDocument pdf = new PdfDocument(writer);

                Document document = new Document(pdf);

                document.Add(
                    new Paragraph("Laundry Invoice")
                    .SetBold()
                    .SetFontSize(20)
                );

                document.Add(new Paragraph(" "));

                document.Add(new Paragraph($"Item: {model.ItemName}"));
                document.Add(new Paragraph($"Quantity: {model.Quantity}"));
                document.Add(new Paragraph($"Total Price: ₹ {model.TotalPrice}"));
                document.Add(new Paragraph($"Payment Method: {model.PaymentMethod}"));

                document.Add(new Paragraph(" "));

                document.Add(
                    new Paragraph("Order Confirmed ✅")
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.GREEN)
                );

                document.Close();

                return ms.ToArray();
            }
        }

        [HttpPost]
        public IActionResult DownloadInvoicePdf(Order model)
        {
            var pdfBytes = GeneratePdf(model);

            return File(
                pdfBytes,
                "application/pdf",
                "LaundryInvoice.pdf"
            );
        }

      

        public async Task<IActionResult> OrderHistory()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(
                    "CustomerLogin",
                    "Auth"
                );
            }

            int id = int.Parse(userId);

            var orders = await _cont.GetOrdersByUserId(id);

            return View(orders);
        }

       

        [HttpPost]
        public async Task<IActionResult> AddMultipleOrders(
            List<Order> items,
            List<string> selectedItems)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("CustomerLogin", "Auth");
            }

            int id = int.Parse(userId);

            var dbItems = await _cont.GetAllItems();

            List<Order> finalOrders = new List<Order>();

            foreach (var item in items)
            {
                if (selectedItems != null &&
                    selectedItems.Contains(item.ItemName))
                {
                    var dbItem = dbItems.FirstOrDefault(
                        x => x.ItemName == item.ItemName
                    );

                    if (dbItem != null)
                    {
                        var order = new Order
                        {
                            UserId = id,
                            ItemName = item.ItemName,
                            Quantity = item.Quantity,
                            Price = dbItem.Price,
                            TotalPrice = dbItem.Price * item.Quantity
                        };

                        finalOrders.Add(order);
                    }
                }
            }

            if (finalOrders.Count == 0)
            {
                TempData["Message"] =
                    "Please select at least one item";

                return RedirectToAction("Index");
            }

            var json =
                System.Text.Json.JsonSerializer.Serialize(finalOrders);

            HttpContext.Session.SetString("MultiOrders", json);

            return View("OrderSummaryMultiple", finalOrders);
        }

   

        [HttpGet]
        public IActionResult OrderSummaryMultiple()
        {
            var json =
                HttpContext.Session.GetString("MultiOrders");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            var orders =
                System.Text.Json.JsonSerializer
                .Deserialize<List<Order>>(json);

            return View(orders);
        }

       

        [HttpPost]
        public async Task<IActionResult> PaymentMultiple(
            string PaymentMethod,
            decimal GrandTotal)
        {
            if (string.IsNullOrWhiteSpace(PaymentMethod))
            {
                TempData["Message"] =
                    "Please select payment method";

                return RedirectToAction("OrderSummaryMultiple");
            }

            var userId =
                HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(
                    "CustomerLogin",
                    "Auth"
                );
            }

            var json =
                HttpContext.Session.GetString("MultiOrders");

            if (string.IsNullOrEmpty(json))
            {
                TempData["Message"] =
                    "No orders found";

                return RedirectToAction("Index");
            }

            var orders =
                System.Text.Json.JsonSerializer
                .Deserialize<List<Order>>(json);

            foreach (var order in orders)
            {
                order.UserId = int.Parse(userId);
                order.PaymentMethod = PaymentMethod;
                order.Status = "Pending";

                await _cont.SaveOrder(order);
            }

            HttpContext.Session.SetString(
                "MultiPayment",
                PaymentMethod
            );

            HttpContext.Session.SetString(
                "MultiGrandTotal",
                GrandTotal.ToString()
            );

            return RedirectToAction("InvoiceMultiple");
        }

        

        public IActionResult InvoiceMultiple()
        {
            var json =
                HttpContext.Session.GetString("MultiOrders");

            var payment =
                HttpContext.Session.GetString("MultiPayment");

            var total =
                HttpContext.Session.GetString("MultiGrandTotal");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            var orders =
                System.Text.Json.JsonSerializer
                .Deserialize<List<Order>>(json);

            ViewBag.Payment = payment;
            ViewBag.Total = total;

            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> SendMultipleInvoiceEmail(
            decimal GrandTotal,
            string Payment)
        {
            var email =
                HttpContext.Session.GetString("UserEmail");

            var json =
                HttpContext.Session.GetString("MultiOrders");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            var orders =
                System.Text.Json.JsonSerializer
                .Deserialize<List<Order>>(json);

            var emailService = new EmailService();

            var body = new StringBuilder();

            body.Append("<h2>Laundry Invoice</h2>");

            foreach (var item in orders)
            {
                body.Append(
                    $"<p>{item.ItemName} " +
                    $"Qty:{item.Quantity} " +
                    $"₹{item.TotalPrice}</p>"
                );
            }

            body.Append($"<h3>Total: ₹ {GrandTotal}</h3>");
            body.Append($"<p>Payment: {Payment}</p>");

            var pdfBytes =
                GenerateMultiplePdf(
                    orders,
                    GrandTotal,
                    Payment
                );

            await emailService.SendInvoiceEmail(
                email,
                "Laundry Invoice",
                body.ToString(),
                pdfBytes
            );

            TempData["Message"] =
                "Invoice sent successfully!";

            return RedirectToAction("InvoiceMultiple");
        }

        

        public byte[] GenerateMultiplePdf(
            List<Order> orders,
            decimal GrandTotal,
            string Payment)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);

                PdfDocument pdf = new PdfDocument(writer);

                Document document = new Document(pdf);

                document.Add(
                    new Paragraph("Laundry Invoice")
                    .SetBold()
                    .SetFontSize(20)
                );

                document.Add(new Paragraph(" "));

                foreach (var item in orders)
                {
                    document.Add(
                        new Paragraph(
                            $"{item.ItemName} | " +
                            $"Qty:{item.Quantity} | " +
                            $"₹{item.TotalPrice}"
                        )
                    );
                }

                document.Add(new Paragraph(" "));

                document.Add(
                    new Paragraph(
                        $"Grand Total: ₹ {GrandTotal}"
                    )
                );

                document.Add(
                    new Paragraph($"Payment: {Payment}")
                );

                document.Close();

                return ms.ToArray();
            }
        }
    }
}
