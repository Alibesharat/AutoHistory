using AutoHistory;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AutoHistoryCoerTest
{
    public class DbContextTrackerTest : DbContext
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "DbContextTrackerTest");
            base.OnConfiguring(optionsBuilder);


        }

        public DbSet<Student> Students { get; set; }
    }


    public class DbContextTrackerTestService
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public DbContextTrackerTestService(ITestOutputHelper testOutputHelper)
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
