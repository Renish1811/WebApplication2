using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class StudentsController : Controller
    {
    public ActionResult Index(string search)
    {
        string selectCmd = "SELECT * FROM Students";

        if (!string.IsNullOrWhiteSpace(search))
        {
            selectCmd += @" WHERE Name LIKE @Search
                        OR Email LIKE @Search
                        OR Phone LIKE @Search
                        OR Course LIKE @Search";
        }

        var conn = new SqlConnection("Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=True;");
        SqlCommand command = new SqlCommand(selectCmd, conn);
        command.CommandType = System.Data.CommandType.Text;

        if (!string.IsNullOrWhiteSpace(search))
        {
            command.Parameters.AddWithValue("@Search", "%" + search + "%");
        }
        conn.Open();
        var result = command.ExecuteReader();

        List<Student> students = new List<Student>();

        while (result.Read())
            {
                Student student = new Student();

            student.ID = Convert.ToInt32(result["ID"]);
            student.Name = result["Name"]?.ToString() ?? "";
            student.Email = result["Email"]?.ToString() ?? "";
            student.Phone = result["Phone"]?.ToString() ?? "";
                student.Course = result["Course"]?.ToString() ?? "";

                if (result["EnrollmentDate"] != DBNull.Value)
                {
                    student.EnrollmentDate = Convert.ToDateTime(result["EnrollmentDate"]);
                }

                    students.Add(student);
            }

            conn.Close();

            ViewBag.Search = search;

            return View("Index", students);
    }


    
    public ActionResult Details(int id)
    {
        return View();
    }

 
    public ActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Student student)
    {

        if (ModelState.IsValid)
        {
            var conn = new SqlConnection("Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=True;");
            conn.Open();

            string insertCmd = "INSERT INTO Students(Name, Email, Phone, Course, EnrollmentDate)" +
                               "VALUES(@Name, @Email, @Phone, @Course, @EnrollmentDate)";
            // $"VALUES('{student.Name}', '{student.Email}', '{student.Phone}', '{student.Course}', '{ Convert.ToDateTime(student.EnrollmentDate).ToString() }')";

            SqlCommand command = new SqlCommand(insertCmd, conn);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Email", student.Email);
            command.Parameters.AddWithValue("@Phone", student.Phone);
            command.Parameters.AddWithValue("@Course", student.Course);
            command.Parameters.AddWithValue("@EnrollmentDate", student.EnrollmentDate);
            command.CommandType = System.Data.CommandType.Text;
            var result = command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index");
        }


        return View(student);
    }
        [HttpGet]
        public IActionResult Search(string searchText)
        {
            string query = @"SELECT * FROM Students
                     WHERE Name LIKE @Search
                        OR Email LIKE @Search
                        OR Phone LIKE @Search
                        OR Course LIKE @Search";

            var students = new List<Student>();

            using (var conn = new SqlConnection(
                "Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchText + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Phone = reader["Phone"].ToString() ?? "",
                                Course = reader["Course"].ToString() ?? "",
                                EnrollmentDate = reader["EnrollmentDate"] != DBNull.Value
                                   ? Convert.ToDateTime(reader["EnrollmentDate"]): DateTime.MinValue
                            });
                        }
                    }
                }
            }

            return PartialView("_StudentList", students);
        }



        [HttpGet]
        // GET: Students/Edit/5
        // This GET supports either loading by id from DB, or receiving values via query string
        public ActionResult Edit(int id, string name = null, string email = null, string phone = null, string course = null, DateTime? enrollmentDate = null)
        {
            // If query string provided student data, use it to pre-populate the edit form (no DB round-trip)
            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(phone) || !string.IsNullOrWhiteSpace(course) || enrollmentDate.HasValue)
            {
                var studentFromQuery = new Student
                {
                    ID = id,
                    Name = name ?? string.Empty,
                    Email = email ?? string.Empty,
                    Phone = phone ?? string.Empty,
                    Course = course ?? string.Empty,
                    EnrollmentDate = enrollmentDate ?? DateTime.MinValue
                };

                return View("Edit", studentFromQuery);
            }

            // Otherwise load from DB by id
            Student student = new Student();

            using (var conn = new SqlConnection(
                "Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=True;"))
            {
                string query = "SELECT * FROM Students WHERE Id = @Id";

                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    conn.Open();

                    using (var result = command.ExecuteReader())
                    {
                        if (result.Read())
                        {
                            student.ID = Convert.ToInt32(result["ID"]);
                            student.Name = result["Name"].ToString() ?? string.Empty;
                            student.Email = result["Email"].ToString() ?? string.Empty;
                            student.Phone = result["Phone"].ToString() ?? string.Empty;
                            student.Course = result["Course"].ToString() ?? string.Empty;

                            if (result["EnrollmentDate"] != DBNull.Value)
                            {
                                student.EnrollmentDate = Convert.ToDateTime(result["EnrollmentDate"]);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return View("Edit", student);
        }

        // POST: Students/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            using (var conn = new SqlConnection(
                "Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=True;"))
            {
                conn.Open();
                string updateCmd = "UPDATE Students SET Name=@Name, Email=@Email, Phone=@Phone, Course=@Course, EnrollmentDate=@EnrollmentDate WHERE ID=@ID";
                using (var cmd = new SqlCommand(updateCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", student.Name ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Email", student.Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Phone", student.Phone ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Course", student.Course ?? string.Empty);
                    cmd.Parameters.AddWithValue("@EnrollmentDate", student.EnrollmentDate);
                    cmd.Parameters.AddWithValue("@ID", student.ID);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var conn = new SqlConnection(
                "Server=localhost\\SQLExpress;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=True;"))
            {
                string query = "DELETE FROM Students WHERE Id = @Id";

                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}


