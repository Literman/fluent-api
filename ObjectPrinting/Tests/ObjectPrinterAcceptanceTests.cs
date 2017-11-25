using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person { Name = "Alex", Age = 19, Height = 200 };
        }

        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .ExcludeType<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(x => x.ToString())
                //3. Для числовых типов указать культуру
                .Printing<int>().Using(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Age).Using(age => age.ToString())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).CutToLength(2)
                //6. Исключить из сериализации конкретного свойства
                .ExcludeProperty(p => p.Height);

            var s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
            var s2 = person.PrintToString();
            //8. ...с конфигурированием
            var s3 = person.PrintToString(x => x.ExcludeType<Guid>());
        }

        [Test]
        public void ExcludeType()
        {
            var printer = ObjectPrinter.For<Person>()
                .ExcludeType<Guid>()
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tName = Alex\r\n\tHeight = 200\r\n\tAge = 19\r\n");
        }

        [Test]
        public void SerializeType()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>()
                .Using(x => x.ToString("D5"))
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 200\r\n\tAge = 00019\r\n");
        }

        [Test]
        public void SetCulture()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>()
                .Using(CultureInfo.CurrentCulture)
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 200\r\n\tAge = 19\r\n");
        }

        [Test]
        public void SerializeProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Age)
                .Using(age => age.ToString("D5"))
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 200\r\n\tAge = 00019\r\n");
        }

        [Test]
        public void CutStringPropertyValue()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Name)
                .CutToLength(2)
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tId = Guid\r\n\tName = Al\r\n\tHeight = 200\r\n\tAge = 19\r\n");
        }

        [Test]
        public void ExcludeProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .ExcludeProperty(p => p.Height)
                .PrintToString(person);
            printer.Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tAge = 19\r\n");
        }

        [Test]
        public void ExtensionPrint()
        {
            person.PrintToString()
                .Should().Be("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 200\r\n\tAge = 19\r\n");
        }

        [Test]
        public void ExtensionPrint_WithConfigure()
        {
            person.PrintToString(x => x
                    .ExcludeType<Guid>()
                    .ExcludeProperty(p => p.Name))
                .Should().Be("Person\r\n\tHeight = 200\r\n\tAge = 19\r\n");
        }
    }
}