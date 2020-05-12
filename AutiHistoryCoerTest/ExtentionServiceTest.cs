using AutoHistory;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AutoHistoryCoerTest
{

    public class testdb : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "testdb");
            base.OnConfiguring(optionsBuilder);

        }

        public DbSet<Student> Students { get; set; }
    }


    [HistoryTrackable]
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string LastName { get; set; }
    }

    public class ExtentionServiceTest
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public ExtentionServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Write_Read_data()
        {
            testdb db = new testdb();
            Student st = new Student()
            {
                Name = "jhon",
                LastName = "Doe"
            };
            db.Add(st);
            db.SaveChangesWithHistory();
        
            var savedStudent = db.Students.FirstOrDefault();
            _testOutputHelper.WriteLine(savedStudent.Hs_Change);
      
            Assert.NotNull(savedStudent.Hs_Change);
        }
    }
}

