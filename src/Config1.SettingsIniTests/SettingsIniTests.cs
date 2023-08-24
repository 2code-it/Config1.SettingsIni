using Microsoft.VisualStudio.TestTools.UnitTesting;
using Config1.SettingsIni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NuGet.Frameworks;

namespace Config1.SettingsIni.Tests
{
	[TestClass]
	public class SettingsIniTests
	{
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void When_ReloadWithoutRootSubject_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "key1=value", "[sub2]" });

			new SettingsIni("", fileSystem);
		}

		[TestMethod]
		public void When_GetSubjects_Expect_SubjectsInFile()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]","key1=value","[sub2]" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			string[] subjects = ini.GetSubjects();

			Assert.AreEqual(2, subjects.Length);
			Assert.AreEqual("sub1", subjects[0]);
		}

		[TestMethod]
		public void When_GetKeys_Expect_KeysForSubject()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			string[] keys = ini.GetKeys("sub1");

			Assert.AreEqual(2, keys.Length);
			Assert.AreEqual("key1", keys[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void When_GetKeysForNonExistingSubject_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			ini.GetKeys("sub2");
		}

		[TestMethod]
		[DataRow("sub1", true)]
		[DataRow("sub2", false)]
		public void When_SubjectExists_Expect_WhenExistsTrueElseFalse(string subject, bool expectedResult)
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);

			Assert.AreEqual(expectedResult, ini.SubjectExists(subject));
		}

		[TestMethod]
		[DataRow("sub1", "key1", true)]
		[DataRow("sub2", "key1", false)]
		[DataRow("sub1", "key3", false)]
		public void When_KeyExists_Expect_WhenExistsTrueElseFalse(string subject, string key, bool expectedResult)
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);

			Assert.AreEqual(expectedResult, ini.KeyExists(subject, key));
		}

		[TestMethod]
		[DataRow("sub1", "key1", "value")]
		[DataRow("sub2", "key1", "")]
		[DataRow("sub1", "key3", "")]
		public void When_GetValue_Expect_WhenExistsValueElseEmptyString(string subject, string key, string expectedResult)
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);

			Assert.AreEqual(expectedResult, ini.GetValue(subject, key));
		}

		[TestMethod]
		public void When_GetValuesAndSubjectExists_Expect_ValuesForSubject()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			string[] values = ini.GetValues("sub1");

			Assert.AreEqual(2, values.Length);
			Assert.AreEqual("value", values[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void When_GetValuesAndSubjectNotExists_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=value", "key2=value" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			ini.GetValues("sub2");
		}

		[TestMethod]
		public void When_GetValuesAsAndSubjectExists_Expect_ValuesAsT()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=2.2", "key2=12.3" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			double[] values = ini.GetValuesAs<double>("sub1");

			Assert.AreEqual(2, values.Length);
			Assert.AreEqual(2.2, values[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void When_GetValuesAsAndSubjectNotExists_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=2.2", "key2=12.3" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			ini.GetValuesAs<double>("sub2");
		}

		[TestMethod]
		public void When_GetValueAsAndSubjectAndKeyExists_Expect_ValuesAsT()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=2.2", "key2=12.3" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			double value = ini.GetValueAs<double>("sub1", "key1");

			Assert.AreEqual(2.2, value);
		}

		[TestMethod]
		public void When_GetValueAsAndSubjectExistsButKeyNot_Expect_DefaultT()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "key1=2.2", "key2=12.3" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			double value = ini.GetValueAs<double>("sub1", "key3");

			Assert.AreEqual(0, value);
		}

		[TestMethod]
		public void When_GetSubjectAsAndSubjectExists_Expect_NewObject()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "name=test1", "value=12", "rate=12.4" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			TestItem value = ini.GetSubjectAs<TestItem>("sub1");

			Assert.AreEqual("test1", value.Name);
			Assert.AreEqual(12, value.Value);
			Assert.AreEqual(12.4, value.Rate);
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void When_GetSubjectAsAndPropertyTypeMismatch_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[sub1]", "name=test1", "value=err", "rate=12.4" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			ini.GetSubjectAs<TestItem>("sub1");
		}

		[TestMethod]
		public void When_GetSubjectAsAndSubjectNotSpecified_Expect_TypeNameAsSubject()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[TestItem]", "name=test1", "value=1", "rate=12.4" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			TestItem value = ini.GetSubjectAs<TestItem>();

			Assert.AreEqual("test1", value.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void When_GetSubjectAsAndSubjectNotExists_Expect_Exception()
		{
			IFileSystem fileSystem = Substitute.For<IFileSystem>();
			fileSystem.FileReadAllLines(Arg.Any<string>()).Returns(new[] { "[Test]", "name=test1", "value=1", "rate=12.4" });

			SettingsIni ini = new SettingsIni("", fileSystem);
			ini.GetSubjectAs<TestItem>("sub1");
		}



		private class TestItem
		{
			public string? Name { get; set; }
			public int Value { get; set; }
			public double Rate { get; set; }
		}
	}
}