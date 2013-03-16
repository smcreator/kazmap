using KazMap.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using KazMap.Data;

namespace KazMap.Routing.Tests
{
    
    
    /// <summary>
    ///Это класс теста для RoutingModelTest, в котором должны
    ///находиться все модульные тесты RoutingModelTest
    ///</summary>
    [TestClass()]
    public class RoutingServiceTest
    {
        private TestContext testContextInstance;
        // inject routing dependency explicitly
        // TODO: dependency injection implicitly        
        private IRoutingService _routing = new RoutingService();
        private KazMapEntities _db = new KazMapEntities();

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Тест для FindRoute
        ///</summary>
        [TestMethod()]
        public void GetRouteTest()
        {
            // Маршрут: Бульвар Мира-Гоголя
            Intersections startIntersection = 
                _db.Intersections.First(i => i.Id == 0);
            Intersections endIntersection = 
                _db.Intersections.First(i => i.Id == 2);
            Route expected = null; // TODO: инициализация подходящего значения
            Route actual;
            actual = _routing.GetRoute(startIntersection, endIntersection);
            
            Assert.AreEqual(actual.Count, 15);
            Assert.AreEqual(actual[0].RightIntersectionId, 32);
            Assert.AreEqual(actual[1].RightIntersectionId, 695);
            Assert.AreEqual(actual[2].RightIntersectionId, 615);
            Assert.AreEqual(actual[3].RightIntersectionId, 701);
            Assert.AreEqual(actual[4].RightIntersectionId, 706);
            Assert.AreEqual(actual[5].RightIntersectionId, 704);
            Assert.AreEqual(actual[6].RightIntersectionId, 683);
            Assert.AreEqual(actual[7].RightIntersectionId, 649);
            Assert.AreEqual(actual[8].RightIntersectionId, 646);
            Assert.AreEqual(actual[9].RightIntersectionId, 413);
            Assert.AreEqual(actual[10].RightIntersectionId, 414);
            Assert.AreEqual(actual[11].RightIntersectionId, 29);
            Assert.AreEqual(actual[12].RightIntersectionId, 30);
            Assert.AreEqual(actual[13].RightIntersectionId, 276);
            Assert.AreEqual(actual[14].RightIntersectionId, 2);

            // Маршрут: Бульвар Мира-Гоголя
            startIntersection =
                _db.Intersections.First(i => i.Id == 23);
            endIntersection =
                _db.Intersections.First(i => i.Id == 2);
            actual = _routing.GetRoute(startIntersection, endIntersection);

            Assert.AreEqual(actual.Count, 4);
            Assert.AreEqual(actual[0].RightIntersectionId, 54);
            Assert.AreEqual(actual[1].RightIntersectionId, 52);
            Assert.AreEqual(actual[2].RightIntersectionId, 276);
            Assert.AreEqual(actual[3].RightIntersectionId, 2);
        }
    }
}