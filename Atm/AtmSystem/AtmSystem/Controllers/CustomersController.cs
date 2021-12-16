using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtmSystem.Models;
using System.Data.SqlClient;

namespace AtmSystem.Controllers
{
    public class CustomersController : Controller
    {
        CustomerContext customerDb = new CustomerContext();
        public ActionResult Index(Customer customer) => View(customer);
        public ActionResult Withdraw(int id)
        {
            Customer customer = customerDb.CustomerTable.Find(id);
            return View(customer);
        }
        [HttpPost]
        public ActionResult Withdraw(Customer updatedCustomer)
        {
            Customer customer = customerDb.CustomerTable.Find(updatedCustomer.Accountno);
            if (updatedCustomer.Balance >= 300)
            {
                if (customer.Balance >= updatedCustomer.Balance)
                {
                    customer.Balance = customer.Balance - updatedCustomer.Balance;
                    customerDb.SaveChanges();
                    string connetionString;
                    SqlConnection cnn;
                    SqlCommand command;
                    SqlDataReader dataReader;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    String sql,Output="";
                    connetionString = @"Data Source=PRATIKSHAT-PC;Initial Catalog=Amount;User ID=sa;Password=Prorigo";
                    cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    sql = "SELECT Notes FROM Tbl_Notes WHERE Name='1000'";
                   // sql = "SELECT Notes FROM Tbl_Notes WHERE Name='500'";

                    command = new SqlCommand(sql, cnn);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        Output = dataReader.GetValue(0) +"\n";
                        //Console.Write(dataReader1.GetValue(0) + "-" + dataReader1.GetValue(1) + "\n");
                        Console.Write(Output);
                    }
                    dataReader.Close();
                    command.Dispose();
           


                    return RedirectToAction("Index", customer);

                }
                else
                {
                    return RedirectToAction("Insufficient", customer);
                }
            }
            else
            {
                return RedirectToAction("Insufficient", customer);
            }
                
        }
        public ActionResult Insufficient(Customer customer) => View(customer);
        public ActionResult Transfer(int? id)
        {
            Customer customer = customerDb.CustomerTable.Find(id);
            return View(customer);
        }
        [HttpPost]
        public ActionResult Transfer(Customer updatedCustomer, int? receiverCustomerId)
        {
            Customer senderCustomer = customerDb.CustomerTable.Find(updatedCustomer.Accountno);
            Customer receiverCustomer = customerDb.CustomerTable.Find(receiverCustomerId);
            if (senderCustomer.Balance >= updatedCustomer.Balance)
            {
                if (receiverCustomer != null)
                {
                    receiverCustomer.Balance = receiverCustomer.Balance + updatedCustomer.Balance;
                    senderCustomer.Balance = senderCustomer.Balance - updatedCustomer.Balance;
                    customerDb.SaveChanges();
                }
                else
                    return RedirectToAction("InvalidCustomer", senderCustomer);
                return RedirectToAction("Index", senderCustomer);
            }
            else
                return RedirectToAction("Insufficient", senderCustomer);
        }
        public ActionResult InvalidCustomer(Customer customer) => View(customer);
    }
}