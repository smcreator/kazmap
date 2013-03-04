using KazMap.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using KazMap.DataAccess;

namespace KazMap.Routing.Tests
{
    
    
    /// <summary>
    ///Это класс теста для RoutingModelTest, в котором должны
    ///находиться все модульные тесты RoutingModelTest
    ///</summary>
    [TestClass()]
    public class RoutingModelTest
    {
        private TestContext testContextInstance;

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
        public void FindRouteTest()
        {
            RoutingModel target = new RoutingModel();
            // Маршрут: Бульвар Мира-Гоголя
            Intersections startIntersection = 
                KazMapConnection.Read.Intersections.First(i => i.Id == 0);
            Intersections endIntersection = 
                KazMapConnection.Read.Intersections.First(i => i.Id == 2);
            Route expected = null; // TODO: инициализация подходящего значения
            Route actual;
            actual = target.FindRoute(startIntersection, endIntersection);
            
            Assert.AreEqual(actual.Count, 15);
            Assert.AreEqual(actual[0].RightId, 32);
            Assert.AreEqual(actual[1].RightId, 695);
            Assert.AreEqual(actual[2].RightId, 615);
            Assert.AreEqual(actual[3].RightId, 701);
            Assert.AreEqual(actual[4].RightId, 706);
            Assert.AreEqual(actual[5].RightId, 704);
            Assert.AreEqual(actual[6].RightId, 683);
            Assert.AreEqual(actual[7].RightId, 649);
            Assert.AreEqual(actual[8].RightId, 646);
            Assert.AreEqual(actual[9].RightId, 413);
            Assert.AreEqual(actual[10].RightId, 414);
            Assert.AreEqual(actual[11].RightId, 29);
            Assert.AreEqual(actual[12].RightId, 30);
            Assert.AreEqual(actual[13].RightId, 276);
            Assert.AreEqual(actual[14].RightId, 2);

            // Маршрут: Бульвар Мира-Гоголя
            startIntersection =
                KazMapConnection.Read.Intersections.First(i => i.Id == 23);
            endIntersection =
                KazMapConnection.Read.Intersections.First(i => i.Id == 2);
            actual = target.FindRoute(startIntersection, endIntersection);

            Assert.AreEqual(actual.Count, 4);
            Assert.AreEqual(actual[0].RightId, 54);
            Assert.AreEqual(actual[1].RightId, 52);
            Assert.AreEqual(actual[2].RightId, 276);
            Assert.AreEqual(actual[3].RightId, 2);
        }
    }
}