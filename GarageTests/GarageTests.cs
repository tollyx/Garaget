using Microsoft.VisualStudio.TestTools.UnitTesting;
using Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Tests {
    [TestClass()]
    public class GarageTests {
        [TestMethod()]
        public void Garage_Init() {
            // Arrange
            int capacity = 10;

            // Act
            var garage = new Garage<TestVehicle>(capacity);

            // Assert
            Assert.AreEqual(capacity, garage.Capacity);
        }

        [TestMethod()]
        public void Add_When_Not_Full() {
            // Arrange
            int capacity = 4;
            var garage = new Garage<TestVehicle>(capacity);
            var vehicles = new TestVehicle[] {
                new TestVehicle(),
                new TestVehicle(),
                new TestVehicle(),
                new TestVehicle(),
            };

            // Act
            foreach (var item in vehicles) {
                garage.Add(item);
            }

            // Assert
            int i = 0;
            foreach (var item in garage) {
                Assert.AreSame(vehicles[i++], item);
            }
            Assert.AreEqual(vehicles.Length, garage.Count);
            Assert.AreEqual(vehicles.Length, i);
            
        }

        [TestMethod()]
        public void Add_When_Full() {
            // Arrange
            int capacity = 1;
            var garage = new Garage<TestVehicle>(capacity);
            garage.Add(new TestVehicle());

            // Act
            bool success = garage.Add(new TestVehicle());

            // Assert
            Assert.AreEqual(false, success);
            Assert.AreEqual(1, garage.Count);
            // Just to make sure the iterator agrees 
            Assert.AreEqual(1, garage.Count());
        }

        [TestMethod()]
        public void Enumerator() {
            // Arrange
            int capacity = 10;
            var vehicles = new TestVehicle[] {
                new TestVehicle(),
                new TestVehicle(),
                new TestVehicle(),
                new TestVehicle(),
            };
            var garage = new Garage<TestVehicle>(capacity);
            foreach (var item in vehicles) {
                garage.Add(item);
            }

            // Act
            var result = garage.ToList();

            // Assert
            // Using result.Count instead of vehicles.Length, 
            // to make sure we don't iterate over any null items
            for (int i = 0; i < result.Count; i++) {
                Assert.AreSame(vehicles[i], result[i]);
            }
            Assert.AreEqual(vehicles.Length, result.Count);
        }

        [TestMethod()]
        public void Remove_At_Back_Success() {
            // Arrange
            var garage = new Garage<TestVehicle>(4);
            var car = new TestVehicle();
            garage.Add(new TestVehicle());
            garage.Add(new TestVehicle());
            garage.Add(car);

            // Act
            bool success = garage.Remove(car);

            // Assert
            Assert.IsTrue(success);
            foreach (var item in garage) {
                Assert.AreNotSame(car, item);
            }
            Assert.AreEqual(2, garage.Count);
            Assert.AreEqual(2, garage.Count()); // Just to make sure the iterator agrees 
        }

        [TestMethod()]
        public void Remove_At_Front_Success() {
            // Arrange
            var garage = new Garage<TestVehicle>(4);
            var car = new TestVehicle();
            garage.Add(car);
            garage.Add(new TestVehicle());
            garage.Add(new TestVehicle());

            // Act
            bool success = garage.Remove(car);

            // Assert
            Assert.IsTrue(success);
            foreach (var item in garage) {
                Assert.AreNotSame(car, item);
            }
            Assert.AreEqual(2, garage.Count);
            Assert.AreEqual(2, garage.Count()); // Just to make sure the iterator agrees 
                                                // (this actually caught a bug, see Garage.Remove(T vehicle))
        }

        [TestMethod()]
        public void Remove_Fail() {
            // Arrange
            var garage = new Garage<TestVehicle>(4);
            var car = new TestVehicle();
            garage.Add(new TestVehicle());
            garage.Add(new TestVehicle());

            // Act
            bool success = garage.Remove(car);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(2, garage.Count);
            Assert.AreEqual(2, garage.Count()); // Just to make sure the iterator agrees 
        }

        [TestMethod()]
        public void Remove_Empty_Fail() {
            // Arrange
            var garage = new Garage<TestVehicle>(4);
            var car = new TestVehicle();

            // Act
            bool success = garage.Remove(car);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, garage.Count);
            Assert.AreEqual(0, garage.Count()); // Just to make sure the iterator agrees 
        }
    }
}