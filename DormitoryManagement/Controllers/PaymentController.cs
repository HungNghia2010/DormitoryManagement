using DormitoryManagement.Areas.Admin.Data;
using DormitoryManagement.Models;
using DormitoryManagement.Models.Payment;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class PaymentController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Payment
        public ActionResult Index()
        {
            var idUser = Convert.ToInt32(Session["idUser"]);




            var data = (from sf in _db.StudentFees
                        join r in _db.Rooms on sf.RoomId equals r.RoomID
                        join b in _db.Buildings on r.BuildingID equals b.BuildingID
                        join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                        join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                        where sa.StudentID == idUser
                        orderby sf.PaymentId descending
                        select new FeeData
                        {
                            RoomName = r.Name,
                            BuildingName = b.Name,
                            PaymentStatus = sf.PaymentStatus,
                            Descript = fp.Description,
                            PaymentId = sf.PaymentId,
                            TotalAmount = sf.TotalAmount,
                            FullName = sa.FullName,
                            StudentID = sa.StudentID,
                            MonthYear = fp.MonthYear,
                            DueDate = fp.DueDate,
                            ExpiryDate = fp.ExpiryDate,
                            ID = sf.Id,
                            Note = sf.Description
                        }).ToList();

            return View(data);
        }

        // GET: ViewPayment
        public ActionResult ViewPayment(int id)
        {
            return View(GetFeeDataById(id));
        }

        public FeeData GetFeeDataById(int id)
        {
            var data = (from sf in _db.StudentFees
                        join r in _db.Rooms on sf.RoomId equals r.RoomID
                        join b in _db.Buildings on r.BuildingID equals b.BuildingID
                        join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                        join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                        where sf.Id == id
                        select new FeeData
                        {
                            RoomName = r.Name,
                            BuildingName = b.Name,
                            PaymentStatus = sf.PaymentStatus,
                            Descript = fp.Description,
                            PaymentId = sf.PaymentId,
                            TotalAmount = sf.TotalAmount,
                            FullName = sa.FullName,
                            StudentID = sa.StudentID,
                            MonthYear = fp.MonthYear,
                            ID = sf.Id,
                            Note = sf.Description
                        }).ToList();

            return data.FirstOrDefault();
        }

        public ActionResult Payment(FeeData feeData)
        {
            return Redirect(Pay(feeData.ID));
        }

        public string Pay(int id)
        {
            var data = _db.StudentFees.Find(id);
            string cleanedParam = data.TotalAmount.Replace(",", "");
            int s;
            int.TryParse(cleanedParam, out s);

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", PayLib.VERSION); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (s*100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo",data.Id.ToString()); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        public ActionResult PayModal(int id)
        {
            return Redirect(Pay(id));
        }
        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                int ID = (int)Convert.ToInt64(pay.GetResponseData("vnp_OrderInfo"));

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, vnp_HashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        var data = _db.StudentFees.Find(ID);
                        data.PaymentStatus = "Đã thanh toán";
                        _db.SaveChanges();
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }


        public ActionResult PayMomo(int id)
        {
            var data = _db.StudentFees.Find(id);
            string cleanedParam = data.TotalAmount.Replace(",", "");

            var test = _db.FeePayments.Find(data.PaymentId);

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh Toán Học Phí Ký túc xá tháng " + test.MonthYear;
            string returnUrl = "https://localhost:44342/Payment/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test


            string amount = cleanedParam;
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = data.Id.ToString();

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());

        }

        public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rRequest = result.requestId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            string s = result.extraData;
            var data = _db.StudentFees.Find(int.Parse(s));
            data.PaymentStatus = "Đã thanh toán";
            _db.SaveChanges();
            if (rErrorCode.Equals("0"))
            {
                ViewBag.Message = "Thanh toán thành công hóa đơn " + rOrderId + " | Mã giao dịch: " + rRequest;
            }
            return View();
        }


    }
}