using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using WebApplication1.Models; // Assuming your CustomerModel is in a folder named Models

namespace WebApplication1.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Customer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the customer data to the database
                SaveCustomerToDatabase(model);

                // Optionally, you can redirect to another action upon successful registration
                return RedirectToAction("RegistrationSuccess");
            }

            // If model state is not valid, return to the registration form with validation errors
            return View(model);
        }

        // Method to save customer data to the database
        private void SaveCustomerToDatabase(CustomerModel model)
        {
            // Connection string to your SQL Server database
            string connectionString = "Data Source=DESKTOP-CJT3444\\SQLEXPRESS;Initial Catalog=ebook;Integrated Security=True";

            // Create a new connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create SQL command with parameters
                string sql = "INSERT INTO Customers (FirstName, LastName, Email, PasswordHash, CreatedAt) " +
                             "VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CreatedAt)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@FirstName", model.FirstName);
                    command.Parameters.AddWithValue("@LastName", model.LastName);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        // GET: Customer/RegistrationSuccess
        public ActionResult RegistrationSuccess()
        {
            return View();
        }

        // GET: Customer/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Customer/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check username and password against database
                var customer = AuthenticateUser(model.Email, model.Password);
                if (customer != null)
                {
                    // Successful login, redirect to home page
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        // Method to authenticate the user
        private CustomerModel AuthenticateUser(string email, string password)
        {
            CustomerModel customer = null;

            string connectionString = "Data Source=DESKTOP-CJT3444\\SQLEXPRESS;Initial Catalog=ebook;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Customers WHERE Email = @Email AND PasswordHash = @PasswordHash";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = new CustomerModel
                            {
                                CustomerID = (int)reader["CustomerID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                    }
                }
            }

            return customer;
        }

        // GET: Customer/Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
