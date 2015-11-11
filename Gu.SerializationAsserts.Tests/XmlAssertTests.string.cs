﻿namespace Gu.SerializationAsserts.Tests
{

    using NUnit.Framework;

    public class XmlAssertTests
    {
        [Test]
        public void HappyPath()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                      "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                      "  <Outer Attribute=\"meh\">" +
                      "    <Value Attribute=\"1\">2</Value>" +
                      "  </Outer>  " +
                      "</Dummy>";
            XmlAssert.Equal(xml, xml);
        }

        [Test]
        public void WrongEncoding()
        {
            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Dummy />";
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Dummy />";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  Expected string length 48 but was 47.\r\n" +
                           "  Strings differ at line 1 index 34.\r\n" +
                           "  Expected: <?xml version=\"1.0\" encoding=\"utf-16\"?><Dummy />\r\n" +
                           "  But was:  <?xml version=\"1.0\" encoding=\"utf-8\"?><Dummy />\r\n" +
                           "  --------------------------------------------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongRoot()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value>2</Value>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Wrong xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Value>2</Value>\r\n" +
                            "</Wrong>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  String lengths are both 176.\r\n" +
                           "  Strings differ at line 2 index 1.\r\n" +
                           "  Expected: <Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                           "  But was:  <Wrong xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                           "  -----------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void InvalidXmlStartingWithWhiteSpace()
        {
            var xml = " <?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                      "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                      "  <Value>2</Value>\r\n" +
                      "</Dummy>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(xml, xml));
            var expected = "  expected is not valid xml.\r\n" +
                           "  XmlException: Unexpected XML declaration. The XML declaration must be the first node in the document, and no white space characters are allowed to appear before it. Line 1, position 4.";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void InvalidXmlUnmatchedElement()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                      "<Dummy>\r\n" +
                      "  <Value>2</Wrong>\r\n" +
                      "</Dummy>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(xml, xml));
            var expected = "  expected is not valid xml.\r\n" +
                           "  XmlException: The 'Value' start tag on line 3 position 4 does not match the end tag of 'Wrong'. Line 3, position 13.";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongElement()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value>2</Value>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Wrong>2</Wrong>\r\n" +
                            "</Dummy>";


            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  String lengths are both 176.\r\n" +
                           "  Strings differ at line 3 index 1.\r\n" +
                           "  Expected: <Value>2</Value>\r\n" +
                           "  But was:  <Wrong>2</Wrong>\r\n" +
                           "  -----------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongNestedElement()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Outer>\r\n" +
                              "    <Value>2</Value>\r\n" +
                              "  </Outer>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Outer>\r\n" +
                            "    <Wrong>2</Wrong>\r\n" +
                            "  </Outer>\r\n" +
                            "</Dummy>";


            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  String lengths are both 201.\r\n" +
                           "  Strings differ at line 4 index 1.\r\n" +
                           "  Expected: <Value>2</Value>\r\n" +
                           "  But was:  <Wrong>2</Wrong>\r\n" +
                           "  -----------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongElementOrder()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value1>1</Value1>\r\n" +
                              "  <Value2>2</Value2>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Value2>2</Value2>\r\n" +
                            "  <Value1>1</Value1>\r\n" +
                            "</Dummy>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  String lengths are both 200.\r\n" +
                           "  Strings differ at line 3 index 6.\r\n" +
                           "  Expected: <Value1>1</Value1>\r\n" +
                           "  But was:  <Value2>2</Value2>\r\n" +
                           "  ----------------^";

            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongAttributeOrder()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value1 Attribute1=\"1\" Attribute2=\"2\">1</Value1>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Value1 Attribute2=\"2\" Attribute1=\"1\">1</Value1>\r\n" +
                            "</Dummy>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  String lengths are both 208.\r\n" +
                           "  Strings differ at line 3 index 17.\r\n" +
                           "  Expected: <Value1 Attribute1=\"1\" Attribute2=\"2\">1</Value1>\r\n" +
                           "  But was:  <Value1 Attribute2=\"2\" Attribute1=\"1\">1</Value1>\r\n" +
                           "  ---------------------------^";

            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongNestedElementValue()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Outer Attribute=\"meh\">\r\n" + 
                              "    <Value Attribute=\"1\">2</Value>\r\n" +
                              "  </Outer>\r\n" + 
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Outer Attribute=\"meh\">\r\n" +
                              "    <Value Attribute=\"1\">Wrong</Value>\r\n" +
                              "  </Outer>\r\n" +
                              "</Dummy>";


            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  Expected string length 231 but was 235.\r\n" +
                           "  Strings differ at line 4 index 21.\r\n" + 
                           "  Expected: <Value Attribute=\"1\">2</Value>\r\n" + 
                           "  But was:  <Value Attribute=\"1\">Wrong</Value>\r\n" + 
                           "  -------------------------------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongNestedAttribute()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                  "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                  "  <Outer Attribute=\"meh\">\r\n" +
                  "    <Value Attribute=\"1\">2</Value>\r\n" +
                  "  </Outer>\r\n" +
                  "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Outer Attribute=\"meh\">\r\n" +
                              "    <Value Wrong=\"1\">2</Value>\r\n" +
                              "  </Outer>\r\n" +
                              "</Dummy>";

            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  Expected string length 231 but was 227.\r\n" +
                           "  Strings differ at line 4 index 7.\r\n" +
                           "  Expected: <Value Attribute=\"1\">2</Value>\r\n" +
                           "  But was:  <Value Wrong=\"1\">2</Value>\r\n" + 
                           "  -----------------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongNestedAttributeValue()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Outer Attribute=\"meh\">\r\n" +
                              "    <Value Attribute=\"1\">2</Value>\r\n" +
                              "  </Outer>\r\n" +
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                            "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                            "  <Outer Attribute=\"meh\">\r\n" +
                            "    <Value Attribute=\"Wrong\">2</Value>\r\n" +
                            "  </Outer>\r\n" +
                            "</Dummy>";


            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  Expected string length 231 but was 235.\r\n" +
                           "  Strings differ at line 4 index 18.\r\n" + "  Expected: <Value Attribute=\"1\">2</Value>\r\n" +
                           "  But was:  <Value Attribute=\"Wrong\">2</Value>\r\n" + 
                           "  ----------------------------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }

        [Test]
        public void WrongElementValue()
        {
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value>1</Value>\r\n" + 
                              "</Dummy>";

            var actualXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                              "<Dummy xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                              "  <Value>Wrong</Value>\r\n" +
                              "</Dummy>";


            var xmlExt = Assert.Throws<AssertException>(() => XmlAssert.Equal(expectedXml, actualXml));
            var expected = "  Expected string length 176 but was 180.\r\n" +
                           "  Strings differ at line 3 index 7.\r\n" +
                           "  Expected: <Value>1</Value>\r\n" +
                           "  But was:  <Value>Wrong</Value>\r\n" +
                           "  -----------------^";
            Assert.AreEqual(expected, xmlExt.Message);
        }
    }
}
