using Microsoft.AspNet.Identity;
using ResearchOrdersWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ResearchOrdersWebsite.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        // GET: Orders/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Orders/SequencingContract
        [HttpGet]
        public ActionResult SequencingContract()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SequencingContract(SequencingContractViewModel model, HttpPostedFileBase DataInfoFile)
        {
            if (DataInfoFile != null && DataInfoFile.ContentLength > 0)
                model.DataInfoFileName = DataInfoFile.FileName;
            else
            {
                model.DataInfoFileName = null;
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            SetSessionFromSequenceContractViewModel(model, DataInfoFile);
            return View("ConfirmOrder", model);
        }

        [HttpGet]
        public FileResult DownloadBlankDataInfoFile()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"\DownloadableFiles\test_file.txt");
            string fileName = "test_file.txt";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult SubmitOrder()
        {
            try
            {
                BsonDocument orderDoc = null;
                orderDoc = GetOrderDocFromSession();

                IMongoDatabase mongoDB = HttpContext.Application["mongoDB"] as IMongoDatabase;
                var ordersCollection = mongoDB.GetCollection<BsonDocument>("orders");
                var filter = Builders<BsonDocument>.Filter.Eq("ProjectId", orderDoc["ProjectId"]);
                var existingOrdersList = ordersCollection.Find(filter).ToList();
                if (existingOrdersList.Count > 0)
                {
                    ViewBag.ErrorMessage = "已经提交过项目编号为" + orderDoc["ProjectId"] + "的订单了";
                    return View("Error");
                }

                ordersCollection.InsertOne(orderDoc);

                Session.Clear();
                return RedirectToAction("ViewUserOrders", "Orders");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SubmitOrder: " + e.Message);
                ViewBag.ErrorMessage = "提交订单出现了问题，请重试";
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult GetOrderDetailsPartialView(OrderViewModel model)
        {
            return PartialView("_OrderDetailsPartial", model);
        }

        private BsonDocument GetOrderDocFromSession()
        {
            BsonDocument orderDoc = new BsonDocument();
            try
            {
                orderDoc["UserId"] = User.Identity.GetUserId();
                orderDoc["UserName"] = User.Identity.GetUserName();
                orderDoc["OrderType"] = (int)Session["OrderType"];
                orderDoc["OrderSubmissionDate"] = DateTime.Now.Date;

                if ((OrderTypeEnum)Session["OrderType"] == OrderTypeEnum.SequencingContract)
                {
                    orderDoc["Organization"] = (string)Session["Organization"] == null ? "" : (string)Session["Organization"];
                    orderDoc["ProjectId"] = (string)Session["ProjectId"] == null ? "" : (string)Session["ProjectId"];
                    orderDoc["OrderComments"] = (string)Session["OrderComments"] == null ? "" : (string)Session["OrderComments"];
                    orderDoc["NumLanes"] = (string)Session["NumLanes"] == null ? "" : (string)Session["NumLanes"];
                    orderDoc["ProjectTypeSelfDefinition"] = (string)Session["ProjectTypeSelfDefinition"] == null ? "" : (string)Session["ProjectTypeSelfDefinition"];
                    orderDoc["SampleGenus"] = (string)Session["SampleGenus"] == null ? "" : (string)Session["SampleGenus"];
                    orderDoc["SampleSpecies"] = (string)Session["SampleSpecies"] == null ? "" : (string)Session["SampleSpecies"];
                    orderDoc["GeneSize"] = (string)Session["GeneSize"] == null ? "" : (string)Session["GeneSize"];
                    orderDoc["IndexDetails"] = (string)Session["IndexDetails"] == null ? "" : (string)Session["IndexDetails"];
                    orderDoc["MixLaneSize"] = (string)Session["MixLaneSize"] == null ? "" : (string)Session["MixLaneSize"];
                    orderDoc["SampleTypeSelfDefinition"] = (string)Session["SampleTypeSelfDefinition"] == null ? "" : (string)Session["SampleTypeSelfDefinition"];
                    orderDoc["GeneseeqSampleId"] = (string)Session["GeneseeqSampleId"] == null ? "" : (string)Session["GeneseeqSampleId"];
                    orderDoc["DeliveryNumber"] = (string)Session["DeliveryNumber"] == null ? "" : (string)Session["DeliveryNumber"];
                    orderDoc["DataInfoFileName"] = (string)Session["DataInfoFileName"] == null ? "" : (string)Session["DataInfoFileName"];
                    orderDoc["DataInfoFile"] = ((HttpPostedFileBase)Session["DataInfoFile"]).ToBson();

                    orderDoc["SplitData"] = (bool)Session["SplitData"];
                    orderDoc["ProjectType"] = (int)Session["ProjectType"];
                    orderDoc["SampleType"] = (int)Session["SampleType"];
                    orderDoc["SequencingPlatform"] = (int)Session["SequencingPlatform"];
                    orderDoc["CustomerProvidesHardDrive"] = (bool)Session["CustomerProvidesHardDrive"];
                    orderDoc["SampleAlreadyAtGeneseeq"] = (bool)Session["SampleAlreadyAtGeneseeq"];
                }

                return orderDoc;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in GetOrderDocFromSession: " + e.Message);
                return null;
            }
        }

        public ActionResult ViewUserOrders()
        {
            List<OrderViewModel> model = new List<OrderViewModel>();

            try
            {
                IMongoDatabase mongoDB = HttpContext.Application["mongoDB"] as IMongoDatabase;
                var ordersCollection = mongoDB.GetCollection<BsonDocument>("orders");
                var filter = Builders<BsonDocument>.Filter.Eq("UserId", User.Identity.GetUserId());
                var ordersList = ordersCollection.Find(filter).ToList();

                foreach (BsonDocument order in ordersList)
                {
                    model.Add(GetOrderViewModelFromOrderDoc(order));
                }
                return View("ViewUserOrders", model);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ViewUserOrders: " + e.Message);
                ViewBag.ErrorMessage = "查看用户订单出现了问题，请重试";
                return View("Error");
            }
        }

        private OrderViewModel GetOrderViewModelFromOrderDoc(BsonDocument order)
        {
            OrderViewModel model = null;
            try
            {
                if ((int)order["OrderType"] == (int)OrderTypeEnum.SequencingContract)
                {
                    model = new SequencingContractViewModel();
                    model.SetBaseInfo(
                        order["ProjectId"].ToString(),
                        (DateTime)order["OrderSubmissionDate"],
                        (OrderTypeEnum)(int)order["OrderType"],
                        order["UserId"].ToString(),
                        order["UserName"].ToString());

                    (model as SequencingContractViewModel).SetDetails(
                        order["Organization"].ToString(),
                        order["OrderComments"].ToString(),
                        (ProjectTypeEnum)(int)order["ProjectType"],
                        order["NumLanes"].ToString(),
                        order["ProjectTypeSelfDefinition"].ToString(),
                        order["SampleGenus"].ToString(),
                        order["SampleSpecies"].ToString(),
                        order["GeneSize"].ToString(),
                        order["SplitData"].ToBoolean(),
                        order["IndexDetails"].ToString(),
                        (SampleTypeEnum)(int)order["SampleType"],
                        order["MixLaneSize"].ToString(),
                        order["SampleTypeSelfDefinition"].ToString(),
                        (SequencingPlatformEnum)(int)order["SequencingPlatform"],
                        order["CustomerProvidesHardDrive"].ToBoolean(),
                        order["SampleAlreadyAtGeneseeq"].ToBoolean(),
                        order["GeneseeqSampleId"].ToString(),
                        order["DeliveryNumber"].ToString(),
                        order["DataInfoFileName"].ToString()
                        );
                }
                return model;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in GetOrderViewModelFromOrderDoc: " + e.Message);
                return null;
            }
        }

        private bool SetSessionFromSequenceContractViewModel(SequencingContractViewModel model, HttpPostedFileBase DataInfoFile)
        {
            try
            {
                Session["OrderType"] = OrderTypeEnum.SequencingContract;
                Session["Organization"] = model.Organization;
                Session["ProjectId"] = model.ProjectId;
                Session["OrderComments"] = model.OrderComments;
                Session["ProjectType"] = model.ProjectType;
                Session["NumLanes"] = model.NumLanes;
                Session["ProjectTypeSelfDefinition"] = model.ProjectTypeSelfDefinition;
                Session["SampleGenus"] = model.SampleGenus;
                Session["SampleSpecies"] = model.SampleSpecies;
                Session["GeneSize"] = model.GeneSize;
                Session["SplitData"] = model.SplitData;
                Session["IndexDetails"] = model.IndexDetails;
                Session["SampleType"] = model.SampleType;
                Session["MixLaneSize"] = model.MixLaneSize;
                Session["SampleTypeSelfDefinition"] = model.SampleTypeSelfDefinition;
                Session["SequencingPlatform"] = model.SequencingPlatform;
                Session["CustomerProvidesHardDrive"] = model.CustomerProvidesHardDrive;
                Session["SampleAlreadyAtGeneseeq"] = model.SampleAlreadyAtGeneseeq;
                Session["GeneseeqSampleId"] = model.GeneseeqSampleId;
                Session["DeliveryNumber"] = model.DeliveryNumber;
                Session["DataInfoFileName"] = model.DataInfoFileName;
                Session["DataInfoFile"] = DataInfoFile;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SetSessionFromSequenceContractViewModel: " + e.Message);
                return false;
            }
        }
    }
}