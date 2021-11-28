using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PayFast;
using PayFast.AspNet;
using ZXing;
using DayCare.Models;

namespace DayCare.Controllers
{
    public class ClassRoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ClassRoomsController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
        // GET: ClassRooms
        public ActionResult Index()
        {
            var uid = User.Identity.GetUserId();
            var classRooms = db.ClassRooms.Include(c => c.Child).Include(c => c.Employee).Where(c=>c.Child.Payment ==true);
            return View(classRooms.ToList());
        }
        [ChildActionOnly]
        public ActionResult NumberCollect()
        {
            var uid = User.Identity.GetUserId();
            ViewData["ChildrenCount"] = db.ClassRooms.Include(c => c.Child).Include(c => c.Employee).Where(c => c.Child.Payment == true && c.Driver_ID == uid && c.Deliverystatus.Status_Name == "At home").Count();
            return PartialView("NumberCollect");
        }

        public ActionResult ClassRoom()
        {
            var uid = User.Identity.GetUserId();
            var classRooms = db.ClassRooms.Include(c => c.Child).Include(c => c.Employee).Where(c => c.Child.Payment == true && c.Employee_Id == uid && c.Deliverystatus.Status_Name == "In class");
            return View(classRooms.ToList());
        }
        public ActionResult WithDriver()
        {
            var uid = User.Identity.GetUserId();
            var classRooms = db.ClassRooms.Include(c => c.Child).Include(c => c.Employee).Where(c => c.Child.Payment == true && c.Driver_ID == uid && c.Deliverystatus.Status_Name == "Out to deliver");
            return View(classRooms.ToList());
        }
        public ActionResult CollectChild()
        {
            var uid = User.Identity.GetUserId();
            var classRooms = db.ClassRooms.Include(c => c.Child).Include(c => c.Employee).Where(c => c.Child.Payment == true && c.Driver_ID == uid && c.Deliverystatus.Status_Name == "At home");
            return View(classRooms.ToList());
        }
        public ActionResult ChildInformation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Models.QRCode qRCode = db.QRCodes.Find(id);
            if (qRCode == null)
            {
                return HttpNotFound();
            }
            return View(qRCode);
        }
        public ActionResult ViewThisPDF(int id)
        {
            var report = new Rotativa.ActionAsPdf("ChildInformation", new { id = id }) { FileName = "Verification.pdf" };
            return report;
        }
        private string GenerateQRCode(string qrcodeText)
		{
			string folderPath = "~/images/";
			string imagePath = "~/images/QrCode.jpg";
			// create new Directory if not exist
			if (!Directory.Exists(Server.MapPath(folderPath)))
			{
				Directory.CreateDirectory(Server.MapPath(folderPath));
			}

			var barcodeWriter = new BarcodeWriter();
			barcodeWriter.Format = BarcodeFormat.QR_CODE;
			var result = barcodeWriter.Write(qrcodeText);

			string barcodePath = Server.MapPath(imagePath);
			var barcodeBitmap = new Bitmap(result);
			using (MemoryStream memory = new MemoryStream())
			{
				using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
				{
					barcodeBitmap.Save(memory, ImageFormat.Jpeg);
					byte[] bytes = memory.ToArray();
					fs.Write(bytes, 0, bytes.Length);
				}
			}
			return imagePath;
		}

		public ActionResult Read()
		{
			return View(ReadQRCode());
		}

		private Models.QRCode ReadQRCode()
		{
			Models.QRCode barcodeModel = new Models.QRCode();
			string barcodeText = "";
			string imagePath = "~/images/QrCode.jpg";
			string barcodePath = Server.MapPath(imagePath);
			var barcodeReader = new BarcodeReader();
			//Decode the image to text
			var result = barcodeReader.Decode(new Bitmap(barcodePath));
			if (result != null)
			{
				barcodeText = result.Text;
			}
			return new Models.QRCode() { QRCodeText = barcodeText, QRCodeImagePath = imagePath };
		}

        public void AddNew(string id)
        {
            Models.QRCode objQR = new Models.QRCode();
            objQR.Child_Id = id;
            objQR.QRCodeText = "https://glorylanddaycare.azurewebsites.net/Beneficiary_Signature/Create/"+ objQR.Child_Id;
            objQR.QRCodeImagePath = GenerateQRCode(objQR.QRCodeText);
            db.QRCodes.Add(objQR);
            db.SaveChanges();
            var parentName = (from i in db.Children
                              where i.Child_Id == objQR.Child_Id
                              select i.Parent.Parent_Name).FirstOrDefault();
            var parentEmail = (from i in db.Children
                               where i.Child_Id == objQR.Child_Id
                               select i.Parent.Parent_Email).FirstOrDefault();
            Email emails = new Email();
            ViewBag.Subject = "Child Information";
            ViewBag.Body = $"Hi {parentName}" + "<br/>" +
                $"This is the final step you need to go through, please follow this link https://glorylanddaycare.azurewebsites.net/ClassRooms/ChildInformation/{objQR.QRId}" + "<br/>" +
                $"On this link you will be able to verify your child's details as well as yours. Please me sure to follow all the instructions." + "<br/>" +
                $"Thank you." + "<br/>" +
                $"Smart DayCare.";

            emails.Gmail(ViewBag.Subject, ViewBag.Body, parentEmail);
        }

        // GET: ClassRooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = db.ClassRooms.Find(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            return View(classRoom);
        }

        // GET: ClassRooms/Create
        public ActionResult Create(string id)
        {
            ClassRoom classRoom = new ClassRoom();
            classRoom.Child_Id = id;
            
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name");
            ViewBag.Employee_Id = new SelectList(db.Employees, "Employee_Id", "Employee_Name");
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name");
            ViewBag.DeliveryStatus_ID = new SelectList(db.Deliverystatuses, "DeliveryStatus_ID", "Status_Name");
            return View(classRoom);
        }

        // POST: ClassRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Room_Id,Room_Date,Fee_Amount,Employee_Id,Child_Id,Driver_ID,DeliveryStatus_ID")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                var Status = (from i in db.Deliverystatuses
                              where i.Status_Name == "At home"
                              select i.DeliveryStatus_ID).SingleOrDefault();
                classRoom.DeliveryStatus_ID = Status;
                classRoom.Room_Date = DateTime.Now;
                var getDayCareStatus = (from i in db.Children
                                  where i.Child_Id == classRoom.Child_Id
                                  select i.AfterCare).FirstOrDefault();
                classRoom.Amount12Months = classRoom.FeeFor12Months(classRoom.Fee_Amount);
                if (getDayCareStatus == true)
                {
                    classRoom.Total_Amount = classRoom.AfterCare(classRoom.Fee_Amount) + classRoom.FeeFor12Months(classRoom.Fee_Amount);
                    classRoom.IfafterCare = classRoom.AfterCare(classRoom.Fee_Amount);
                }
                else
                {
                    classRoom.Total_Amount = classRoom.FeeFor12Months(classRoom.Fee_Amount);
                    classRoom.IfafterCare = 0;
                }

                db.ClassRooms.Add(classRoom);
                db.SaveChanges();
                var parentName = (from i in db.Children
                                  where i.Child_Id == classRoom.Child_Id
                                  select i.Parent.Parent_Name).FirstOrDefault();
                var parentEmail = (from i in db.Children
                                  where i.Child_Id == classRoom.Child_Id
                                  select i.Parent.Parent_Email).FirstOrDefault();
                Email emails = new Email();
                ViewBag.Subject = " Registration Confirmation ";
                ViewBag.Body = $"Hi {parentName}" + "<br/>" +
                    $"Use this link to make payment for your child's fee https://glorylanddaycare.azurewebsites.net/ClassRooms/OnceOff/{classRoom.Room_Id}" + "<br/>" +
                    $"Once the payment has been done, your Child will active in the class that he or she has been assigned to." + "<br/>" +
                    $"Thank you." + "<br/>" +
                    $"Smart DayCare.";

                emails.Gmail(ViewBag.Subject, ViewBag.Body,parentEmail);
                return RedirectToAction("Index","Children");
            }

            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", classRoom.Child_Id);
            ViewBag.Employee_Id = new SelectList(db.Employees, "Employee_Id", "Employee_Name", classRoom.Employee_Id);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", classRoom.Driver_ID);
            ViewBag.DeliveryStatus_ID = new SelectList(db.Deliverystatuses, "DeliveryStatus_ID", "Status_Name", classRoom.DeliveryStatus_ID);
            return View(classRoom);
        }

        // GET: ClassRooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = db.ClassRooms.Find(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", classRoom.Child_Id);
            ViewBag.Employee_Id = new SelectList(db.Employees, "Employee_Id", "Employee_Name", classRoom.Employee.Employee_Name);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", classRoom.Driver_ID);
            ViewBag.DeliveryStatus_ID = new SelectList(db.Deliverystatuses, "DeliveryStatus_ID", "Status_Name", classRoom.DeliveryStatus_ID);
            return View(classRoom);
        }

        // POST: ClassRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Room_Id,Room_Date,Fee_Amount,Employee_Id,Child_Id")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classRoom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", classRoom.Child_Id);
            ViewBag.Employee_Id = new SelectList(db.Employees, "Employee_Id", "Employee_Name", classRoom.Employee_Id);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", classRoom.Driver_ID);
            ViewBag.DeliveryStatus_ID = new SelectList(db.Deliverystatuses, "DeliveryStatus_ID", "Status_Name", classRoom.DeliveryStatus_ID);
            return View(classRoom);
        }

        // GET: ClassRooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = db.ClassRooms.Find(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            return View(classRoom);
        }

        // POST: ClassRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassRoom classRoom = db.ClassRooms.Find(id);
            db.ClassRooms.Remove(classRoom);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Payment(int id)
        {
            return RedirectToAction("OnceOff", new { id = id });
        }

        #region Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            recurringRequest.email_address = "nkosi@finalstride.com";
            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";
            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";
            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;
            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";
            return Redirect(redirectUrl);
        }


        public ActionResult OnceOff(int id)
        {

            //var uid = User.Identity.GetUserId();
            //var appointments = db.Appointments.Include(a => a.Client).Where(x => x.ClientId == uid).Where(a => a.paymentstatus == false).Where(a => a.status == false);
            ClassRoom classRoom = db.ClassRooms.Find(id);



            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            double amount = (double)classRoom.Total_Amount;
            //var products = db.Items.Select(x => x.Item_Name).ToList();
            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = amount;
            onceOffRequest.item_name = "Your appointment Number is " + id;
            onceOffRequest.item_description = "You are now paying your rental fee";
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            Child child = db.Children.Find(classRoom.Child_Id);
            child.Payment = true;
            child.Payment_Date = DateTime.Now;
            AddNew(child.Child_Id);
            db.Entry(child).State = EntityState.Modified;
            db.SaveChanges();
            AddNew(child.Child_Id);
            return Redirect(redirectUrl);
        }


        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion Payment

    }
}
